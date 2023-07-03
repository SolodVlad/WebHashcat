using Microsoft.AspNetCore.Mvc;
using WebHashcat.Areas.Identity.Models;

namespace WebHashcat.Areas.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (register.Password != register.ConfirmPassword) return View();

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("api/AuthenticationApi/RegisterAsync");

            if (response.IsSuccessStatusCode) return RedirectToAction("Index", "Profile");
            else return View();
        }
    }
}