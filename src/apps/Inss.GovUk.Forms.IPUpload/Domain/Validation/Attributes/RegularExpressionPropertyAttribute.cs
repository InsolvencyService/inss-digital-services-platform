using System.ComponentModel.DataAnnotations;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RegularExpressionPropertyAttribute : RegularExpressionAttribute
{
    public RegularExpressionPropertyAttribute(string key, string pattern) : base(pattern)
    {
        Key = key;
    }

    public string Key { get; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ErrorMessage = Key; // Error message is calculated from the key
        return base.IsValid(value, validationContext);
    }
}