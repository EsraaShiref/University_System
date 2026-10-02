namespace University_System.ViewModels
{
    public class DepartmentListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public int InstructorsCount { get; set; }
        public int CoursesCount { get; set; }
        public int TraineesCount { get; set; }
    }
}
