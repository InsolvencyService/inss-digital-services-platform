using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class IPUploadXmlErrorsModel : PageModel
{
    private readonly List<ErrorInfo> _errors = [];
    
    public string Filename { get; set; }

    public bool HasErrors => _errors.Count > 0;
    
    public void AddError(string category, string property, string subcategory)
    {
        ErrorInfo? error = _errors.FirstOrDefault(e => e.Category == category && e.Property == property && e.SubCategory == subcategory);

        if (error is null)
        {
            error = new ErrorInfo { Category = category, Property = property, SubCategory = subcategory };
            _errors.Add(error);
        }

        error.Count++;
    }

    public ErrorInfo[] GetErrors(string category)
    {
        return _errors.Where(e => e.Category == category).ToArray();
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