using Microsoft.AspNetCore.Mvc;

namespace WebHashcatAdminPanel.Areas.AdminPanel.Controllers
{
    [Area("AdminPanel")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
