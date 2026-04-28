namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

public static class ValidationAnnotationProvider
{
    private static readonly Dictionary<string, ErrorInfoHeader> _errorInfoLookup;

    static ValidationAnnotationProvider()
    {
        _errorInfoLookup = new CaseLookups();
        _errorInfoLookup = _errorInfoLookup.Concat(new EmployerLookups()).ToDictionary(k => k.Key, v => v.Value);
        _errorInfoLookup = _errorInfoLookup.Concat(new EmployeeLookups()).ToDictionary(k => k.Key, v => v.Value);
        _errorInfoLookup = _errorInfoLookup.Concat(new EmployeePayLookups()).ToDictionary(k => k.Key, v => v.Value);
        _errorInfoLookup = _errorInfoLookup.Concat(new EmployeeHolidayLookups()).ToDictionary(k => k.Key, v => v.Value);
    }
    
    public static ErrorInfo GetErrorInfo(string key)
    {
        ErrorInfoHeader errorInfo = _errorInfoLookup[key];

        return new ErrorInfo
        {
            Category = errorInfo.Category,
            Property = errorInfo.Property,
            Error = errorInfo.Error,
            Hint = errorInfo.Hint
        };
    }
}