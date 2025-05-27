using ClinetraSolutions.Models;
using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClinetraSolutions.Controllers
{
    public class StudentEnrollController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string username, string password)
        {
            if (username == "admin" && password == "Clinetra")
            {
                HttpContext.Session.SetString("isUser", "true");
            }
            else
            {
                ViewBag.Error = "Invalid Credentials";
            }
            return View();
        }
        [HttpPost]
        public async Task<JsonResult> EnrollAStudent([FromBody] Dictionary<string, object> data)
        {
            try
            {
                string name = data["name"].ToString();
                string id = data["id"].ToString();
                string category = data["category"].ToString();
                string courses = data["courses"].ToString();
                string duration = data["duration"].ToString();
                string fee = data["fee"].ToString();
                string discount = data["discount"].ToString();
                string paid = data["paid"].ToString();
                string remaining = data["remaining"].ToString();
                bool isUpdated = DbData.EnrollAStudent(id, name, category, courses, duration, fee, discount, paid, remaining);
                if (isUpdated)
                {
                    return Json(new { message = "Price updated successfully." });
                }
                else
                {
                    return Json(new { message = "Failed to update price." });
                }
            }
            catch
            {
                return Json(new { message = "An error occurred. Contact developer" });
            }
        }
        [HttpGet]
        public IActionResult CourseAvailability(string seatId)
        {
            try
            {
                DataTable dtCourseAvail = DbData.GetACourseAvail(seatId);
                var model = new CourseAvailability
                {
                    dtCourseAvailability = dtCourseAvail
                };
                return PartialView("CourseAvailabilityPartial", model);
            }
            catch
            {
                return Json(new { message = "An error occurred. Contact developer" });
            }
        }
        [HttpPost]
        public IActionResult SeatAvail(string seatId)
        {
            bool isUpdated = DbData.UpdateCourseSeat(seatId);
            if (isUpdated)
            {
                return Json(new { message = "Course is availed." });
            }
            else
            {
                return Json(new { message = "Course is not availed." });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return Ok();
        }
    }
}