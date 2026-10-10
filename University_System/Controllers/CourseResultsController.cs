using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using University_System.Data;
using University_System.Helpers;
using University_System.Models;
using University_System.ViewModels;

namespace University_System.Controllers
{
    public class CourseResultsController : Controller
    {
        private const int PageSize = 10;
        private readonly AppDbContext _context;

        public CourseResultsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /CourseResults?q=...&status=passed|failed&page=2
        public async Task<IActionResult> Index(string? q, string? status, int page = 1)
        {
            q = SearchHelper.Clean(q);
            var query = _context.CourseResults.AsNoTracking()
                .Include(r => r.Trainee)
                .Include(r => r.Course)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(r => r.Trainee.Name.Contains(q) || r.Course.Name.Contains(q));
            }

            if (status == "passed")
                query = query.Where(r => r.Degree >= r.Course.MinDegree);
            else if (status == "failed")
                query = query.Where(r => r.Degree < r.Course.MinDegree);

            ViewData["q"] = q;
            ViewData["status"] = status;

            return View(await PaginatedList<CourseResult>.CreateAsync(
                query.OrderBy(r => r.Trainee.Name).ThenBy(r => r.Course.Name).ThenBy(r => r.Id), page, PageSize));
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _context.CourseResults.AsNoTracking()
                .Include(r => r.Trainee).ThenInclude(t => t.Department)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null) return NotFound();
            return View(result);
        }

        public async Task<IActionResult> Create(int? traineeId, int? courseId)
        {
            await PopulateListsAsync(traineeId, courseId);
            return View("Form", new CourseResultFormViewModel { TraineeId = traineeId, CourseId = courseId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseResultFormViewModel vm)
        {
            await ValidateAsync(vm);
            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(vm.TraineeId, vm.CourseId);
                return View("Form", vm);
            }

            var entity = new CourseResult
            {
                TraineeId = vm.TraineeId!.Value,
                CourseId = vm.CourseId!.Value,
                Degree = vm.Degree!.Value
            };
            _context.CourseResults.Add(entity);
            if (!await TrySaveAsync())
            {
                await PopulateListsAsync(vm.TraineeId, vm.CourseId);
                return View("Form", vm);
            }

            TempData["Success"] = "Course result was created.";
            return RedirectToAction(nameof(Details), new { id = entity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var r = await _context.CourseResults.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound();

            await PopulateListsAsync(r.TraineeId, r.CourseId);
            return View("Form", new CourseResultFormViewModel
            {
                Id = r.Id,
                TraineeId = r.TraineeId,
                CourseId = r.CourseId,
                Degree = r.Degree
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseResultFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            await ValidateAsync(vm);
            if (!ModelState.IsValid)
            {
                await PopulateListsAsync(vm.TraineeId, vm.CourseId);
                return View("Form", vm);
            }

            var entity = await _context.CourseResults.FindAsync(id);
            if (entity == null) return NotFound();

            entity.TraineeId = vm.TraineeId!.Value;
            entity.CourseId = vm.CourseId!.Value;
            entity.Degree = vm.Degree!.Value;
            if (!await TrySaveAsync())
            {
                await PopulateListsAsync(vm.TraineeId, vm.CourseId);
                return View("Form", vm);
            }

            TempData["Success"] = "Course result was updated.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.CourseResults.FindAsync(id);
            if (entity == null) return NotFound();

            _context.CourseResults.Remove(entity);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "The record could not be deleted because other data still depends on it.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Course result was deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Remote validation: one result per trainee and course
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsPairAvailable(int? courseId, int? traineeId, int id)
        {
            if (!courseId.HasValue || !traineeId.HasValue) return Json(true);

            var exists = await _context.CourseResults.AnyAsync(r =>
                r.TraineeId == traineeId.Value && r.CourseId == courseId.Value && r.Id != id);
            return Json(exists ? "This trainee already has a result for this course." : true);
        }

        // Remote validation: the degree cannot exceed the course total
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsDegreeValid(int? degree, int? courseId)
        {
            if (!degree.HasValue || !courseId.HasValue) return Json(true);

            var total = await _context.Courses.AsNoTracking()
                .Where(c => c.Id == courseId.Value)
                .Select(c => (int?)c.Degree)
                .FirstOrDefaultAsync();

            return Json(total.HasValue && degree.Value > total.Value
                ? $"The degree cannot be greater than the course total ({total.Value})."
                : true);
        }

        private async Task ValidateAsync(CourseResultFormViewModel vm)
        {
            if (vm.TraineeId.HasValue && !await _context.Trainees.AnyAsync(t => t.Id == vm.TraineeId.Value))
                ModelState.AddModelError(nameof(vm.TraineeId), "The selected trainee does not exist.");

            if (!vm.CourseId.HasValue) return;

            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.Id == vm.CourseId.Value);
            if (course == null)
            {
                ModelState.AddModelError(nameof(vm.CourseId), "The selected course does not exist.");
                return;
            }

            if (vm.Degree > course.Degree)
                ModelState.AddModelError(nameof(vm.Degree), $"The degree cannot be greater than the course total ({course.Degree}).");

            if (vm.TraineeId.HasValue &&
                await _context.CourseResults.AnyAsync(r =>
                    r.TraineeId == vm.TraineeId.Value && r.CourseId == vm.CourseId.Value && r.Id != vm.Id))
            {
                ModelState.AddModelError(nameof(vm.CourseId), "This trainee already has a result for this course.");
            }
        }

        private async Task PopulateListsAsync(int? traineeId, int? courseId)
        {
            var trainees = await _context.Trainees.AsNoTracking()
                .OrderBy(t => t.Name)
                .Select(t => new { t.Id, Label = t.Name + " — " + t.Department.Name })
                .ToListAsync();
            var courses = await _context.Courses.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.Id, Label = c.Name + " — " + c.Department.Name })
                .ToListAsync();

            ViewData["Trainees"] = new SelectList(trainees, "Id", "Label", traineeId);
            ViewData["Courses"] = new SelectList(courses, "Id", "Label", courseId);
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
