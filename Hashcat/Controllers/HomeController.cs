using WebHashcat.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System;
using Domain.Models;
using WebHashcat.Models;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace WebHashcat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //var token = HttpContext.Request.Cookies["AuthCookie"];
            //if (!string.IsNullOrEmpty(token))
            //{
            //    var httpClient = _httpClientFactory.CreateClient();
            //    var jsonContent = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
            //    var response = await httpClient.PostAsync("https://localhost:7149/api/AuthenticationApi/ValidateJWTToken", jsonContent);
            //    if (response.IsSuccessStatusCode)
            //        if (await response.Content.ReadAsStringAsync() != null) ViewBag.email = await response.Content.ReadAsStringAsync();
            //        else Response.Cookies.Delete("AuthCookie");
            //}

            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> StartHashcat(HashcatArguments hashcatArguments)
        //{
        //    var token = Request.Cookies["AuthCookie"];

        //    var httpClient = _httpClientFactory.CreateClient();
        //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

        //    var jsonContent = new StringContent(JsonConvert.SerializeObject(hashcatArguments), Encoding.UTF8, "application/json");
        //    var response = await httpClient.PostAsync("https://localhost:7149/api/HashcatApi", jsonContent);

        //    if (response.IsSuccessStatusCode) return RedirectToAction("Index", "Profile");
        //    else return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult FAQ () 
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}