using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinetraSolutions.Controllers
{
    public class MedicalDeviceController : Controller
    {
        public IActionResult Index()
        {
            ViewData["MedicalDevice"] = DbData.GetACourse("Medical Device");
            return View();
        }
    }
}