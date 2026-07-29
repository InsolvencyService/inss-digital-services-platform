using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.Common.IPUpload;
using Inss.GovUk.Forms.IPUpload.Application.Clients;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly IValidationFactory _validationFactory;
    private readonly IUploadContentBlobClient _uploadContentBlobClient;
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;

    public FileUploadFlowNodeExecutor(IValidationFactory validationFactory, IUploadContentBlobClient uploadContentBlobClient)
    {
        _validationFactory = validationFactory;
        _uploadContentBlobClient = uploadContentBlobClient;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        XmlFileUploadModel fileUpload = context.CurrentPage.As<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        EmployerDetailsModel employerDetails = context.Section.Pages.GetFirstOf<EmployerDetailsModel>();
        fileUploadErrors.ClearErrors();
        fileUploadErrors.Filename = fileUpload.Filename;
        context.Section.ReturnUrl = null;
        
        object redundancyPayment = FileHelper.GetRedundancyPaymentObject(fileUpload.Contents);
        Validate(fileUploadErrors, employerDetails, redundancyPayment);

        if (fileUploadErrors.HasErrors)
        {
            return context.CurrentNode.NextNodes[FileUploadErrorIndex];
        }

        await _uploadContentBlobClient.SaveAsync(fileUpload.Contents, context.Form.Id);

        return context.CurrentNode.NextNodes[SummaryIndex];
    }
    
    private void Validate(IPUploadXmlErrorsModel fileUploadErrors, EmployerDetailsModel employerDetails, object model)
    {
        IBaseValidator validator = _validationFactory.Create(model);
        ValidatorContext context = validator.Validate(employerDetails);
        fileUploadErrors.BuildErrorList(context.Errors);
    }
}