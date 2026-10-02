using University_System.Models;

namespace University_System.ViewModels
{
    public class TraineeDetailsViewModel
    {
        public Trainee Trainee { get; set; } = null!;
        public List<CourseResult> Results { get; set; } = new();
    }
}
