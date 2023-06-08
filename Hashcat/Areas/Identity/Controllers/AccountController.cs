using WebHashcat.Areas.Identity.Models;
using Microsoft.AspNetCore.Mvc;
using Domain.Models;

namespace WebHashcat.Areas.Identity.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController() => _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7149/api/") };

        [HttpGet]
        public IActionResult Register() => View();

        //[HttpPost]
        //public async Task<IActionResult> RegisterAsync([FromBody] RegisterViewModel model)
        //{
        //    var user = new User() { Email = model.Email, UserName = model.Email, Password = model.Password };

        //    HttpResponseMessage response = await _httpClient.PostAsJsonAsync("AuthentificationApi/RegisterUser", user);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        // Обработка успешного ответа
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        // Дальнейшая обработка полученных данных
        //        return RedirectToAction("Index", "Home", new { Area = "" });
        //    }
        //    else
        //    {
        //        return View();
        //        // Обработка ошибки
        //        // Можно вернуть соответствующий HTTP-статус или другую информацию о неудачном запросе
        //    }
        //}
    }
}
