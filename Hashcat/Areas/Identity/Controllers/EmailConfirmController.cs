using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebHashcat.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class EmailConfirmController : Controller
    {
        private readonly UserManager<User> _userManager;

        public EmailConfirmController(UserManager<User> userManager) => _userManager = userManager;

        [HttpGet]
        [Route("EmailConfirm")]
        public async Task<IActionResult> EmailConfirmAsync(string guid, string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return BadRequest("USER NOT FOUND");
            var res = await _userManager.ConfirmEmailAsync(user, guid);
            if (res.Succeeded) return RedirectToAction("Index", "Home", new { Area = "" });

            return BadRequest("INVALID TOKEN");
        }
    }
}
