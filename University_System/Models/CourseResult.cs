using System.ComponentModel.DataAnnotations.Schema;

namespace University_System.Models
{
    public class CourseResult
    {
        public int Id { get; set; }
        public int Degree { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [ForeignKey("Trainee")]
        public int TraineeId { get; set; }
        public Trainee Trainee { get; set; } = null!;
    }
}
