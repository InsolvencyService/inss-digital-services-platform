using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class IPUploadXmlErrorsModel : PageModel
{
    private readonly List<ErrorInfo> _errors = [];
    
    public string Filename { get; set; }

    public bool HasErrors => _errors.Count > 0;
    
    public void AddError(
        string firstName,
        string surname,
        DateOnly dob,
        string nino,
        string value,
        string category, 
        string property, 
        string error,
        string? hint)
    {
        ErrorInfo? errorInfo = _errors.FirstOrDefault(e => e.Category == category && e.Property == property && e.Error == error);

        if (errorInfo is null)
        {
            errorInfo = new ErrorInfo { Category = category, Property = property, Error = error, Hint = hint };
            _errors.Add(errorInfo);
        }

        errorInfo.AddIdentifier(firstName, surname, dob, nino, value);
    }
    
    public ErrorInfo[] GetErrors(string category)
    {
        return _errors.Where(e => e.Category == category).OrderBy(e => e.Property).ToArray();
    }

    public ErrorInfo? GetError(string id)
    {
        return _errors.FirstOrDefault(e => e.Id == id);
    }

    public void ClearErrors()
    {
        Filename = string.Empty;
        _errors.Clear();
    }
    
    public override void CopyTo(PageModel target)
    {
        IPUploadXmlErrorsModel ipUploadXmlErrors = target.As<IPUploadXmlErrorsModel>();
        ipUploadXmlErrors.Filename = Filename;
    }
}