using Microsoft.AspNetCore.Mvc;

namespace MaaltijdenApp_WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
