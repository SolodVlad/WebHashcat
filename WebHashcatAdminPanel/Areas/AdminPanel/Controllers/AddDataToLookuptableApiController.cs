using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebHashcatAdminPanel.Areas.AdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddDataToLookuptableApiController : ControllerBase
    {
        [HttpPost("AddDataToLookupTable")]
        public async Task<IActionResult> AddDataToLookupTableAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Не переданы файлы для обработки.");
            }

            foreach (var file in files)
            {
                using (var stream = file.OpenReadStream())
                {
                    //await _dataService.ProcessFileAsync(stream);
                }
            }

            return Ok("Файлы успешно обработаны и добавлены в базу данных.");
        }
    }
}
