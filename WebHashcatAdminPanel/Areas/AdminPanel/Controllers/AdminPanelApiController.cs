using BLL.Services;
using Microsoft.AspNetCore.Mvc;
using WebHashcatAdminPanel.Areas.AdminPanel.Services;

namespace WebHashcatAdminPanel.Areas.AdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPanelApiController : ControllerBase
    {
        private readonly WordlistService _wordlistService;

        public AdminPanelApiController(LookupTableService lookupTableService, IConfiguration configuration) => _wordlistService = new WordlistService(lookupTableService, configuration);

        [HttpPost("AddDataToLookupTable")]
        public async Task<IActionResult> AddDataToLookupTableAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return BadRequest("Не передані файли для обробки.");

            foreach (var file in files)
            {
                using var stream = file.OpenReadStream();
                await _wordlistService.AddDataToLookuptableAsync(stream);
            }

            return Ok();
        }

        [HttpPost("UploadWordlistToServer")]
        public IActionResult UploadWordlistToServer(List<IFormFile> files)
        {
            if (files == null || files.Count == 0) return BadRequest("Не передані файли для обробки.");

            foreach (var file in files)
            {
                using var stream = file.OpenReadStream();
                _wordlistService.UploadWordlistToServer(stream, file.FileName);
            }

            return Ok();
        }
    }
}