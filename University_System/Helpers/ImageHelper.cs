using System.Text.RegularExpressions;

namespace University_System.Helpers
{
    public static class ImageHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

        // Files saved by the app are named <32 hex chars>.<ext>
        private static readonly Regex UploadedName = new("^[0-9a-f]{32}\\.[a-z]+$", RegexOptions.Compiled);

        /// <summary>
        /// Server-side check: extension, size and the real file signature (so a renamed .exe is rejected).
        /// Returns an error message, or null when the file is acceptable.
        /// </summary>
        public static async Task<string?> ValidateAsync(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return "Only JPG, PNG, WEBP or GIF images are allowed.";
            if (file.Length == 0)
                return "The selected file is empty.";
            if (file.Length > MaxBytes)
                return "The photo must be 2 MB or smaller.";

            var header = new byte[12];
            await using var stream = file.OpenReadStream();
            var read = await stream.ReadAtLeastAsync(header, header.Length, throwOnEndOfStream: false);

            return HasImageSignature(header, read) ? null : "The file is not a valid image.";
        }

        private static bool HasImageSignature(byte[] h, int length)
        {
            if (length < 12) return false;

            var jpeg = h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF;
            var png = h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47;
            var gif = h[0] == 'G' && h[1] == 'I' && h[2] == 'F' && h[3] == '8';
            var webp = h[0] == 'R' && h[1] == 'I' && h[2] == 'F' && h[3] == 'F'
                    && h[8] == 'W' && h[9] == 'E' && h[10] == 'B' && h[11] == 'P';

            return jpeg || png || gif || webp;
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
