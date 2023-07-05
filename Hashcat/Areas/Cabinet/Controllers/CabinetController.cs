using Microsoft.AspNetCore.Mvc;

namespace WebHashcat.Areas.Cabinet.Controllers
{
    public class CabinetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
