using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClinetraSolutions.Controllers
{
    public class AdminMainController : Controller
    {
        public IActionResult Index()
        {
            DataTable allCourses = DbData.GetAllCourses();
            if (allCourses != null && allCourses.Rows.Count > 0)
            {
                string[] categories = { "Medical Device", "Pharma", "IT" };
                Dictionary<string, DataTable> categoryTables = new Dictionary<string, DataTable>();
                foreach (string category in categories)
                {
                    DataView view = new DataView(allCourses);
                    view.RowFilter = $"Category = '{category.Replace("'", "''")}'";
                    DataTable filteredTable = view.ToTable();
                    categoryTables[category] = filteredTable;
                }
                DataTable medicalDeviceCourses = categoryTables["Medical Device"];
                DataTable pharmaCourses = categoryTables["Pharma"];
                DataTable itCourses = categoryTables["IT"];

                ViewData["MedicalDevice"] = medicalDeviceCourses;
                ViewData["Pharma"] = pharmaCourses;
                ViewData["IT"] = itCourses;
            }
            return View();
        }
        public IActionResult EnquiryDashboard()
        {
            ViewData["EnquiryData"] = DbData.GetEnquiryData();
            return View();
        }
        public IActionResult Students()
        {
            ViewData["StudentsData"] = DbData.GetStudentsData();
            return View();
        }
        public IActionResult NewBatch()
        {
            ViewData["AllCourses"] = DbData.GetAllCourses();
            ViewData["AllSeats"] = DbData.GetAllSeats();
            return View();
        }
        [HttpGet]
        public JsonResult GetCourses(string category)
        {
            DataTable dt = DbData.GetCourse(category);
            var list = dt.AsEnumerable().Select(row => new
            {
                id = row["Id"],
                name = row["CourseName"],
                price = row["Price"],
                seatId = row["SeatId"],
                batchStartDate = row["BatchStartDate"],
                availableSeats = row["AvailableSeats"]
            }).ToList();
            return Json(list);
        }
        [HttpPost]
        public async Task<IActionResult> AddCourse(IFormCollection formCollection)
        {
            string courseType = formCollection["courseType"];
            string courseName = formCollection["courseName"];
            string studentType = formCollection["studentType"].ToString().Equals("null") ? "" : formCollection["studentType"].ToString();
            string courseDetails = formCollection["courseDetails"];
            string coursePrice = formCollection["coursePrice"];
            IFormFile courseImage = formCollection.Files["courseImage"];
            if (courseImage == null || string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(courseDetails) || string.IsNullOrWhiteSpace(coursePrice))
            {
                return BadRequest("Invalid input data.");
            }
            bool res = DbData.PostCourse(courseImage, courseType, courseDetails.Replace("\n", ""), courseName, coursePrice, studentType);
            return Json(new { status = res });
        }
        [HttpPost]
        public JsonResult UpdatePrice(IFormCollection form)
        {
            try
            {
                var id = form["id"];
                var price = form["price"];
                var coursetype = form["coursetype"];
                bool isUpdated = DbData.UpdatePrice(id, price, coursetype);
                if (isUpdated)
                {
                    return Json(new { message = "Price updated successfully!" });
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
        [HttpPost]
        public JsonResult SetNewBatch(IFormCollection form)
        {
            try
            {
                string courseId = form["CourseId"];
                string batchStartDate = form["BatchDate"];
                string courseName = form["CourseName"];
                bool flag = DbData.SetNewBatch(courseId, batchStartDate);
                if (flag)
                {
                    return Json(new { message = $"New batch dates for '{courseName}' have been set successfully!" });
                }
                else
                {
                    return Json(new { message = $"Failed to set new batch dates for '{courseName}'." });
                }
            }
            catch
            {
                return Json(new { message = "An error occurred. Contact developer" });
            }
        }
        [HttpPost]
        public JsonResult UpdateBatch(IFormCollection form)
        {
            try
            {
                string seatId = form["SeatId"].ToString();
                string batchStartDate = form["BatchStartDate"].ToString();
                bool flag = DbData.UpdateBatch(seatId, batchStartDate);
                if (flag)
                {
                    return Json(new { message = " batch dates are updated successfully!" });
                }
                else
                {
                    return Json(new { message = " failed to update batch dates." });
                }
            }
            catch
            {
                return Json(new { message = "An error occurred. Contact developer" });
            }
        }
    }
}