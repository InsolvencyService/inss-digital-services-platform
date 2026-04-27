using System.ComponentModel.DataAnnotations;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RangePropertyAttribute : RangeAttribute
{
    public RangePropertyAttribute(string key, int minimum, int maximum) : base(minimum, maximum)
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