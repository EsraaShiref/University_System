namespace University_System.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTrainees { get; set; }
        public int TotalCourses { get; set; }
        public int TotalInstructors { get; set; }
        public int TotalDepartments { get; set; }
        public List<Trainee> Trainees { get; set; } = new();
        public List<Course> RecentCourses { get; set; } = new();
    }
}