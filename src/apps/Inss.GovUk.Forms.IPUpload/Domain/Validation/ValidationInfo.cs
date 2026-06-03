namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ValidationInfo
{
    internal const string CaseReferenceFormat = "CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}";
    internal const string NinoFormat = "[A-CEGHJ-PR-TW-Za-ceghj-pr-tw-z]{1}[A-CEGHJ-NPR-TW-Za-ceghj-npr-tw-z]{1}[0-9]{6}[A-DFMa-dfm]{1}";
    internal const string MoneyFormat = @"^\d+(\.\d{2})?$";
    internal const string HolidayFormat = @"^\d+(\.\d{2})?$";
    internal const string PercentFormat = @"^\d+(\.\d{2})?$";
    
    public string Key { get; init; }
    
    public string Category { get; init; }
    
    public string Property { get; init; }
    
    public string SingularErrorPattern { get; init; }
    
    public string PluralErrorPattern { get; init; }
    
    public string? Hint { get; init; }
}