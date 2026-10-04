using System.Text.RegularExpressions;

namespace University_System.Helpers
{
    public static class ImageHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

        // Files saved by the app are named <32 hex chars>.<ext>
        private static readonly Regex UploadedName = new("^[0-9a-f]{32}\\.[a-z]+$", RegexOptions.Compiled);

        /// <summary>Returns an error message, or null when the file is acceptable.</summary>
        public static string? Validate(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return "Only JPG, PNG, WEBP or GIF images are allowed.";
            if (file.Length > MaxBytes)
                return "The image must be 2 MB or smaller.";
            return null;
        }

        /// <summary>Saves the file under wwwroot/images/{folder} and returns the stored file name.</summary>
        public static async Task<string> SaveAsync(IFormFile file, IWebHostEnvironment env, string folder)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var directory = Path.Combine(env.WebRootPath, "images", folder);
            Directory.CreateDirectory(directory);

            await using var stream = File.Create(Path.Combine(directory, fileName));
            await file.CopyToAsync(stream);
            return fileName;
        }

        /// <summary>
        /// Deletes an uploaded image. Seed placeholders (instructor1.jpg ...) are shared by several
        /// records, so only files created by this app (GUID names) are ever removed.
        /// </summary>
        public static void DeleteUploaded(IWebHostEnvironment env, string folder, string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            fileName = Path.GetFileName(fileName);
            if (!UploadedName.IsMatch(fileName)) return;

            var path = Path.Combine(env.WebRootPath, "images", folder, fileName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
