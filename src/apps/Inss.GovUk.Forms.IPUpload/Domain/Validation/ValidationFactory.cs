using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ValidationFactory : IValidationFactory
{
    private readonly ICaseReferenceService _caseReferenceService;

    public ValidationFactory(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }
    
    public IBaseValidator Create(object model)
    {
        return model switch
        {
            Inss.Common.IPUpload.Employee.Spreadsheet.RP14A spreadsheetRP14A 
                => new EmployeeSpreadsheetValidator(spreadsheetRP14A, _caseReferenceService),
            Inss.Common.IPUpload.Employee.Api.RP14A apiRP14A 
                => new EmployeeApiValidator(apiRP14A, _caseReferenceService),
            Inss.Common.IPUpload.Employer.Spreadsheet.RP14 spreadsheetRP14 
                => new EmployerSpreadsheetValidator(spreadsheetRP14, _caseReferenceService),
            Inss.Common.IPUpload.Employer.Api.RP14 apiRP14 
                => new EmployerApiValidator(apiRP14, _caseReferenceService),
            _ => throw new IPUploadException($"Unable to validate {model.GetType()} RP14/A upload.")
        };
    }
}