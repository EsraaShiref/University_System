using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace University_System.Validation
{
    /// <summary>The value must be less than or equal to another property (e.g. MinDegree &lt;= Degree).</summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LessOrEqualToPropertyAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _otherProperty;

        public LessOrEqualToPropertyAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
            ErrorMessage = $"The value cannot be greater than {otherProperty}.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var other = validationContext.ObjectType.GetProperty(_otherProperty);
            if (other == null) return new ValidationResult($"Unknown property '{_otherProperty}'.");

            var otherValue = other.GetValue(validationContext.ObjectInstance);
            if (value is IComparable current && otherValue != null && current.CompareTo(otherValue) > 0)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            Merge(context.Attributes, "data-val", "true");
            Merge(context.Attributes, "data-val-lteprop", ErrorMessage!);
            Merge(context.Attributes, "data-val-lteprop-other", _otherProperty);
        }

        private static void Merge(IDictionary<string, string> attributes, string key, string value)
        {
            if (!attributes.ContainsKey(key)) attributes.Add(key, value);
        }
    }
}
