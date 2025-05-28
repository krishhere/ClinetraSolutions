using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinetraSolutions.Controllers
{
    public class CourseDetailsController : Controller
    {
        public IActionResult Index(string courseType, string course)
        {
            ViewData["courses"] = DbData.GetCourseDetails(courseType.Replace("_", " "), course.Replace("_", " "));
            return View();
        }
    }
}