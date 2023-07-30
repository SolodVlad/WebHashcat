using Microsoft.AspNetCore.Mvc;

namespace WebHashcat.Areas.Identity.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
