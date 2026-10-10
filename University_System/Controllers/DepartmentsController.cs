using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.Helpers;
using University_System.Models;
using University_System.ViewModels;

namespace University_System.Controllers
{
    public class DepartmentsController : Controller
    {
        private const int PageSize = 10;
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Departments?q=...&page=2
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            q = SearchHelper.Clean(q);
            var query = _context.Departments.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(d => d.Name.Contains(q) || d.Manager.Contains(q));
            }

            var projected = query
                .OrderBy(d => d.Name).ThenBy(d => d.Id)
                .Select(d => new DepartmentListItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Manager = d.Manager,
                    InstructorsCount = _context.Instructors.Count(i => i.DepartmentId == d.Id),
                    CoursesCount = _context.Courses.Count(c => c.DepartmentId == d.Id),
                    TraineesCount = _context.Trainees.Count(t => t.DepartmentId == d.Id)
                });

            ViewData["q"] = q;
            return View(await PaginatedList<DepartmentListItemViewModel>.CreateAsync(projected, page, PageSize));
        }

        public async Task<IActionResult> Details(int id)
        {
            var department = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            if (department == null) return NotFound();

            return View(new DepartmentDetailsViewModel
            {
                Department = department,
                Instructors = await _context.Instructors.AsNoTracking()
                    .Include(i => i.Course)
                    .Where(i => i.DepartmentId == id)
                    .OrderBy(i => i.InsName)
                    .ToListAsync(),
                Courses = await _context.Courses.AsNoTracking()
                    .Include(c => c.Instructors)
                    .Include(c => c.courseResults)
                    .Where(c => c.DepartmentId == id)
                    .OrderBy(c => c.Name)
                    .ToListAsync(),
                Trainees = await _context.Trainees.AsNoTracking()
                    .Where(t => t.DepartmentId == id)
                    .OrderBy(t => t.Name)
                    .ToListAsync()
            });
        }

        public IActionResult Create() => View("Form", new DepartmentFormViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentFormViewModel vm)
        {
            await ValidateAsync(vm);
            if (!ModelState.IsValid) return View("Form", vm);

            var entity = new Department { Name = vm.Name.Trim(), Manager = vm.Manager.Trim() };
            _context.Departments.Add(entity);
            if (!await TrySaveAsync()) return View("Form", vm);

            TempData["Success"] = $"Department \"{entity.Name}\" was created.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var d = await _context.Departments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (d == null) return NotFound();

            return View("Form", new DepartmentFormViewModel { Id = d.Id, Name = d.Name, Manager = d.Manager });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DepartmentFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();
            await ValidateAsync(vm);
            if (!ModelState.IsValid) return View("Form", vm);

            var d = await _context.Departments.FindAsync(id);
            if (d == null) return NotFound();

            d.Name = vm.Name.Trim();
            d.Manager = vm.Manager.Trim();
            if (!await TrySaveAsync()) return View("Form", vm);

            TempData["Success"] = $"Department \"{d.Name}\" was updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var d = await _context.Departments.FindAsync(id);
            if (d == null) return NotFound();

            var inUse = await _context.Instructors.AnyAsync(i => i.DepartmentId == id)
                     || await _context.Courses.AnyAsync(c => c.DepartmentId == id)
                     || await _context.Trainees.AnyAsync(t => t.DepartmentId == id);

            if (inUse)
            {
                TempData["Error"] = $"\"{d.Name}\" cannot be deleted because it still has instructors, courses or students.";
                return RedirectToAction(nameof(Index));
            }

            _context.Departments.Remove(d);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "The record could not be deleted because other data still depends on it.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = $"Department \"{d.Name}\" was deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Remote validation: called by the browser while the user types (see [Remote] on the view model)
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsNameAvailable(string? name, int id)
        {
            name = name?.Trim() ?? "";
            var taken = await _context.Departments.AnyAsync(d => d.Name == name && d.Id != id);
            return Json(taken ? $"A department named \"{name}\" already exists." : true);
        }

        // Server-side rules that cannot be expressed as attributes
        private async Task ValidateAsync(DepartmentFormViewModel vm)
        {
            var name = vm.Name?.Trim() ?? "";
            if (await _context.Departments.AnyAsync(d => d.Name == name && d.Id != vm.Id))
                ModelState.AddModelError(nameof(vm.Name), $"A department named \"{name}\" already exists.");
        }

        // Last line of defence: a unique index or check constraint rejected the data
        private async Task<bool> TrySaveAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "The record could not be saved. A record with the same values may already exist, or a value is out of range.");
                return false;
            }
        }
    }
}
