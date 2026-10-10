using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace University_System.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Department
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = null!;

        [Required, StringLength(100)]
        public string Manager { get; set; } = null!;

        public ICollection<Instructor> Instructors = new List<Instructor>();
        public ICollection<Course> Courses = new List<Course>();
        public ICollection<Trainee> Trainees = new List<Trainee>();
    }
}
