using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.Common.IPUpload;
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
        if (redundancyPayment is Inss.Common.IPUpload.Employee.Spreadsheet.RP14A spreadsheetRP14A)
        {
            await ValidateSpreadsheetUploadAsync(fileUploadErrors, spreadsheetRP14A);
        }
        else if (redundancyPayment is Inss.Common.IPUpload.Employee.Api.RP14A apiRP14A)
        {
            await ValidateApiUploadAsync(fileUploadErrors, apiRP14A);
        }
        else if (redundancyPayment is Inss.Common.IPUpload.Employer.Spreadsheet.RP14 spreadsheetRP14)
        {
            await ValidateSpreadsheetUploadAsync(fileUploadErrors, spreadsheetRP14);
        }
        else if (redundancyPayment is Inss.Common.IPUpload.Employer.Api.RP14 apiRP14)
        {
            await ValidateApiUploadAsync(fileUploadErrors, apiRP14);
        }
    }

    private async Task ValidateSpreadsheetUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors, 
        Inss.Common.IPUpload.Employee.Spreadsheet.RP14A redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();
        EmployeeSpreadsheetValidator validator = new(caseReferenceService);
        ValidatorContext context = await validator.ValidateAsync(redundancyPayment);
        fileUploadErrors.BuildErrorList(context.Errors);
    }
    
    private async Task ValidateSpreadsheetUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors,
        Inss.Common.IPUpload.Employer.Spreadsheet.RP14 redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();
        EmployerSpreadsheetValidator validator = new(caseReferenceService);
        ValidatorContext context = await validator.ValidateAsync(redundancyPayment);
        fileUploadErrors.BuildErrorList(context.Errors);
    }
    
    private async Task ValidateApiUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors, 
        Inss.Common.IPUpload.Employee.Api.RP14A redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();
        EmployeeApiValidator validator = new(caseReferenceService);
        ValidatorContext context = await validator.ValidateAsync(redundancyPayment);
        fileUploadErrors.BuildErrorList(context.Errors);
    }
    
    private async Task ValidateApiUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors, 
        Inss.Common.IPUpload.Employer.Api.RP14 redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();
        EmployerApiValidator validator = new(caseReferenceService);
        ValidatorContext context = await validator.ValidateAsync(redundancyPayment);
        fileUploadErrors.BuildErrorList(context.Errors);
    }
}