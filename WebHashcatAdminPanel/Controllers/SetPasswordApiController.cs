using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebHashcatAdminPanel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetPasswordApiController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public SetPasswordApiController(UserManager<User> userManager) => _userManager = userManager;

        [HttpPost]
        [Route("SetPassword")]
        public async Task<IActionResult> SetPassword([FromBody] string password)
        {
            var defUsername = "admin";
            var defPass = "!1Adminadminadmin";

            var admin = await _userManager.FindByNameAsync(defUsername);
            var res = await _userManager.ChangePasswordAsync(admin, defPass, password);
            if (res.Succeeded) return Ok();
            else return BadRequest(res.Errors);
        }
    }
}
