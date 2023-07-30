using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebHashcat.Models;

namespace WebHashcat.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class HashcatApiController : ControllerBase
    {
        [HttpPost]
        public void Start(HashcatArguments hashcatArguments)
        {
            //string scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6", "hashcat.exe");
            //string workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hashcat-6.2.6");
            
            string scriptPath = "hashcat-6.2.6\\hashcat.exe";
            string workingDirectory = "hashcat-6.2.6";

            string arguments = @$"-m {(int)hashcatArguments.AttackMode} -a 0 {hashcatArguments.Hash} example.dict";

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

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"{e.Data}{Environment.NewLine}"); // вывод данных на консоль
            };

            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit();
        }
    }
}
