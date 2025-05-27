using Microsoft.AspNetCore.Mvc;

namespace ClinetraSolutions.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}