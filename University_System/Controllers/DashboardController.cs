using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.Models.ViewModels;

namespace University_System.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TotalTrainees = _context.Trainees.Count(),
                TotalCourses = _context.Courses.Count(),
                TotalInstructors = _context.Instructors.Count(),
                TotalDepartments = _context.Departments.Count(),

                Trainees = _context.Trainees
                    .Include(t => t.Department)
                    .OrderByDescending(t => t.Id)
                    .Take(5)
                    .ToList(),

                RecentCourses = _context.Courses
                    .Include(c => c.courseResults)
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .ToList()
            };

            return View(model);
        }
    }
}
