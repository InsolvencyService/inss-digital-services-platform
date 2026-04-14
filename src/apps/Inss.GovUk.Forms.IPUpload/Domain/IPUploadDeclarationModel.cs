using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.IPUpload.Domain;

// TODO: Refactor and clean uip below once we know how errors will be presented to user

public class IPUploadDeclarationModel : StaticHtmlModel
{
    public IPUploadDeclarationModel()
    {
        Key = "ipupload-declaration";
    }
    
    public bool Accepted { get; set; }

    public override void CopyTo(PageModel target)
    {
        IPUploadDeclarationModel ipUploadDeclaration = target.As<IPUploadDeclarationModel>();
        ipUploadDeclaration.Accepted = Accepted;
    }
}

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

    public override void ClearValues()
    {
        base.ClearValues();
        Filename = string.Empty;
        _errors.Clear();
    }
    
    public override void CopyTo(PageModel target)
    {
        IPUploadXmlErrorsModel ipUploadXmlErrors = target.As<IPUploadXmlErrorsModel>();
        ipUploadXmlErrors.Filename = Filename;
    }
}

public sealed class ErrorInfo
{
    public string Category { get; init; }
    
    public string Property { get; init; }
    
    public string SubCategory { get; init; }
    
    public int Count { get; set; }
}