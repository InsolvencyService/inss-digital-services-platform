using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
// ReSharper disable MemberCanBePrivate.Global

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class IPUploadXmlErrorsModel : PageModel
{
    public string Filename { get; set; }

    public bool HasErrors => Errors.Length > 0;

    public ErrorInfo[] Errors { get; set; } = [];
    
    public void AddOrMergeError(ErrorInfo errorInfo)
    {
        List<ErrorInfo> errors = new(Errors);
        ErrorInfo? existingErrorInfo = errors.FirstOrDefault(e => e.IsMatch(errorInfo));

        if (existingErrorInfo is null)
        {
            errors.Add(errorInfo);
        }
        else
        {
            existingErrorInfo.AddRow(errorInfo.GetValueRow());
        }
        
        Errors = errors.ToArray();
    }
    
    public ErrorInfo[] GetErrors(string category)
    {
        return Errors.Where(e => e.Category == category).OrderBy(e => e.Property).ToArray();
    }

    public ErrorInfo? GetError(string id)
    {
        return Errors.FirstOrDefault(e => e.Id == id);
    }

    public void ClearErrors()
    {
        Filename = string.Empty;
        Errors = [];
    }
    
    public override void CopyTo(PageModel target)
    {
        IPUploadXmlErrorsModel ipUploadXmlErrors = target.As<IPUploadXmlErrorsModel>();
        ipUploadXmlErrors.Filename = Filename;
    }
}