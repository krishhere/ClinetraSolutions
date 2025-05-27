using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinetraSolutions.Controllers
{
    public class CourseDetailsController : Controller
    {
        public IActionResult Index(string courseType, string course)
        {
            ViewData["courses"] = DbData.GetCourseDetails(courseType, course.Replace("-", " "));
            return View();
        }
    }
}