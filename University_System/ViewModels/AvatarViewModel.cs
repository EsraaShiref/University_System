namespace University_System.ViewModels
{
    public class AvatarViewModel
    {
        public string? Name { get; set; }

        // File name stored in the database, e.g. "instructor1.jpg"
        public string? FileName { get; set; }

        // Sub-folder under wwwroot/images, e.g. "instructors" or "trainees"
        public string Folder { get; set; } = "";

        // Design-system size class: avatar-sm, avatar-md, avatar-lg, avatar-xl
        public string Size { get; set; } = "avatar-md";
    }
}
