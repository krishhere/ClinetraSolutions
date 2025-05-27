using ClinetraSolutions.Models;
using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClinetraSolutions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PostEnquiry(IFormCollection form)
        {
            string name = form["name"].ToString().Trim();
            string mobile = form["mobile"].ToString().Trim();
            string qualification = form["qualification"].ToString().Trim();
            string comments = form["comments"].ToString().Trim();
            string query = $"INSERT INTO enquiryform(Name, Mobile, Qualification, Comments) VALUES('{name}','{mobile}','{qualification}','{comments}')";
            bool res = DbData.PostData(query);
            return Json(new { status = res });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
