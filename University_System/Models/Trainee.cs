using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University_System.Models
{
    public class Trainee
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(260)]
        public string? Image { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; } = null!;

        [Range(0, 100)]
        public int Grade { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public ICollection<CourseResult> CourseResults = new List<CourseResult>();
    }
}
