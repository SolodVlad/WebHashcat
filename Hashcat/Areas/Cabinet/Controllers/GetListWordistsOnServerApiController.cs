using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using System.Configuration;

namespace WebHashcat.Areas.Cabinet.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class GetListWordistsOnServerApiController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public GetListWordistsOnServerApiController(IConfiguration configuration) => _configuration = configuration;

        public void ListWordlistsOnServer()
        {
            using var client = new SshClient(_configuration.GetValue<string>("SSH-Host"), _configuration.GetValue<string>("SSH-Username"), _configuration.GetValue<string>("SSH-Password"));
            client.Connect();

            using var sftp = new SftpClient(client.ConnectionInfo);
            sftp.Connect();

            var files = sftp.ListDirectory("/home/KaliVMForWebhashcat/wordlists");

            foreach (var file in files)
                if (!file.IsDirectory)
                {
                    var size = file.Length / (1024.0 * 1024.0);
                    Console.WriteLine($"Имя файла: {file.Name}");
                    Console.WriteLine($"Размер файла: {file.Length / (1024.0 * 1024.0)} МБ");
                }

            sftp.Disconnect();
            client.Disconnect();
        }
    }
}