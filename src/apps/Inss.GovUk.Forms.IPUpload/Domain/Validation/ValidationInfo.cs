namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

/*internal abstract class ValidationInfo
{
    internal const string CaseReferenceFormat = "CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}";
    internal const string NinoFormat = "[A-CEGHJ-PR-TW-Za-ceghj-pr-tw-z]{1}[A-CEGHJ-NPR-TW-Za-ceghj-npr-tw-z]{1}[0-9]{6}[A-DFMa-dfm]{1}";
    internal const string MoneyFormat = @"^\d+(\.\d{2})?$";
    internal const string HolidayFormat = @"^\d+(\.\d{2})?$";
    internal const string PercentFormat = @"^\d+(\.\d{2})?$";
    
    protected ValidationInfo(string category, string property, string error, string? hint = null)
    {
        PropertyFormat = $"{category}|{property}";
        ErrorFormat = !string.IsNullOrWhiteSpace(hint) ? $"{error}|{hint}" : error;
    }
    
    internal string PropertyFormat { get; }
    
    internal string ErrorFormat { get; }
    
    internal static string GetCategory(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length > 0 ? parts[0] : "No category defined";
    }
    
    internal static string GetProperty(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length == 2 ? parts[1] : "No property defined";
    }
    
    internal static string GetError(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length > 0 ? parts[0] : "No error defined";
    }
    
    internal static string? GetHint(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length == 2 ? parts[1] : null;
    }
}*/

internal sealed class ValidationInfo
{
    internal const string CaseReferenceFormat = "CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}";
    internal const string NinoFormat = "[A-CEGHJ-PR-TW-Za-ceghj-pr-tw-z]{1}[A-CEGHJ-NPR-TW-Za-ceghj-npr-tw-z]{1}[0-9]{6}[A-DFMa-dfm]{1}";
    internal const string MoneyFormat = @"^\d+(\.\d{2})?$";
    internal const string HolidayFormat = @"^\d+(\.\d{2})?$";
    internal const string PercentFormat = @"^\d+(\.\d{2})?$";
    
    internal string Key { get; init; }
    
    internal string Category { get; init; }
    
    internal string Property { get; init; }
    
    internal string SingularErrorPattern { get; init; }
    
    internal string PluralErrorPattern { get; init; }
    
    internal string? Hint { get; init; }
}

internal static class ValidationInfoLookup
{
    private static readonly Dictionary<string, ValidationInfo> _lookup = [];

    static ValidationInfoLookup()
    {
        _lookup.Add(CaseValidationInfo.UnknownCaseReference.Key, CaseValidationInfo.UnknownCaseReference);
        _lookup.Add(CaseValidationInfo.MissingCaseReference.Key, CaseValidationInfo.MissingCaseReference);
        _lookup.Add(CaseValidationInfo.InvalidCaseReferenceFormat.Key, CaseValidationInfo.InvalidCaseReferenceFormat);
        _lookup.Add(CaseValidationInfo.InvalidCaseReferenceLength.Key, CaseValidationInfo.InvalidCaseReferenceLength);
        
        _lookup.Add(EmployerValidationInfo.InvalidEmployerNameLength.Key, EmployerValidationInfo.InvalidEmployerNameLength);
        
        _lookup.Add(EmployeeValidationInfo.MissingEmployeeSurname.Key, EmployeeValidationInfo.MissingEmployeeSurname);
        _lookup.Add(EmployeeValidationInfo.InvalidEmployeeSurnameLength.Key, EmployeeValidationInfo.InvalidEmployeeSurnameLength);
        _lookup.Add(EmployeeValidationInfo.InvalidAopOwedFormat.Key, EmployeeValidationInfo.InvalidAopOwedFormat);
        _lookup.Add(EmployeeValidationInfo.MissingEmployeeNino.Key, EmployeeValidationInfo.MissingEmployeeNino);
        _lookup.Add(EmployeeValidationInfo.InvalidEmployeeNinoFormat.Key, EmployeeValidationInfo.InvalidEmployeeNinoFormat);
        _lookup.Add(EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat.Key, EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat);
        _lookup.Add(EmployeeValidationInfo.InvalidEmployeeEmploymentDates.Key, EmployeeValidationInfo.InvalidEmployeeEmploymentDates);
        _lookup.Add(EmployeeValidationInfo.InvalidEmployeeAopDates.Key, EmployeeValidationInfo.InvalidEmployeeAopDates);
        
        _lookup.Add(EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat.Key, EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat);
        
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat.Key, EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange.Key, EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat.Key, EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange.Key, EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat.Key, EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange.Key, EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat.Key, EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayOwedRange.Key, EmployeeHolidayValidationInfo.InvalidHolidayOwedRange);
        _lookup.Add(EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange.Key, EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange);
        
    }

    internal static ValidationInfo Get(string key)
    {
        return _lookup[key];
    }
}