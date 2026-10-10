using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using University_System.Validation;

namespace University_System.ViewModels
{
    public class DepartmentFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The name must be between 2 and 100 characters.")]
        [RegularExpression(ValidationPatterns.Title, ErrorMessage = "The name can contain letters, numbers, spaces and . , & ' ( ) - : / + # only.")]
        [Remote("IsNameAvailable", "Departments", AdditionalFields = nameof(Id))]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Manager name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The manager name must be between 2 and 100 characters.")]
        [RegularExpression(ValidationPatterns.PersonName, ErrorMessage = "The manager name can contain letters, spaces, dots, apostrophes and hyphens only.")]
        public string Manager { get; set; } = "";
    }
}
