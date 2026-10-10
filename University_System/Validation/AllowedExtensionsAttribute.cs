using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace University_System.Validation
{
    /// <summary>Server check plus a "data-val-fileext" client adapter (see validation-extensions.js).</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AllowedExtensionsAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions.Select(e => e.ToLowerInvariant()).ToArray();
            ErrorMessage = "Only JPG, PNG, WEBP or GIF images are allowed.";
        }

        public override bool IsValid(object? value)
        {
            if (value is not IFormFile file) return true;
            return _extensions.Contains(Path.GetExtension(file.FileName).ToLowerInvariant());
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            Merge(context.Attributes, "data-val", "true");
            Merge(context.Attributes, "data-val-fileext", ErrorMessage!);
            Merge(context.Attributes, "data-val-fileext-allowed", string.Join(",", _extensions));
        }

        private static void Merge(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key)) attributes.Add(key, value);
        }
    }
}
