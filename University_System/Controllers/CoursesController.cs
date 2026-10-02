using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_System.Data;

namespace University_System.Controllers
{
    public class CoursesController : Controller
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context) => _context = context;
        public IActionResult Index()
        {
            var courses = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructors)
                .OrderBy(c => c.Id)
                .ToList();

            return View(courses);
        }

        public IActionResult Details(int id)
        {
            var course = _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Instructors)
                .Include(c => c.courseResults)
                    .ThenInclude(r => r.Trainee)
                .FirstOrDefault(c => c.Id == id);

            if (course == null) return NotFound();

            return View(course);
        }
    }
}
