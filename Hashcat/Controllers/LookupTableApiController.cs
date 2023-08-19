using BLL.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
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

        public LookupTableApiController(LookupTableService lookupTableService, IMemoryCache cache)
        {
            _lookupTableService = lookupTableService;
            _checkHashTypeService = new CheckHashTypeService();
        }

        [HttpPost]
        public async Task<IActionResult> SearchPasswords([FromBody] string hashesStr)
        {
            var hashesArr = hashesStr.Split("\n");
            var datas = new List<DataLookupTableViewModel>();

            foreach (var hash in hashesArr)
            {
                //var upperHash = hash.ToUpper();
                var hashType = _checkHashTypeService.GetHashType(hash);
                var password = "";

                switch (hashType)
                {
                    case Enums.HashType.None: break;
                    case Enums.HashType.MD5: password = (await _lookupTableService.FindAsync(x => x.MD5 == hash)).FirstOrDefault()?.Value; break;
                    case Enums.HashType.SHA1: password = (await _lookupTableService.FindAsync(x => x.SHA1 == hash)).FirstOrDefault()?.Value; break;
                    case Enums.HashType.SHA256: password = (await _lookupTableService.FindAsync(x => x.SHA256 == hash)).FirstOrDefault()?.Value; break;
                    case Enums.HashType.SHA384: password = (await _lookupTableService.FindAsync(x => x.SHA384 == hash)).FirstOrDefault()?.Value; break;
                    case Enums.HashType.SHA512: password = (await _lookupTableService.FindAsync(x => x.SHA512 == hash)).FirstOrDefault()?.Value; break;
                }

                var dataLookupTable = new DataLookupTableViewModel
                {
                    Hash = hash,
                    Password = password,
                    HashType = hashType
                };

                if (!string.IsNullOrEmpty(dataLookupTable.Password)) dataLookupTable.Status = Status.Success;
                else dataLookupTable.Status = Status.Failed;

                datas.Add(dataLookupTable);
            }

            return Ok(datas);
        }
    }
}