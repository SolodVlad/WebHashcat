using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using System.Globalization;
using WebHashcat.Models;

namespace WebHashcat.SignalR
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HashcatHub : Hub
    {
        public void StartHashcat(HashcatArguments hashcatArguments)
        {
            //string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6", "hashcat.exe");
            //string workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6");

            string scriptPath = "hashcat-6.2.6\\hashcat.exe";
            string workingDirectory = "hashcat-6.2.6";

            string arguments = @$"-m {(int)hashcatArguments.HashType} -a 0 {hashcatArguments.Hash} BasicPasswords_+_10-million-password-list-top-1000000_+_piotrcki-wordlist-top10m_+_rockyou_+_PasswordsPro_full.txt --status --potfile-disable";

            ProcessStartInfo startInfo = new()
            {
                FileName = scriptPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory
            };

            using Process process = new();
            process.StartInfo = startInfo;

            var result = new HashcatResult();
            result.Hash = hashcatArguments.Hash;

            process.OutputDataReceived += (sender, e) =>
            {
                var data = e.Data;
                if (!string.IsNullOrEmpty(data))
                {
                    if (data.Contains(hashcatArguments.Hash))
                    {
                        var value = data.Split(':')[1];

                        if (result.Value != null) result.Value = value;
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

                        Clients.Client(Context.ConnectionId).SendAsync("hashcatResult", result);
                    }
                }
            };

            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }

        //Clients.Client(Context.ConnectionId).SendAsync("hashcatResult", e.Data);
        //public void BroadcastMessage(string name, string message)
        //{
        //    Clients.All.SendAsync("broadcastMessage", name, message);
        //}

        //public void Echo(string name, string message)
        //{
        //    Clients.Client(Context.ConnectionId).SendAsync("echo", name, message + " (echo from server)");
        //}
    }
}
