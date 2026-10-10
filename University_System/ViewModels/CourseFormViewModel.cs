using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using University_System.Validation;

namespace University_System.ViewModels
{
    public class CourseFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 100 characters.")]
        [RegularExpression(ValidationPatterns.Title, ErrorMessage = "The name can contain letters, numbers, spaces and . , & ' ( ) - : / + # only.")]
        [Remote("IsNameAvailable", "Courses", AdditionalFields = nameof(DepartmentId) + "," + nameof(Id))]
        public string Name { get; set; } = "";

        [Display(Name = "Total Degree")]
        [Required(ErrorMessage = "The total degree is required.")]
        [Range(1, 1000, ErrorMessage = "The total degree must be between 1 and 1000.")]
        public int? Degree { get; set; } = 100;

        [Display(Name = "Minimum Degree")]
        [Required(ErrorMessage = "The minimum degree is required.")]
        [Range(0, 1000, ErrorMessage = "The minimum degree must be between 0 and 1000.")]
        [LessOrEqualToProperty(nameof(Degree), ErrorMessage = "The minimum degree cannot be greater than the total degree.")]
        public int? MinDegree { get; set; } = 50;

        [Display(Name = "Department")]
        [Required(ErrorMessage = "Please select a department.")]
        public int? DepartmentId { get; set; }
    }
}
