using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.Common.IPUpload;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;

    public FileUploadFlowNodeExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        XmlFileUploadModel fileUpload = context.CurrentPage.As<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.ClearErrors();
        fileUploadErrors.Filename = fileUpload.Filename;
        
        object redundancyPayment = FileHelper.GetRedundancyPaymentObject(fileUpload.Contents);
        await ValidateAsync(fileUploadErrors, redundancyPayment);

        return fileUploadErrors.HasErrors 
            ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
            : context.CurrentNode.NextNodes[SummaryIndex];
    }
    
    private async Task ValidateAsync(IPUploadXmlErrorsModel fileUploadErrors, object redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();

        BaseValidator validator = redundancyPayment switch
        {
            Inss.Common.IPUpload.Employee.Spreadsheet.RP14A spreadsheetRP14A 
                => new EmployeeSpreadsheetValidator(spreadsheetRP14A, caseReferenceService),
            Inss.Common.IPUpload.Employee.Api.RP14A apiRP14A 
                => new EmployeeApiValidator(apiRP14A, caseReferenceService),
            Inss.Common.IPUpload.Employer.Spreadsheet.RP14 spreadsheetRP14 
                => new EmployerSpreadsheetValidator(spreadsheetRP14, caseReferenceService),
            Inss.Common.IPUpload.Employer.Api.RP14 apiRP14 
                => new EmployerApiValidator(apiRP14, caseReferenceService),
            _ => throw new IPUploadException($"Unable to validate {redundancyPayment.GetType()} RP14/A upload.")
        };

        ValidatorContext context = await validator.ValidateAsync();
        fileUploadErrors.BuildErrorList(context.Errors);
    }
}