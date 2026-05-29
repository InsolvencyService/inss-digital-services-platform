using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
// ReSharper disable MemberCanBePrivate.Global

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class IPUploadXmlErrorsModel : PageModel
{
    public string Filename { get; set; }

    public bool HasErrors => SummaryList.Length > 0;

    public ErrorSummary[] SummaryList { get; set; } = [];
    
    public void BuildErrorList(List<Error> errors)
    {
        List<ErrorSummary> summaryList = [];
        
        foreach (Error error in errors)
        {
            ErrorSummary? existingSummary = summaryList.FirstOrDefault(s => s.Category == error.GetCategory());

            if (existingSummary is null)
            {
                existingSummary = new ErrorSummary { Category = error.GetCategory() };
                summaryList.Add(existingSummary);
            }

            ErrorPropertySummary? propertySummary = existingSummary.Properties.FirstOrDefault(p => p.Key == error.Key);

            if (propertySummary is null)
            {
                propertySummary = new ErrorPropertySummary { Key = error.Key };
                existingSummary.AddProperty(propertySummary);
            }
                
            propertySummary.AddError(error);
        }

        SummaryList = summaryList.ToArray();
    }

    public ErrorSummary GetSummaryForCategory(string category)
    {
        return SummaryList.FirstOrDefault(s => s.Category == category) ?? new ErrorSummary { Category = category };
    }

    public ErrorPropertySummary GetPropertyErrors(string id)
    {
        foreach (ErrorSummary errorSummary in SummaryList)
        {
            foreach (ErrorPropertySummary errorPropertySummary in errorSummary.Properties)
            {
                if (errorPropertySummary.Id == id)
                {
                    return errorPropertySummary;
                }
            }
        }

        throw new IPUploadException($"Unable to find the property error details for {id}.");
    }
    
    public void ClearErrors()
    {
        Filename = string.Empty;
        SummaryList = [];
    }
    
    public override void CopyTo(PageModel target)
    {
        IPUploadXmlErrorsModel ipUploadXmlErrors = target.As<IPUploadXmlErrorsModel>();
        ipUploadXmlErrors.Filename = Filename;
    }
}