using Microsoft.AspNetCore.Mvc;

namespace TodorovNet.Controllers
{
    public class LiveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
