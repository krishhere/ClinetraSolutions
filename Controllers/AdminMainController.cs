using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinetraSolutions.Controllers
{
    [Authorize]
    public class AdminMainController : Controller
    {
        public IActionResult Index()
        {
            ViewData["MedicalDeviceVigilance"] = DbData.GetData("MedicalDeviceVigilance");
            ViewData["IT"] = DbData.GetData("IT");
            ViewData["Pharma"] = DbData.GetData("pharma");
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddCourse(IFormFile courseImage,string courseType, string courseDetails, string courseName)
        {
            if (courseImage == null || string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(courseDetails))
            {
                return BadRequest("Invalid input data.");
            }

            bool res = DbData.PostCourse(courseImage, courseType, courseDetails, courseName);
            return Json(new { status = res });
        }
        

    }
}