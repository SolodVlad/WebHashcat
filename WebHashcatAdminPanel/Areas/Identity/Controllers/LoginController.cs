using Microsoft.AspNetCore.Mvc;

namespace WebHashcatAdminPanel.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class LoginController : Controller
    {
        [HttpGet]
        [Route("Login")]
        public IActionResult Index() => View();
    }
}
