using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using WebHashcat.Services;
using WebHashcat.ViewModels;

namespace WebHashcat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupTableApiController : ControllerBase
    {
        private readonly LookupTableService _lookupTableService;
        private readonly CheckHashTypeService _checkHashTypeService;

        public LookupTableApiController(LookupTableService lookupTableService)
        {
            _lookupTableService = lookupTableService;
            _checkHashTypeService = new CheckHashTypeService();

            //var md5 = MD5.Create();
            //var sha1 = SHA1.Create();
            //var sha256 = SHA256.Create();
            //var sha384 = SHA384.Create();
            //var sha512 = SHA512.Create();

            //string? password;
            //using var streamReader = new StreamReader("E:\\Словари для брута\\test.txt");
            //while ((password = streamReader.ReadLine()) != null)
            //    _lookupTableService.AddAsync(new DataLookupTable()
            //    {
            //        Value = password,
            //        MD5 = ComputeHash(Encoding.UTF8.GetBytes(password), md5),
            //        SHA1 = ComputeHash(Encoding.UTF8.GetBytes(password), sha1),
            //        SHA256 = ComputeHash(Encoding.UTF8.GetBytes(password), sha256),
            //        SHA384 = ComputeHash(Encoding.UTF8.GetBytes(password), sha384),
            //        SHA512 = ComputeHash(Encoding.UTF8.GetBytes(password), sha512),
            //    });

            //string ComputeHash(byte[] data, HashAlgorithm algorithm)
            //{
            //    using var stream = new MemoryStream(data);
            //    var hashBytes = algorithm.ComputeHash(stream);
            //    return BitConverter.ToString(hashBytes).Replace("-", "");
            //}
        }

        [HttpPost]
        public async Task<DataLookupTableViewModel[]> SearchPasswords(string hashesStr)
        {
            var hashesArr = hashesStr.Split(Environment.NewLine);
            var datas = new DataLookupTableViewModel[hashesArr.Length];

            for (int i = 0; i < hashesArr.Length; i++)
            {
                datas[i].Password = (await _lookupTableService.FindAsync(x => x.MD5 == hashesArr[i] || x.SHA1 == hashesArr[i] || x.SHA256 == hashesArr[i] || x.SHA384 == hashesArr[i] || x.SHA512 == hashesArr[i])).FirstOrDefault()?.Value;
                datas[i].HashType = _checkHashTypeService.GetHashType(hashesArr[i]);

                if (datas[i].Password != null) datas[i].Status = Status.Success;
                else datas[i].Status = Status.Failed;
            }

            return datas;
        }
    }
}