using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ValidationFactory : IValidationFactory
{
    public IBaseValidator Create(object model)
    {
        return model switch
        {
            Inss.Common.IPUpload.Employee.Spreadsheet.RP14A spreadsheetRP14A => new EmployeeSpreadsheetValidator(spreadsheetRP14A),
            Inss.Common.IPUpload.Employee.Api.RP14A apiRP14A => new EmployeeApiValidator(apiRP14A),
            Inss.Common.IPUpload.Employer.Spreadsheet.RP14 spreadsheetRP14 => new EmployerSpreadsheetValidator(spreadsheetRP14),
            Inss.Common.IPUpload.Employer.Api.RP14 apiRP14 => new EmployerApiValidator(apiRP14),
            _ => throw new IPUploadException($"Unable to validate {model.GetType()} RP14/A upload.")
        };
    }
}