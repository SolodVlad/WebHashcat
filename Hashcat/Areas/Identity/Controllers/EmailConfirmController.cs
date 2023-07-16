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

        public async Task<IActionResult> ConfirmAsync(string guid, string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return Json("USER NOT FOUND");
            var res = await _userManager.ConfirmEmailAsync(user, guid);
            if (res.Succeeded) return RedirectToAction("Index", "Home", new { Area = "" });

            return View("INVALID TOKEN");
        }
    }
}
