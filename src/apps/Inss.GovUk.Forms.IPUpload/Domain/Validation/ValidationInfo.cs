namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal abstract class ValidationInfo
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
}