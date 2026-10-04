using System.ComponentModel.DataAnnotations;

namespace University_System.ViewModels
{
    public class CourseFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Display(Name = "Total Degree")]
        [Range(1, 1000)]
        public int Degree { get; set; } = 100;

        [Display(Name = "Minimum Degree")]
        [Range(0, 1000)]
        public int MinDegree { get; set; } = 50;

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select a department.")]
        public int? DepartmentId { get; set; }
    }
}
