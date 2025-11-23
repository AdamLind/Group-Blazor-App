using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/
        public IActionResult Index()
        {
            return View();
        }

        // Optionally, add actions for BookList and Reading if you want to serve those views directly
        public IActionResult BookList()
        {
            return View();
        }

        public IActionResult Reading()
        {
            return View();
        }
    }
}
