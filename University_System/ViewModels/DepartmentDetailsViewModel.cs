using University_System.Models;

namespace University_System.ViewModels
{
    public class DepartmentDetailsViewModel
    {
        public Department Department { get; set; } = null!;
        public List<Instructor> Instructors { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
        public List<Trainee> Trainees { get; set; } = new();
    }
}
