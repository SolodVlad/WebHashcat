using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
        private static readonly List<DataLookupTableViewModel> _datas = new List<DataLookupTableViewModel>();

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

        [HttpGet]
        public List<DataLookupTableViewModel> Get() { return _datas; }

        [HttpPost]
        public async Task SearchPasswords([FromBody] string hashesStr)
        {
            var hashesArr = hashesStr.Split(Environment.NewLine);
            //var datas = new List<DataLookupTableViewModel>();

            foreach (var hash in hashesArr)
            {
                var dataLookupTable = new DataLookupTableViewModel
                {
                    Hash = hash,
                    Password = (await _lookupTableService.FindAsync(x => x.MD5 == hash || x.SHA1 == hash || x.SHA256 == hash || x.SHA384 == hash || x.SHA512 == hash)).FirstOrDefault()?.Value,
                    HashType = _checkHashTypeService.GetHashType(hash)
                };

                if (dataLookupTable.Password != null) dataLookupTable.Status = Status.Success;
                else dataLookupTable.Status = Status.Failed;

                _datas.Add(dataLookupTable);
            }
        }
    }
}