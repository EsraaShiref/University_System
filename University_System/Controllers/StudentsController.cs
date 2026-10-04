using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.Helpers;
using University_System.Models;
using University_System.ViewModels;

namespace University_System.Controllers
{
    // Works with the Trainee model; the name "Students" matches the sidebar link.
    public class StudentsController : Controller
    {
        private const int PageSize = 10;
        private const string ImageFolder = "trainees";

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Students?q=...&departmentId=...&page=2
        public async Task<IActionResult> Index(string? q, int? departmentId, int page = 1)
        {
            var query = _context.Trainees.AsNoTracking()
                .Include(t => t.Department)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(t => t.Name.Contains(q) || t.Address.Contains(q));
            }
            if (departmentId.HasValue)
                query = query.Where(t => t.DepartmentId == departmentId.Value);

            await PopulateDepartmentsAsync(departmentId);
            ViewData["q"] = q;
            ViewData["departmentId"] = departmentId;

            return View(await PaginatedList<Trainee>.CreateAsync(
                query.OrderBy(t => t.Name).ThenBy(t => t.Id), page, PageSize));
        }

        public async Task<IActionResult> Details(int id)
        {
            var trainee = await _context.Trainees.AsNoTracking()
                .Include(t => t.Department)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trainee == null) return NotFound();

            var results = await _context.CourseResults.AsNoTracking()
                .Include(r => r.Course)
                .Where(r => r.TraineeId == id)
                .OrderBy(r => r.Course.Name)
                .ToListAsync();

            return View(new TraineeDetailsViewModel { Trainee = trainee, Results = results });
        }

        public async Task<IActionResult> Create()
        {
            await PopulateDepartmentsAsync();
            return View("Form", new TraineeFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TraineeFormViewModel vm)
        {
            await ValidateAsync(vm);
            if (!ModelState.IsValid)
            {
                await PopulateDepartmentsAsync(vm.DepartmentId);
                return View("Form", vm);
            }

            var entity = new Trainee
            {
                Name = vm.Name.Trim(),
                Address = vm.Address.Trim(),
                Grade = vm.Grade,
                DepartmentId = vm.DepartmentId!.Value
            };
            if (vm.ImageFile is { Length: > 0 })
                entity.Image = await ImageHelper.SaveAsync(vm.ImageFile, _env, ImageFolder);

            _context.Trainees.Add(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Student \"{entity.Name}\" was created.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var t = await _context.Trainees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound();

            await PopulateDepartmentsAsync(t.DepartmentId);
            return View("Form", new TraineeFormViewModel
            {
                Id = t.Id,
                Name = t.Name,
                Address = t.Address,
                Grade = t.Grade,
                DepartmentId = t.DepartmentId,
                Image = t.Image
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TraineeFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            await ValidateAsync(vm);
            var entity = await _context.Trainees.FindAsync(id);
            if (entity == null) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.Image = entity.Image;
                await PopulateDepartmentsAsync(vm.DepartmentId);
                return View("Form", vm);
            }

            var oldImage = entity.Image;
            entity.Name = vm.Name.Trim();
            entity.Address = vm.Address.Trim();
            entity.Grade = vm.Grade;
            entity.DepartmentId = vm.DepartmentId!.Value;

            var replaced = false;
            if (vm.ImageFile is { Length: > 0 })
            {
                entity.Image = await ImageHelper.SaveAsync(vm.ImageFile, _env, ImageFolder);
                replaced = true;
            }

            await _context.SaveChangesAsync();
            if (replaced) ImageHelper.DeleteUploaded(_env, ImageFolder, oldImage);

            TempData["Success"] = $"Student \"{entity.Name}\" was updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Trainees.FindAsync(id);
            if (entity == null) return NotFound();

            if (await _context.CourseResults.AnyAsync(r => r.TraineeId == id))
            {
                TempData["Error"] = $"\"{entity.Name}\" cannot be deleted because the student still has course results. Delete those results first.";
                return RedirectToAction(nameof(Index));
            }

            _context.Trainees.Remove(entity);
            await _context.SaveChangesAsync();
            ImageHelper.DeleteUploaded(_env, ImageFolder, entity.Image);

            TempData["Success"] = $"Student \"{entity.Name}\" was deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task ValidateAsync(TraineeFormViewModel vm)
        {
            if (vm.DepartmentId.HasValue && !await _context.Departments.AnyAsync(d => d.Id == vm.DepartmentId.Value))
                ModelState.AddModelError(nameof(vm.DepartmentId), "The selected department does not exist.");

            if (vm.ImageFile is { Length: > 0 })
            {
                var error = ImageHelper.Validate(vm.ImageFile);
                if (error != null) ModelState.AddModelError(nameof(vm.ImageFile), error);
            }
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
