using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Renci.SshNet;
using System.Diagnostics;
using System.Globalization;
using WebHashcat.Models;

namespace WebHashcat.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HashcatHub : Hub
    {
        private readonly string _scriptPath;
        private readonly string _workingDirectory;

        private readonly IEmailSender _emailSender;

        private bool isMultipleHashTypes;
        private bool isOneHashType;
        private bool isNotHash;

        private readonly string _host;
        private readonly string _username;
        private readonly string _password;

        public HashcatHub(IEmailSender emailSender, IConfiguration configuration)
        {
            _scriptPath = "hashcat-6.2.6\\hashcat.exe";
            _workingDirectory = "hashcat-6.2.6";
            _emailSender = emailSender;

            _host = configuration.GetValue<string>("SSH-Host");
            _username = configuration.GetValue<string>("SSH-Username");
            _password = configuration.GetValue<string>("SSH-Password");
        }

        public async Task StartAutodetectModeHashcatAsync(string hash)
        {
            //string scriptPath = "hashcat-6.2.6\\hashcat.exe";
            //string workingDirectory = "hashcat-6.2.6";

            //////string host = "20.215.192.48";
            //////string username = "KaliVMForWebhashcat";
            //////string password = "VM0mrj-fy8p-kpmz";
            string command = $"hashcat -a 0 {hash} ~/wordlists/hashkiller-dict.txt --status --potfile-disable"; // Замените на фактическую команду hashcat и ее аргументы

            using var client = new SshClient(_host, _username, _password);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    using var shellStream = client.CreateShellStream("", 0, 0, 0, 0, 1024);
                    // Отправляем команду на SSH-сервер
                    shellStream.WriteLine(command);

                    // Читаем и выводим результат построчно
                    string line;
                    while ((line = shellStream.ReadLine()) != null)
                    {
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
                                //process.Kill();
                            }
                        }
                        else if (isNotHash) await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", null, null);
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to the remote server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Disconnect();
            }

            //ProcessStartInfo startInfo = new()
            //{
            //    FileName = _scriptPath,
            //    Arguments = arguments,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //    WorkingDirectory = _workingDirectory
            //};

            //using Process process = new();
            //process.StartInfo = startInfo;

            //process.OutputDataReceived += async (sender, e) =>
            //{
            //    var data = e.Data;
            //    if (!string.IsNullOrEmpty(data))
            //    {
            //                        }
            //};

            //process.Start();
            //process.BeginOutputReadLine();

            //process.WaitForExit();
        }

        public async Task StartCrackHashcatAsync(HashcatArguments hashcatArguments)
        {
            //string host = "20.215.192.48";
            //string username = "KaliVMForWebhashcat";
            //string password = "VM0mrj-fy8p-kpmz";
            string command = $"hashcat -m {hashcatArguments.HashType} -a 0 {hashcatArguments.Hash} ~/wordlists/hashkiller-dict.txt  --status --potfile-disable"; // Замените на фактическую команду hashcat и ее аргументы

            using var client = new SshClient(_host, _username, _password);
            try
            {
                client.Connect();
                if (client.IsConnected)
                {
                    using var shellStream = client.CreateShellStream(Guid.NewGuid().ToString(), 0, 0, 0, 0, 1024);
                    // Отправляем команду на SSH-сервер
                    shellStream.WriteLine(command);

                    var result = new HashcatResult { Hash = hashcatArguments.Hash };

                    // Читаем и выводим результат построчно
                    string line;
                    while ((line = shellStream.ReadLine()) != null)
                    {
                        if (line.Contains(hashcatArguments.Hash))
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
                        else if (line.Contains("Hash.Target"))
                        {
                            var hash = line.Split(':')[1].TrimStart();

                            result.Hash = hash;
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
                    }
                }
                else
                {
                    Console.WriteLine("Failed to connect to the remote server.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Disconnect();
            }

            //string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6", "hashcat.exe");
            //string workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6");

            //string scriptPath = "hashcat-6.2.6\\hashcat.exe";
            //string workingDirectory = "hashcat-6.2.6";

            //string arguments = @$"-m {hashcatArguments.HashType} -a 0 {hashcatArguments.Hash} hashkiller-dict.txt --status --potfile-disable";

            //ProcessStartInfo startInfo = new()
            //{
            //    FileName = _scriptPath,
            //    Arguments = arguments,
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //    WorkingDirectory = _workingDirectory
            //};

            //using Process process = new();
            //process.StartInfo = startInfo;

            //_processes.Add(hashcatArguments.Hash, process);

            //var result = new HashcatResult { Hash = hashcatArguments.Hash };

            //process.OutputDataReceived += async (sender, e) =>
            //{
            //    var data = e.Data;
            //    if (!string.IsNullOrEmpty(data))
            //    {
            //        if (data.Contains(hashcatArguments.Hash))
            //        {
            //            var value = data.Split(':')[1].TrimStart();

            //            if (value != hashcatArguments.Hash) result.Value = value;
            //        }
            //        else if (data.Contains("Status"))
            //        {
            //            var status = data.Split(':')[1].TrimStart();

            //            result.Status = (Status)Enum.Parse(typeof(Status), status);
            //        }
            //        else if (data.Contains("Hash.Mode"))
            //        {
            //            var startIndex = data.IndexOf('(');
            //            var endIndex = data.LastIndexOf(')');
            //            var hashType = data.Substring(startIndex + 1, endIndex - startIndex - 1);

            //            result.HashType = hashType;
            //        }
            //        else if (data.Contains("Hash.Target"))
            //        {
            //            var hash = data.Split(':')[1].TrimStart();

            //            result.Hash = hash;
            //        }
            //        else if (data.Contains("Time.Started"))
            //        {
            //            var dataArr = data.Split(':');
            //            var timeStarted = $"{dataArr[1][5..]}:{dataArr[2]}:{dataArr[3].Split(' ')[0]}";
            //            var timePassed = data.Split('(')[1].Replace(")", "");

            //            var parsedTimeStarted = DateTime.ParseExact(timeStarted, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture);

            //            result.TimeStarted = parsedTimeStarted;
            //            result.TimePassed = timePassed;
            //        }
            //        else if (data.Contains("Time.Estimated"))
            //        {
            //            var dataArr = data.Split(':');
            //            var timeEstimated = $"{dataArr[1][5..]}:{dataArr[2]}:{dataArr[3].Split(' ')[0]}";
            //            var timeLeft = data.Split('(')[1].Replace(")", "");

            //            var parsedTimeEstimated = DateTime.ParseExact(timeEstimated, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture);

            //            result.TimeEstimated = parsedTimeEstimated;
            //            result.TimeLeft = timeLeft;
            //        }
            //        else if (data.Contains("Progress"))
            //        {
            //            var progressPercentage = data.Split(':')[1].Split('(')[1].Replace(")", "").Replace("%", "");

            //            result.Progress = double.Parse(progressPercentage, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            //            await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", result);

            //            if (result.Value != null)
            //                _emailSender.SendEmailAsync(Context.UserIdentifier, "ВАШ ХЕШ БУВ УСПІШНО ЗЛАМАНИЙ", $"Значення хешу {hashcatArguments.Hash}: {result.Value}");
            //        }
            //    }
            //};

            //process.Start();
            //process.BeginOutputReadLine();

            //process.WaitForExit();
        }

        //public async Task StopCrack(string hash)
        //{
        //    _processes[hash].Kill();
        //    _processes.Remove(hash);
        //    await Clients.User(Context.UserIdentifier).SendAsync("stopCrack", hash);
        //}
    }
}