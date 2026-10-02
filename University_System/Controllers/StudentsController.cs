using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.ViewModels;

namespace University_System.Controllers
{
    // Works with the Trainee model; the name "Students" matches the sidebar link.
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var trainees = _context.Trainees
                .Include(t => t.Department)
                .OrderBy(t => t.Id)
                .ToList();

            return View(trainees);
        }

        public IActionResult Details(int id)
        {
            var trainee = _context.Trainees
                .Include(t => t.Department)
                .FirstOrDefault(t => t.Id == id);

            if (trainee == null) return NotFound();

            var results = _context.CourseResults
                .Include(r => r.Course)
                .Where(r => r.TraineeId == id)
                .ToList();

            return View(new TraineeDetailsViewModel
            {
                Trainee = trainee,
                Results = results
            });
        }
    }
}
