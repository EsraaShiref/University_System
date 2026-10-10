using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_System.Models
{
    public class Instructor
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string InsName { get; set; } = null!;

        [StringLength(260)]
        public string? Img { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "1000000")]
        public decimal Salary { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
