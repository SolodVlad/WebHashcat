using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;
using System.Globalization;
using WebHashcat.Areas.Cabinet.Services;
using WebHashcat.Models;

namespace WebHashcat.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HashcatHub : Hub
    {
        private readonly IEmailSender _emailSender;

        private bool isMultipleHashTypes;
        private bool isOneHashType;
        private bool isNotHash;

        private readonly string _sshHost;
        private readonly string _sshUsername;
        private readonly string _sshPassword;

        private readonly ShellStreamService _userSessionsManager;

        public HashcatHub(IEmailSender emailSender, IConfiguration configuration, ShellStreamService userSessionsManager)
        {
            _emailSender = emailSender;

            _sshHost = configuration.GetValue<string>("SSH-Host");
            _sshUsername = configuration.GetValue<string>("SSH-Username");
            _sshPassword = configuration.GetValue<string>("SSH-Password");

            _userSessionsManager = userSessionsManager;
        }

        public async Task StartAutodetectModeHashcatAsync(string hash)
        {
            using var client = new SshClient(_sshHost, _sshUsername, _sshPassword);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    var guid = Guid.NewGuid().ToString();
                    var command = $"hashcat -a 0 {hash} ~/wordlists/hashkiller-dict.txt --status --potfile-disable --session {guid}";

                    using var shellStream = client.CreateShellStream(guid, 0, 0, 0, 0, 1024);

                    shellStream.WriteLine(command);

                    var line = "";
                    while (true)
                    {
                        line = shellStream.ReadLine();

                        if (line.Contains("Stopped"))
                            break;

                        if (line.Contains("hash-modes match the structure of your input hash:"))
                            isMultipleHashTypes = true;
                        else if (line.Contains("The following mode was auto-detected as the only one matching your input hash:"))
                            isOneHashType = true;
                        else if (line.Contains("No hash-mode matches the structure of the input hash."))
                            isNotHash = true;

                        if (isMultipleHashTypes)
                        {
                            if (line.Contains('|') && !line.Contains("Name"))
                            {
                                var numberHashType = line.Split('|')[0].Trim();
                                var hashType = line.Split('|')[1].Trim();
                                await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", numberHashType, hashType);
                            }
                        }
                        else if (isOneHashType)
                        {
                            if (line.Contains('|'))
                            {
                                var numberHashType = line.Split('|')[0].Trim();
                                var hashType = line.Split('|')[1].Trim();
                                await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", numberHashType, hashType);
                            }
                        }
                        else if (isNotHash) await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", null, null);
                    }
                }
                else await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", "Failed to connect to the remote server.", null);
            }
            catch (Exception ex)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", $"An error occurred: {ex.Message}", null);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public async Task StartCrackHashcatAsync(HashcatArguments hashcatArguments)
        {
            using var client = new SshClient(_sshHost, _sshUsername, _sshPassword);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    var guid = Guid.NewGuid().ToString();
                    var command = $"hashcat -m {hashcatArguments.HashType} -a 0 {hashcatArguments.Hash} ~/wordlists/hashkiller-dict.txt  --status --potfile-disable --session {guid}";
                    using var shellStream = client.CreateShellStream(hashcatArguments.Hash, 0, 0, 0, 0, 1024);
                    shellStream.WriteLine(command);
                    var result = new HashcatResult { Hash = hashcatArguments.Hash };

                    _userSessionsManager.AddToCurrentUser(Context.UserIdentifier, hashcatArguments.Hash, shellStream);

                    var line = "";
                    while (true)
                    {
                        line = shellStream.ReadLine();

                        if (line.Contains(hashcatArguments.Hash) && line != command)
                        {
                            var value = line.Split(':')[1].TrimStart();

                            if (value != hashcatArguments.Hash) result.Value = value;
                        }
                        else if (line.Contains("Status"))
                        {
                            var status = line.Split(':')[1].TrimStart();

                            result.Status = (Status)Enum.Parse(typeof(Status), status);
                        }
                        else if (line.Contains("Hash.Mode"))
                        {
                            var startIndex = line.IndexOf('(');
                            var endIndex = line.LastIndexOf(')');
                            var hashType = line.Substring(startIndex + 1, endIndex - startIndex - 1);

                            result.HashType = hashType;
                        }
                        else if (line.Contains("Time.Started"))
                        {
                            var dataArr = line.Split(':');
                            var timeStarted = $"{dataArr[1][5..]}:{dataArr[2]}:{dataArr[3].Split(' ')[0]}";
                            var timePassed = line.Split('(')[1].Replace(")", "");

                            var parsedTimeStarted = DateTime.ParseExact(timeStarted, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture);

                            result.TimeStarted = parsedTimeStarted;
                            result.TimePassed = timePassed;
                        }
                        else if (line.Contains("Time.Estimated"))
                        {
                            var dataArr = line.Split(':');
                            var timeEstimated = $"{dataArr[1][5..]}:{dataArr[2]}:{dataArr[3].Split(' ')[0]}";
                            var timeLeft = line.Split('(')[1].Replace(")", "");

                            var parsedTimeEstimated = DateTime.ParseExact(timeEstimated, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture);

                            result.TimeEstimated = parsedTimeEstimated;
                            result.TimeLeft = timeLeft;
                        }
                        else if (line.Contains("Progress"))
                        {
                            var progressPercentage = line.Split(':')[1].Split('(')[1].Replace(")", "").Replace("%", "");

                            result.Progress = double.Parse(progressPercentage, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                            await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", result);

                            if (result.Value != null)
                                await _emailSender.SendEmailAsync(Context.UserIdentifier, "ВАШ ХЕШ БУВ УСПІШНО ЗЛАМАНИЙ", $"Значення хешу {hashcatArguments.Hash}: {result.Value}");
                        }
                        else if (line.Contains("Stopped")) break;
                    }

                    shellStream.Close();
                }
                else
                {
                    await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", "Failed to connect to the remote server.");
                }
            }
            catch (Exception ex) 
            {
                await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", ex.Message);
            }
            finally
            {
                _userSessionsManager.RemoveFromUserSessionAsync(Context.UserIdentifier, hashcatArguments.Hash);
                client.Disconnect();
            }
        }

        public async Task ManageRunningRecovery(string hash, string argument)
        {
            using var client = new SshClient(_sshHost, _sshUsername, _sshPassword);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    //var command = $"hashcat q --session {_userSessionsManager.GetValueFromUserSessionAsync(Context.UserIdentifier, hash)}";
                    //using var shellStream = client.CreateShellStream(Guid.NewGuid().ToString(), 0, 0, 0, 0, 1024);

                    var shellStream = _userSessionsManager.GetShellStreamFromCurrentUser(Context.UserIdentifier, hash);
                    shellStream.WriteLine(argument);
                    shellStream.Flush();
                }
                else await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", "Failed to connect to the remote server.");
            }
            catch (Exception ex)
            {
                await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", ex.Message);
            }
            finally
            {
                client.Disconnect();
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Этот метод вызывается при разрыве соединения клиента с хабом.
            // Здесь вы можете выполнять необходимые действия при обрыве соединения.
            return base.OnDisconnectedAsync(exception);
        }
    }
}