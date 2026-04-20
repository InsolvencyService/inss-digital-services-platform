using System.ComponentModel.DataAnnotations;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CaseReferenceAttribute : ValidationAttribute
{
    public CaseReferenceAttribute(string key)
    {
        Key = key;
    }

    public string Key { get; }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        } 
        
        ICaseReferenceService caseReferenceService = validationContext.GetRequiredService<ICaseReferenceService>();

        string caseReference = value.ToString()!;
        bool exists = caseReferenceService.CheckExistsAsync(caseReference).GetAwaiter().GetResult();

        if (exists)
        {
            return ValidationResult.Success;
        }
        
        ErrorMessage = Key; // Error message is calculated from the key
        return new ValidationResult(ErrorMessage, [validationContext.MemberName!]);
    }
}