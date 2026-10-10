using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace University_System.Models
{
    [Index(nameof(DepartmentId), nameof(Name), IsUnique = true)]
    public class Course
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Range(1, 1000)]
        public int Degree { get; set; }

        [Range(0, 1000)]
        public int MinDegree { get; set; }

        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
        public ICollection<CourseResult> courseResults { get; set; } = new List<CourseResult>();

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;
    }
}
