using System.ComponentModel.DataAnnotations;

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
        public int? CourseId { get; set; }

        [Range(0, 1000)]
        public int Degree { get; set; }
    }
}
