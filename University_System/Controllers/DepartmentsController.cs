using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.ViewModels;

namespace University_System.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var departments = _context.Departments
                .OrderBy(d => d.Id)
                .Select(d => new DepartmentListItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manager = d.Manager,
                    InstructorsCount = _context.Instructors.Count(i => i.DepartmentId == d.Id),
                    CoursesCount = _context.Courses.Count(c => c.DepartmentId == d.Id),
                    TraineesCount = _context.Trainees.Count(t => t.DepartmentId == d.Id)
                })
                .ToList();

            return View(departments);
        }

        public IActionResult Details(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.Id == id);
            if (department == null) return NotFound();

            return View(new DepartmentDetailsViewModel
            {
                Department = department,
                Instructors = _context.Instructors
                    .Include(i => i.Course)
                    .Where(i => i.DepartmentId == id)
                    .OrderBy(i => i.InsName)
                    .ToList(),
                Courses = _context.Courses
                    .Include(c => c.Instructors)
                    .Include(c => c.courseResults)
                    .Where(c => c.DepartmentId == id)
                    .OrderBy(c => c.Name)
                    .ToList(),
                Trainees = _context.Trainees
                    .Where(t => t.DepartmentId == id)
                    .OrderBy(t => t.Name)
                    .ToList()
            });
        }
    }
}
