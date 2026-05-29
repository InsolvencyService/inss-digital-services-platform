// ReSharper disable MemberCanBePrivate.Global - Cosmos won't serialize properly
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global - Cosmos won't serialize properly
using System.Globalization;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ErrorPropertySummary
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public Error[] Errors { get; set; } = [];
    
    public string Key { get; init; }
    
    public bool IsEmployeeErrors => Errors.Length > 0 && Errors[0] is EmployeeError;
    
    public string GetProperty()
    {
        ValidationInfo validationInfo = ValidationInfoLookup.Get(Key);
        return validationInfo.Property;
    }

    public string GetFormattedMessage()
    {
        ValidationInfo validationInfo = ValidationInfoLookup.Get(Key);
        return Errors.Length == 1
            ? validationInfo.SingularErrorPattern
            : validationInfo.PluralErrorPattern.Replace("[COUNT]", Errors.Length.ToString(CultureInfo.CurrentCulture));
    }
    
    public string? GetHint()
    {
        ValidationInfo validationInfo = ValidationInfoLookup.Get(Key);
        return validationInfo.Hint;
    }

    public void AddError(Error error)
    {
        List<Error> errors = [..Errors, error];
        Errors = errors.ToArray();
    }
}