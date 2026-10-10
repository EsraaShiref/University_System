using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace University_System.Models
{
    [Index(nameof(TraineeId), nameof(CourseId), IsUnique = true)]
    public class CourseResult
    {
        public int Id { get; set; }

        [Range(0, 1000)]
        public int Degree { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [ForeignKey("Trainee")]
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; } = null!;
    }
}
