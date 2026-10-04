using System.ComponentModel.DataAnnotations;

namespace University_System.ViewModels
{
    public class DepartmentFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Required, StringLength(100)]
        public string Manager { get; set; } = "";
    }
}
