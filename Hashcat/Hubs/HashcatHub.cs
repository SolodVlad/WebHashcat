using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
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

        private readonly Dictionary<string, Process> _processes;

        public HashcatHub(IEmailSender emailSender)
        {
            _scriptPath = "hashcat-6.2.6\\hashcat.exe";
            _workingDirectory = "hashcat-6.2.6";
            _processes = new Dictionary<string, Process>();
            _emailSender = emailSender;
        }

        public async Task StartAutodetectModeHashcat(string hash)
        {
            //string scriptPath = "hashcat-6.2.6\\hashcat.exe";
            //string workingDirectory = "hashcat-6.2.6";

            string arguments = @$"-a 0 {hash} hashkiller-dict.txt --status --potfile-disable";

            ProcessStartInfo startInfo = new()
            {
                FileName = _scriptPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = _workingDirectory
            };

            using Process process = new();
            process.StartInfo = startInfo;

            process.OutputDataReceived += async (sender, e) =>
            {
                var data = e.Data;
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains("Autodetecting hash-modes. Please be patient...Autodetected hash-modesThe following")) isMultipleHashTypes = true;
                    else if (data.Contains("The following mode was auto-detected as the only one matching your input hash:")) isOneHashType = true;
                    else if (data.Contains("Autodetecting hash-modes. Please be patient...Autodetected hash-modesStarted")) isNotHash = true;

                    if (isMultipleHashTypes)
                    {
                        if (data.Contains('|') && !data.Contains("Name"))
                        {
                            var numberHashType = data.Split('|')[0].Trim();
                            var hashType = data.Split('|')[1].Trim();
                            await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", numberHashType, hashType);
                        }
                    }
                    else if (isOneHashType)
                    {
                        if (data.Contains('|'))
                        {
                            var numberHashType = data.Split('|')[0].Trim();
                            var hashType = data.Split('|')[1].Trim();
                            await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", numberHashType, hashType);
                            process.Kill();
                        }
                    }
                    else if (isNotHash) await Clients.User(Context.UserIdentifier).SendAsync("hashTypeResult", null, null);
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }

        public async Task StartCrackHashcat(HashcatArguments hashcatArguments)
        {
            //string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6", "hashcat.exe");
            //string workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6");

            //string scriptPath = "hashcat-6.2.6\\hashcat.exe";
            //string workingDirectory = "hashcat-6.2.6";

            string arguments = @$"-m {hashcatArguments.HashType} -a 0 {hashcatArguments.Hash} hashkiller-dict.txt --status --potfile-disable";

            ProcessStartInfo startInfo = new()
            {
                FileName = _scriptPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = _workingDirectory
            };

            using Process process = new();
            process.StartInfo = startInfo;

            _processes.Add(hashcatArguments.Hash, process);

            var result = new HashcatResult { Hash = hashcatArguments.Hash };

            process.OutputDataReceived += async (sender, e) =>
            {
                var data = e.Data;
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains(hashcatArguments.Hash))
                    {
                        var value = data.Split(':')[1].TrimStart();

                        if (value != hashcatArguments.Hash) result.Value = value;
                    }
                    else if (data.Contains("Status"))
                    {
                        var status = data.Split(':')[1].TrimStart();

                        result.Status = (Status)Enum.Parse(typeof(Status), status);
                    }
                    else if (data.Contains("Hash.Mode"))
                    {
                        var startIndex = data.IndexOf('(');
                        var endIndex = data.LastIndexOf(')');
                        var hashType = data.Substring(startIndex + 1, endIndex - startIndex - 1);

                        result.HashType = hashType;
                    }
                    else if (data.Contains("Hash.Target"))
                    {
                        var hash = data.Split(':')[1].TrimStart();

                        result.Hash = hash;
                    }
                    else if (data.Contains("Time.Started"))
                    {
                        var dataArr = data.Split(':');
                        var timeStarted = $"{dataArr[1][5..]}:{dataArr[2]}:{dataArr[3].Split(' ')[0]}";
                        var timePassed = data.Split('(')[1].Replace(")", "");

                        var parsedTimeStarted = DateTime.ParseExact(timeStarted, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture);

                        result.TimeStarted = parsedTimeStarted;
                        result.TimePassed = timePassed;
                    }
                    else if (data.Contains("Time.Estimated"))
                    {
                        var dataArr = data.Split(':');
                        var timeEstimated = $"{dataArr[1][5..]}:{dataArr[2]}:{dataArr[3].Split(' ')[0]}";
                        var timeLeft = data.Split('(')[1].Replace(")", "");

                        var parsedTimeEstimated = DateTime.ParseExact(timeEstimated, "MMM dd HH:mm:ss", CultureInfo.InvariantCulture);

                        result.TimeEstimated = parsedTimeEstimated;
                        result.TimeLeft = timeLeft;
                    }
                    else if (data.Contains("Progress"))
                    {
                        var progressPercentage = data.Split(':')[1].Split('(')[1].Replace(")", "").Replace("%", "");

                        result.Progress = double.Parse(progressPercentage, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                        await Clients.User(Context.UserIdentifier).SendAsync("hashcatResult", result);

                        if (result.Value != null)
                            _emailSender.SendEmailAsync(Context.UserIdentifier, "ВАШ ХЕШ БУВ УСПІШНО ЗЛАМАНИЙ", $"Значення хешу {hashcatArguments.Hash}: {result.Value}");
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }

        public async Task StopCrack(string hash)
        {
            _processes[hash].Kill();
            _processes.Remove(hash);
            await Clients.User(Context.UserIdentifier).SendAsync("stopCrack", hash);
        }
    }
}