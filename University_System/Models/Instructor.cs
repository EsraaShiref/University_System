using System.ComponentModel.DataAnnotations.Schema;

namespace University_System.Models
{
    public class Instructor
    {
        public int Id { get; set; }
        public string InsName { get; set; }
        public string? Img { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }
        public string? Address { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
