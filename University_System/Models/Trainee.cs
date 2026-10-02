using System.ComponentModel.DataAnnotations.Schema;

namespace University_System.Models
{
    public class Trainee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public string Address { get; set; } = null!;
        public int Grade { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
        public ICollection<CourseResult> CourseResults = new List<CourseResult>();
    }
}
