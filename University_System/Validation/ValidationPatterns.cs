namespace University_System.Validation
{
    // The same patterns run on the server (.NET) and in the browser (JavaScript),
    // so they only use syntax both engines understand. \u0600-\u06FF = Arabic letters.
    public static class ValidationPatterns
    {
        public const string PersonName = @"^[A-Za-z\u0600-\u06FF][A-Za-z\u0600-\u06FF .'\-]*$";
        public const string Title = @"^[A-Za-z0-9\u0600-\u06FF][A-Za-z0-9\u0600-\u06FF &.,'()\-:/+#]*$";
        public const string Address = @"^[A-Za-z0-9\u0600-\u06FF][A-Za-z0-9\u0600-\u06FF .,\u060C'#/()\-]*$";
        public const string Money = @"^\d{1,16}(\.\d{1,2})?$";
    }
}
