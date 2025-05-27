using ClinetraSolutions.Models;
using ClinetraSolutions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClinetraSolutions.Controllers
{
    public class PharmaController : Controller
    {
        public IActionResult Index()
        {
            string studentType = "B Pharmacy";
            DataTable dt = DbData.GetPharmaCourse(studentType);
            var filteredRows = dt.AsEnumerable().Where(row => row["StudentType"].ToString().Equals(studentType, StringComparison.OrdinalIgnoreCase));
            DataTable filteredDt = dt.Clone();
            foreach (var row in filteredRows)
            {
                filteredDt.ImportRow(row);
            }

            var model = new PharmaModel
            {
                PharmaData = filteredDt
            };
            return View(model);
        }
        public IActionResult FilterPharma(string studentType)
        {
            DataTable dt = DbData.GetPharmaCourse(studentType);
            var filteredRows = dt.AsEnumerable().Where(row => row["StudentType"].ToString().Equals(studentType, StringComparison.OrdinalIgnoreCase));
            DataTable filteredDt = dt.Clone();
            foreach (var row in filteredRows)
            {
                filteredDt.ImportRow(row);
            }

            var model = new PharmaModel
            {
                PharmaData = filteredDt
            };

            return PartialView("PharmaPartial", model);
        }
    }
}