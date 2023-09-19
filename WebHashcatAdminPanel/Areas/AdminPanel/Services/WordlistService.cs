using BLL.Services;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace WebHashcatAdminPanel.Areas.AdminPanel.Services
{
    public class WordlistService
    {
        private readonly LookupTableService _lookupTableService;

        private readonly IConfiguration _configuration;

        public WordlistService(LookupTableService lookupTableService, IConfiguration configuration)
        {
            _lookupTableService = lookupTableService;
            _configuration = configuration;
        }

        public async Task AddDataToLookuptableAsync(Stream fileStream)
        {
            using var md5 = MD5.Create();
            using var sha1 = SHA1.Create();
            using var sha256 = SHA256.Create();
            using var sha384 = SHA384.Create();
            using var sha512 = SHA512.Create();

            using var streamReader = new StreamReader(fileStream);
            string? value;
            while ((value = await streamReader.ReadLineAsync()) != null)
            {
                var newData = new DataLookupTable()
                {
                    Value = value,
                    MD5 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), md5),
                    SHA1 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha1),
                    SHA256 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha256),
                    SHA384 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha384),
                    SHA512 = await ComputeHashAsync(Encoding.UTF8.GetBytes(value), sha512),
                };

                await _lookupTableService.AddAsync(newData);
            }
        }

        public void UploadWordlistToServer(Stream fs, string fileName)
        {
            using var client = new SshClient(_configuration.GetValue<string>("SSH-Host"), _configuration.GetValue<string>("SSH-Username"), _configuration.GetValue<string>("SSH-Password"));
            client.Connect();
            using var sftp = new SftpClient(client.ConnectionInfo);
            sftp.Connect();
            var remoteFilePath = $"/home/KaliVMForWebhashcat/wordlists/{fileName}";
            using (var remoteFileStream = sftp.Create(remoteFilePath))
                sftp.UploadFile(fs, remoteFilePath);
            sftp.Disconnect();
            client.Disconnect();
        }

        private static async Task<string> ComputeHashAsync(byte[] data, HashAlgorithm algorithm)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await algorithm.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }   
    }
}