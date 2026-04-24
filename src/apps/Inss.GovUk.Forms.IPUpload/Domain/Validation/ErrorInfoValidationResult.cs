using System.ComponentModel.DataAnnotations;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorInfoValidationResult : ValidationResult
{
    public ErrorInfoValidationResult(ValidationResult validationResult, ErrorInfo errorInfo , string value) : base(validationResult)
    {
        ErrorInfo = errorInfo;
        Value = value;
    }

    public ErrorInfo ErrorInfo { get; }
    
    public string Value { get; }
}