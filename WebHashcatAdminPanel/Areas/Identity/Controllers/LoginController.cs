using Microsoft.AspNetCore.Mvc;

namespace WebHashcatAdminPanel.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
