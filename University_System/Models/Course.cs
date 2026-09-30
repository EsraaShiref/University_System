using System.ComponentModel.DataAnnotations.Schema;

namespace University_System.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Degree { get; set; }
        public int MinDegree { get; set; }
        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
        public ICollection<CourseResult> courseResults { get; set; } = new List<CourseResult>();

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; }
    }
}
