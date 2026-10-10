using System.ComponentModel.DataAnnotations;
using University_System.Validation;

namespace University_System.ViewModels
{
    public class InstructorFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "The instructor name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 100 characters.")]
        [RegularExpression(ValidationPatterns.PersonName, ErrorMessage = "The name can contain letters, spaces, dots, apostrophes and hyphens only.")]
        public string InsName { get; set; } = "";

        [Required(ErrorMessage = "The salary is required.")]
        [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "The salary must be between 0.01 and 1,000,000.")]
        [RegularExpression(ValidationPatterns.Money, ErrorMessage = "Enter a valid amount with at most 2 decimal places.")]
        public decimal? Salary { get; set; }

        [StringLength(200, ErrorMessage = "The address cannot exceed 200 characters.")]
        [RegularExpression(ValidationPatterns.Address, ErrorMessage = "The address contains characters that are not allowed.")]
        public string? Address { get; set; }

        [Display(Name = "Course")]
        [Required(ErrorMessage = "Please select a course.")]
        public int? CourseId { get; set; }

        [Display(Name = "Photo")]
        [AllowedExtensions(".jpg", ".jpeg", ".png", ".webp", ".gif")]
        [MaxFileSize(2)]
        public IFormFile? ImageFile { get; set; }

        // Current image file name (kept when no new file is uploaded)
        public string? Img { get; set; }
    }
}
