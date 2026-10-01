using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_System.Data;

namespace University_System.Controllers
{
    public class InstructorController : Controller
    {
        private readonly AppDbContext _context;

        public InstructorController(AppDbContext context) => _context = context;

        public IActionResult Index()
        {
            var instructors = _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .ToList();
            return View(instructors);
        }

        public IActionResult Details(int id)
        {
            var instructor = _context.Instructors
                .Include(i => i.Department)
                .Include(i => i.Course)
                .FirstOrDefault(i => i.Id == id);

            if (instructor == null) return NotFound();
            return View(instructor);
        }
    }
}
