using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinetraSolutions.Controllers
{
    public class ITController : Controller
    {
        public IActionResult Index()
        {
            ViewData["IT"] = DbData.GetCourse("IT");
            return View();
        }
    }
}