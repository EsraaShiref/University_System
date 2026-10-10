namespace University_System.Helpers
{
    public static class SearchHelper
    {
        public const int MaxLength = 100;

        /// <summary>Trims the search text, caps its length and turns blanks into null.</summary>
        public static string? Clean(string? q)
        {
            if (string.IsNullOrWhiteSpace(q)) return null;
            q = q.Trim();
            return q.Length > MaxLength ? q[..MaxLength] : q;
        }
    }
}
