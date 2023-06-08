using Domain.Models;
using WebHashcat.Services;
using WebHashcat.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebHashcat.Controllers
{
    public class LookupTableController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly CheckHashTypeService _checkHashTypeService;

        public LookupTableController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7149/api/") };
            _checkHashTypeService = new CheckHashTypeService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SearchPasswords(string hashesStr)
        {
            var hashesArr = hashesStr.Split(Environment.NewLine).ToList();

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("LookupTableApi/SearchPasswords", hashesArr);

            if (response.IsSuccessStatusCode)
            {
                // Обработка успешного ответа
                var responseData = await response.Content.ReadAsStringAsync();
                for (int i = 0; i < hashesArr.Count; i++)
                {
                    var hash = hashesArr[i];
                    var hashType = _checkHashTypeService.GetHashType(hash);
                    var dataLookupTable = new DataLookupTableViewModel() { Hash = hash, HashType = hashType };
                }
                // Дальнейшая обработка полученных данных
                return RedirectToAction("Index", "Home", new { Area = "" });
            }
            else
            {
                return View();
                // Обработка ошибки
                // Можно вернуть соответствующий HTTP-статус или другую информацию о неудачном запросе
            }  
        }
    }
}
