using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace University_System.Validation
{
    /// <summary>Server check plus a "data-val-filesize" client adapter (see validation-extensions.js).</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxFileSizeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly long _maxBytes;

        public MaxFileSizeAttribute(int maxMegabytes)
        {
            _maxBytes = maxMegabytes * 1024L * 1024L;
            ErrorMessage = $"The photo must be {maxMegabytes} MB or smaller.";
        }

        public override bool IsValid(object? value) => value is not IFormFile file || file.Length <= _maxBytes;

        public void AddValidation(ClientModelValidationContext context)
        {
            Merge(context.Attributes, "data-val", "true");
            Merge(context.Attributes, "data-val-filesize", ErrorMessage!);
            Merge(context.Attributes, "data-val-filesize-max", _maxBytes.ToString());
        }

        private static void Merge(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key)) attributes.Add(key, value);
        }
    }
}
