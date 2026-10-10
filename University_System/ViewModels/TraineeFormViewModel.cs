using System.ComponentModel.DataAnnotations;
using University_System.Validation;

namespace University_System.ViewModels
{
    public class TraineeFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The student name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 100 characters.")]
        [RegularExpression(ValidationPatterns.PersonName, ErrorMessage = "The name can contain letters, spaces, dots, apostrophes and hyphens only.")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "The address is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "The address must be between 3 and 200 characters.")]
        [RegularExpression(ValidationPatterns.Address, ErrorMessage = "The address contains characters that are not allowed.")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "The grade is required.")]
        [Range(0, 100, ErrorMessage = "The grade must be between 0 and 100.")]
        public int? Grade { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select a department.")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Photo")]
        [AllowedExtensions(".jpg", ".jpeg", ".png", ".webp", ".gif")]
        [MaxFileSize(2)]
        public IFormFile? ImageFile { get; set; }

        // Current image file name (kept when no new file is uploaded)
        public string? Image { get; set; }
    }
}
