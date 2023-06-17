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
        private readonly IMemoryCache _cache;

        public LookupTableApiController(LookupTableService lookupTableService, IMemoryCache cache)
        {
            _lookupTableService = lookupTableService;
            _checkHashTypeService = new CheckHashTypeService();
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Get() => Ok((List<DataLookupTableViewModel>)_cache.Get("dataLookupTableCache"));

        [HttpPost]
        public async Task<IActionResult> SearchPasswords([FromBody] string hashesStr)
        {
            var hashesArr = hashesStr.Split("\n");
            var datas = new List<DataLookupTableViewModel>();

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

                datas.Add(dataLookupTable);
            }

            _cache.Set("dataLookupTableCache", datas);

            return Ok();
        }
    }
}