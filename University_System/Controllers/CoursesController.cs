using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.Helpers;
using University_System.Models;
using University_System.ViewModels;

namespace University_System.Controllers
{
    public class CoursesController : Controller
    {
        private const int PageSize = 10;
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Courses?q=...&departmentId=...&page=2
        public async Task<IActionResult> Index(string? q, int? departmentId, int page = 1)
        {
            var query = _context.Courses.AsNoTracking()
                .Include(c => c.Department)
                .Include(c => c.Instructors)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(c => c.Name.Contains(q));
            }
            if (departmentId.HasValue)
                query = query.Where(c => c.DepartmentId == departmentId.Value);

            await PopulateDepartmentsAsync(departmentId);
            ViewData["q"] = q;
            ViewData["departmentId"] = departmentId;

            return View(await PaginatedList<Course>.CreateAsync(
                query.OrderBy(c => c.Name).ThenBy(c => c.Id), page, PageSize));
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses.AsNoTracking()
                .Include(c => c.Department)
                .Include(c => c.Instructors)
                .Include(c => c.courseResults).ThenInclude(r => r.Trainee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound();
            return View(course);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsAsync();
            return View("Form", new CourseFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel vm)
        {
            await ValidateAsync(vm);
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(vm.DepartmentId);
                return View("Form", vm);
            }

            var entity = new Course
            {
                Name = vm.Name.Trim(),
                Degree = vm.Degree,
                MinDegree = vm.MinDegree,
                DepartmentId = vm.DepartmentId!.Value
            };
            _context.Courses.Add(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course \"{entity.Name}\" was created.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (c == null) return NotFound();

            await PopulateDepartmentsAsync(c.DepartmentId);
            return View("Form", new CourseFormViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Degree = c.Degree,
                MinDegree = c.MinDegree,
                DepartmentId = c.DepartmentId
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            await ValidateAsync(vm);
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(vm.DepartmentId);
                return View("Form", vm);
            }

            var c = await _context.Courses.FindAsync(id);
            if (c == null) return NotFound();

            c.Name = vm.Name.Trim();
            c.Degree = vm.Degree;
            c.MinDegree = vm.MinDegree;
            c.DepartmentId = vm.DepartmentId!.Value;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course \"{c.Name}\" was updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _context.Courses.FindAsync(id);
            if (c == null) return NotFound();

            var inUse = await _context.Instructors.AnyAsync(i => i.CourseId == id)
                     || await _context.CourseResults.AnyAsync(r => r.CourseId == id);

            if (inUse)
            {
                TempData["Error"] = $"\"{c.Name}\" cannot be deleted because it still has instructors or trainee results.";
                return RedirectToAction(nameof(Index));
            }

            _context.Courses.Remove(c);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Course \"{c.Name}\" was deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateAsync(CourseFormViewModel vm)
        {
            if (vm.MinDegree > vm.Degree)
                ModelState.AddModelError(nameof(vm.MinDegree), "Minimum degree cannot be greater than the total degree.");

            if (vm.DepartmentId.HasValue && !await _context.Departments.AnyAsync(d => d.Id == vm.DepartmentId.Value))
                ModelState.AddModelError(nameof(vm.DepartmentId), "The selected department does not exist.");
        }

        private async Task PopulateDepartmentsAsync(int? selected = null)
        {
            var items = await _context.Departments.AsNoTracking()
                .OrderBy(d => d.Name)
                .Select(d => new { d.Id, d.Name })
                .ToListAsync();
            ViewData["Departments"] = new SelectList(items, "Id", "Name", selected);
        }
    }
}
