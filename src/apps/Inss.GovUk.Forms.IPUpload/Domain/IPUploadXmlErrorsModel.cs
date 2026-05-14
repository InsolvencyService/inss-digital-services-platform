using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
// ReSharper disable MemberCanBePrivate.Global

namespace Inss.GovUk.Forms.IPUpload.Domain;

public sealed class IPUploadXmlErrorsModel : PageModel
{
    public string Filename { get; set; }

    public bool HasErrors => Errors.Length > 0;

    public ErrorInfo[] Errors { get; set; } = [];
    
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
        List<ErrorInfo> errors = new(Errors);
        ErrorInfo? errorInfo = errors.FirstOrDefault(e => e.Category == category && e.Property == property && e.Error == error);

        if (errorInfo is null)
        {
            errorInfo = new ErrorInfo { Category = category, Property = property, Error = error, Hint = hint };
            errorInfo.AddRow("Forenames", "Surname", "Date of birth", "NI number", "Cell value");
            errors.Add(errorInfo);
        }

        errorInfo.AddRow(firstName, surname, dob.ToString(CultureInfo.CurrentCulture), nino, value);
        //errorInfo.AddIdentifier(firstName, surname, dob, nino, value);
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