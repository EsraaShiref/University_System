using System.ComponentModel.DataAnnotations;

namespace University_System.ViewModels
{
    public class InstructorFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required, StringLength(100)]
        public string InsName { get; set; } = "";

        [Range(0, 1000000)]
        public decimal Salary { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Course")]
        [Required(ErrorMessage = "Please select a course.")]
        public int? CourseId { get; set; }

        [Display(Name = "Photo")]
        public IFormFile? ImageFile { get; set; }

        // Current image file name (kept when no new file is uploaded)
        public string? Img { get; set; }
    }
}
