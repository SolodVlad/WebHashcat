using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Text;
using WebHashcat.Areas.Identity.Models;
using WebHashcat.Models;

namespace WebHashcat.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (registerViewModel.Password != registerViewModel.ConfirmPassword) return Json("Password != confirm password");

            var register = new Register() { Email = registerViewModel.Email, Login = registerViewModel.Email, Password = registerViewModel.Password };

            var httpClient = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(register), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://localhost:7149/api/AuthenticationApi/Register", jsonContent);

            if (response.IsSuccessStatusCode) return Json("Підтвердіть свою пошту");
            else return Json("API error");
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login() => View();

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync(RegisterViewModel loginViewModel)
        {
            var login = new Login() { Login_ = loginViewModel.Email, Password = loginViewModel.Password };

            var httpClient = _httpClientFactory.CreateClient();
            var jsonContent = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://localhost:7149/api/AuthenticationApi/Login", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<dynamic>(responseData).token.Value;

                // Создание объекта CookieOptions для настройки куки
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // Токен доступен только для сервера
                    Secure = true, // Кука будет передаваться только по HTTPS
                    SameSite = SameSiteMode.Strict, // Запрещает куку быть отправленной на другой домен
                    Expires = DateTime.Now.AddDays(1) // Устанавливаем срок действия куки
                };

                // Установка значения токена в куку
                Response.Cookies.Append("AuthCookie", token, cookieOptions);

                return RedirectToAction("Index", "Home", new { Area = "Cabinet" });
            } return Json("API error");
        }
    }
}