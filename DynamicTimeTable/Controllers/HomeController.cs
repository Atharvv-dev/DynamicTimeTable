using Microsoft.AspNetCore.Mvc;
using DynamicTimeTable.Models;
using DynamicTimeTable.Services;
using System.Text.Json;

namespace DynamicTimeTable.Controllers
{
    public class HomeController : Controller
    {
        // GET: Show initial form for basic input
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Process initial input and redirect to subject hours
        [HttpPost]
        public IActionResult Index(TimetableInputModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Store values temporarily for next step
            TempData["WorkingDays"] = model.WorkingDays;
            TempData["SubjectsPerDay"] = model.SubjectsPerDay;
            TempData["TotalSubjects"] = model.TotalSubjects;
            TempData["TotalHours"] = model.TotalHours;

            return RedirectToAction("SubjectHours");
        }

        // GET: Show form to enter subject names and their hours
        [HttpGet]
        public IActionResult SubjectHours()
        {
            // Retrieve data from TempData
            if (!TempData.TryGetValue("TotalSubjects", out var totalSubjectsObj) ||
                !TempData.TryGetValue("TotalHours", out var totalHoursObj))
            {
                return RedirectToAction("Index"); // fallback if data is missing
            }

            TempData.Keep(); // Retain for POST

            int totalSubjects = (int)totalSubjectsObj;
            int totalHours = (int)totalHoursObj;

            var viewModel = new SubjectHoursViewModel
            {
                TotalSubjects = totalSubjects,
                TotalHours = totalHours
            };

            // Prepare empty subject inputs
            for (int i = 0; i < totalSubjects; i++)
            {
                viewModel.SubjectHours.Add(new SubjectHourModel());
            }

            return View(viewModel);
        }

        // POST: Process subject hours input and validate total hours
        [HttpPost]
        public IActionResult SubjectHours(SubjectHoursViewModel model)
        {
            int enteredHours = model.SubjectHours.Sum(s => s.Hours);

            if (enteredHours != model.TotalHours)
            {
                ModelState.AddModelError("", $"Total entered hours ({enteredHours}) must equal {model.TotalHours}.");
                return View(model);
            }

            // Store subject hour data for final generation
            TempData["SubjectHours"] = JsonSerializer.Serialize(model.SubjectHours);

            return RedirectToAction("Generate");
        }

        // GET: Final step – generate and show timetable
        public IActionResult Generate()
        {
            if (!TryGetTempData("WorkingDays", out int workingDays) ||
                !TryGetTempData("SubjectsPerDay", out int subjectsPerDay) ||
                !TempData.TryGetValue("SubjectHours", out var jsonData))
            {
                return RedirectToAction("Index"); // fallback
            }

            var subjectHours = JsonSerializer.Deserialize<List<SubjectHourModel>>(jsonData.ToString());

            var generator = new TimetableGeneratorService();
            var timetable = generator.GenerateGrid(subjectHours, subjectsPerDay, workingDays);

            var model = new FinalTimetableViewModel
            {
                TimetableGrid = timetable,
                Rows = subjectsPerDay,
                Columns = workingDays
            };

            return View(model);
        }

        // Helper method to retrieve and cast TempData safely
        private bool TryGetTempData(string key, out int value)
        {
            if (TempData.TryGetValue(key, out var obj) && obj is int intValue)
            {
                value = intValue;
                return true;
            }

            value = 0;
            return false;
        }
    }
}
