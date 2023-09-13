using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Security.Claims;
using System;
using System.Text;
using WebHashcat.Areas.Identity.Models;
using WebHashcat.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using WebHashcat.Areas.Identity.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace WebHashcat.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<User> userManager, IConfiguration config, IDistributedCache cache)
        {
            _userManager = userManager;
            _tokenService = new TokenService(config, cache);
        }

        //private readonly IHttpClientFactory _httpClientFactory;

        //public AccountController(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        //[HttpPost]
        //[Route("Register")]
        //public async Task<IActionResult> RegisterAsync(RegisterViewModel registerViewModel)
        //{
        //    if (registerViewModel.Value != registerViewModel.ConfirmValue) return Json("Value != confirm Value");

        //    var register = new Register() { Email = registerViewModel.Email, Login = registerViewModel.Email, Value = registerViewModel.Value };

        //    var httpClient = _httpClientFactory.CreateClient();
        //    var jsonContent = new StringContent(JsonConvert.SerializeObject(register), Encoding.UTF8, "application/json");
        //    var response = await httpClient.PostAsync("https://localhost:7149/api/AuthenticationApi/Register", jsonContent);

        //    if (response.IsSuccessStatusCode) return Json("Підтвердіть свою пошту");
        //    else return Json("API error");
        //}

        [HttpGet]
        [Route("Login")]
        public IActionResult Login() => View();

        [HttpGet]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string userEmail, string? token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest("Invalid link");
            return View(new ResetPassword { Token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)), Email = userEmail });
        }

        [Route("EmailConfirm")]
        public async Task<IActionResult> EmailConfirmAsync(string guid, string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null) return BadRequest("USER NOT FOUND");
            var res = await _userManager.ConfirmEmailAsync(user, guid);
            if (res.Succeeded) return View();

            return BadRequest("INVALID TOKEN");
        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPassword resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null) return BadRequest("This user is not exist");

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.NewPassword);

            if (result.Succeeded)
            {
                var userNameHash = await ComputeSha512Async(Encoding.UTF8.GetBytes(user.UserName));
                await _tokenService.IsRevokeRefreshTokenSuccessAsync(userNameHash);
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        private static async Task<string> ComputeSha512Async(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var hashBytes = await SHA512.Create().ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "");
        }

        //[HttpPost]
        //[Route("Login")]
        //public async Task<IActionResult> LoginAsync(RegisterViewModel loginViewModel)
        //{
        //    var login = new Login() { Login_ = loginViewModel.Email, Value = loginViewModel.Value };

        //    var httpClient = _httpClientFactory.CreateClient();
        //    var jsonContent = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
        //    var response = await httpClient.PostAsync("https://localhost:7149/api/AuthenticationApi/Login", jsonContent);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        var responseData = await response.Content.ReadAsStringAsync();
        //        var token = JsonConvert.DeserializeObject<dynamic>(responseData).token.Value;

        //        var cookieOptions = new CookieOptions
        //        {
        //            HttpOnly = true,
        //            Secure = true,
        //            SameSite = SameSiteMode.Strict,
        //            Expires = DateTime.Now.AddDays(1),
        //            IsEssential = true
        //        };

        //        HttpContext.Response.Cookies.Append("AuthCookie", token, cookieOptions);

        //        return RedirectToAction("Index", "Home", new { Area = "Cabinet" });
        //    } return Json("API error");
        //}
    }
}