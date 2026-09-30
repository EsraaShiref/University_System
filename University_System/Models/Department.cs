namespace University_System.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public ICollection<Instructor> Instructors = new List<Instructor>();
        public ICollection<Course> Courses = new List<Course>();
        public ICollection<Trainee> Trainees = new List<Trainee>();
    }
}
