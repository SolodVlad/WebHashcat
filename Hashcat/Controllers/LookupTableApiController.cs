using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace WebHashcat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupTableApiController : ControllerBase
    {
        private readonly LookupTableService _lookupTableService;

        public LookupTableApiController(LookupTableService lookupTableService)
        {
            _lookupTableService = lookupTableService;

            var md5 = MD5.Create();
            var sha1 = SHA1.Create();
            var sha256 = SHA256.Create();
            var sha384 = SHA384.Create();
            var sha512 = SHA512.Create();

            string? password;
            using var streamReader = new StreamReader("E:\\Словари для брута\\test.txt");
            while ((password = streamReader.ReadLine()) != null)
                _lookupTableService.AddAsync(new DataLookupTable()
                {
                    Value = password,
                    MD5 = ComputeHash(Encoding.UTF8.GetBytes(password), md5),
                    SHA1 = ComputeHash(Encoding.UTF8.GetBytes(password), sha1),
                    SHA256 = ComputeHash(Encoding.UTF8.GetBytes(password), sha256),
                    SHA384 = ComputeHash(Encoding.UTF8.GetBytes(password), sha384),
                    SHA512 = ComputeHash(Encoding.UTF8.GetBytes(password), sha512),
                });

            string ComputeHash(byte[] data, HashAlgorithm algorithm)
            {
                using var stream = new MemoryStream(data);
                var hashBytes = algorithm.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }

        [HttpPost]
        public async Task<string?[]> SearchPasswords(string?[] hashes)
        {
            string?[] passwords = new string?[hashes.Length];

            for (int i = 0; i < hashes.Length; i++)
                passwords[i] = (await _lookupTableService.FindAsync(x => x.MD5 == hashes[i] || x.SHA1 == hashes[i] || x.SHA256 == hashes[i] || x.SHA384 == hashes[i] || x.SHA512 == hashes[i])).FirstOrDefault()?.Value;

            return passwords;
        }
    }
}