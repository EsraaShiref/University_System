using System.ComponentModel.DataAnnotations;

namespace University_System.ViewModels
{
    public class TraineeFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Required, StringLength(200)]
        public string Address { get; set; } = "";

        [Range(0, 100)]
        public int Grade { get; set; }

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select a department.")]
        public int? DepartmentId { get; set; }

        [Display(Name = "Photo")]
        public IFormFile? ImageFile { get; set; }

        // Current image file name (kept when no new file is uploaded)
        public string? Image { get; set; }
    }
}
