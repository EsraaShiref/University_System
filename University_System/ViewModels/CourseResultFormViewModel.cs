using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace University_System.ViewModels
{
    public class CourseResultFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Trainee")]
        [Required(ErrorMessage = "Please select a trainee.")]
        public int? TraineeId { get; set; }

        [Display(Name = "Course")]
        [Required(ErrorMessage = "Please select a course.")]
        [Remote("IsPairAvailable", "CourseResults", AdditionalFields = nameof(TraineeId) + "," + nameof(Id))]
        public int? CourseId { get; set; }

        [Required(ErrorMessage = "The degree is required.")]
        [Range(0, 1000, ErrorMessage = "The degree must be between 0 and 1000.")]
        [Remote("IsDegreeValid", "CourseResults", AdditionalFields = nameof(CourseId))]
        public int? Degree { get; set; }
    }
}
