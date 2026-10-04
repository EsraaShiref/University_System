using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.Helpers;
using University_System.Models;
using University_System.ViewModels;

namespace University_System.Controllers
{
    public class InstructorController : Controller
    {
        private const int PageSize = 10;
        private const string ImageFolder = "instructors";

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public InstructorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Instructor?q=...&departmentId=...&page=2
        public async Task<IActionResult> Index(string? q, int? departmentId, int page = 1)
        {
            var query = _context.Instructors.AsNoTracking()
                .Include(i => i.Department)
                .Include(i => i.Course)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(i => i.InsName.Contains(q) || (i.Address != null && i.Address.Contains(q)));
            }
            if (departmentId.HasValue)
                query = query.Where(i => i.DepartmentId == departmentId.Value);

            await PopulateDepartmentsAsync(departmentId);
            ViewData["q"] = q;
            ViewData["departmentId"] = departmentId;

            return View(await PaginatedList<Instructor>.CreateAsync(
                query.OrderBy(i => i.InsName).ThenBy(i => i.Id), page, PageSize));
        }

        public async Task<IActionResult> Details(int id)
        {
            var instructor = await _context.Instructors.AsNoTracking()
                .Include(i => i.Department)
                .Include(i => i.Course)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instructor == null) return NotFound();
            return View(instructor);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateCoursesAsync();
            return View("Form", new InstructorFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorFormViewModel vm)
        {
            var course = await ValidateAsync(vm);
            if (!ModelState.IsValid)
            {
                await PopulateCoursesAsync(vm.CourseId);
                return View("Form", vm);
            }

            var entity = new Instructor
            {
                InsName = vm.InsName.Trim(),
                Salary = vm.Salary,
                Address = string.IsNullOrWhiteSpace(vm.Address) ? null : vm.Address.Trim(),
                CourseId = course!.Id,
                DepartmentId = course.DepartmentId   // an instructor belongs to the department of their course
            };
            if (vm.ImageFile is { Length: > 0 })
                entity.Img = await ImageHelper.SaveAsync(vm.ImageFile, _env, ImageFolder);

            _context.Instructors.Add(entity);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Instructor \"{entity.InsName}\" was created.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var i = await _context.Instructors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (i == null) return NotFound();

            await PopulateCoursesAsync(i.CourseId);
            return View("Form", new InstructorFormViewModel
            {
                Id = i.Id,
                InsName = i.InsName,
                Salary = i.Salary,
                Address = i.Address,
                CourseId = i.CourseId,
                Img = i.Img
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstructorFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            var course = await ValidateAsync(vm);
            var entity = await _context.Instructors.FindAsync(id);
            if (entity == null) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.Img = entity.Img;
                await PopulateCoursesAsync(vm.CourseId);
                return View("Form", vm);
            }

            var oldImage = entity.Img;
            entity.InsName = vm.InsName.Trim();
            entity.Salary = vm.Salary;
            entity.Address = string.IsNullOrWhiteSpace(vm.Address) ? null : vm.Address.Trim();
            entity.CourseId = course!.Id;
            entity.DepartmentId = course.DepartmentId;

            var replaced = false;
            if (vm.ImageFile is { Length: > 0 })
            {
                entity.Img = await ImageHelper.SaveAsync(vm.ImageFile, _env, ImageFolder);
                replaced = true;
            }

            await _context.SaveChangesAsync();
            if (replaced) ImageHelper.DeleteUploaded(_env, ImageFolder, oldImage);

            TempData["Success"] = $"Instructor \"{entity.InsName}\" was updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Instructors.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Instructors.Remove(entity);
            await _context.SaveChangesAsync();
            ImageHelper.DeleteUploaded(_env, ImageFolder, entity.Img);

            TempData["Success"] = $"Instructor \"{entity.InsName}\" was deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<Course?> ValidateAsync(InstructorFormViewModel vm)
        {
            Course? course = null;
            if (vm.CourseId.HasValue)
            {
                course = await _context.Courses.FindAsync(vm.CourseId.Value);
                if (course == null)
                    ModelState.AddModelError(nameof(vm.CourseId), "The selected course does not exist.");
            }

            if (vm.ImageFile is { Length: > 0 })
            {
                var error = ImageHelper.Validate(vm.ImageFile);
                if (error != null) ModelState.AddModelError(nameof(vm.ImageFile), error);
            }
            return course;
        }

        private async Task PopulateCoursesAsync(int? selected = null)
        {
            var items = await _context.Courses.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, Label = c.Name + " — " + c.Department.Name })
                .ToListAsync();
            ViewData["Courses"] = new SelectList(items, "Id", "Label", selected);
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
