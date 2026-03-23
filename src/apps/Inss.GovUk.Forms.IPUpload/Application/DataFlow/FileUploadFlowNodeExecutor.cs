using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;
    
    // private readonly IXsdProvider _xsdProvider;
    // private readonly IRedundancyPaymentProvider _redundancyPaymentProvider;
    //
    // public FileUploadFlowNodeExecutor(IXsdProvider xsdProvider, IRedundancyPaymentProvider redundancyPaymentProvider)
    // {
    //     _xsdProvider = xsdProvider;
    //     _redundancyPaymentProvider = redundancyPaymentProvider;
    // }

    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        XmlFileUploadModel fileUpload = context.UpdatedPage.As<XmlFileUploadModel>();

        // TODO: Hack - we need to uncouple the execute and processing as the execute gets called twice. Once before updates and once after
        // Below needs to move to a process before decider is called
        if (!string.IsNullOrWhiteSpace(fileUpload.Filename))
        {
            IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
            fileUploadErrors.ClearValues();
            fileUploadErrors.Filename = fileUpload.Filename;

            if (fileUpload.Filename.Equals("rp14a-with-error.xml", StringComparison.OrdinalIgnoreCase))
            {
                fileUploadErrors.AddError("Case", "Case reference", "The case reference provided does not match any of our records");

                fileUploadErrors.AddError("Employee", "Employee title", "missing employee titles");
                fileUploadErrors.AddError("Employee", "Employee title", "missing employee titles");
                fileUploadErrors.AddError("Employee", "National insurance number", "Invalid national insurance numbers");
                fileUploadErrors.AddError("Employee", "National insurance number", "Invalid national insurance numbers");
                fileUploadErrors.AddError("Employee", "National insurance number", "Invalid national insurance numbers");

                fileUploadErrors.AddError("Employment dates", "Employer start date", "invalid employer start dates");
                fileUploadErrors.AddError("Employment dates", "Employer start date", "invalid employer start dates");
            }


            /*
            XDocument document = fileUpload.GetXml();

            if (document.Root is null)
            {

                //validationResults.AddResult("The XML file is missing a root element", [nameof(fileUpload.Contents)]);
                return Task.CompletedTask;
            }

            XmlSchemaSet schemaSet = _xsdProvider.Load(document.Root);

            OnValidateFileUpload(fileUpload, document, schemaSet);

            Rp14A redundancyPayment = _redundancyPaymentProvider.Create(document);

            if (redundancyPayment.EmployerName.Length > 100)
            {
                //validationResults.AddResult("The employee name is too long", [nameof(XmlFileUploadModel.Contents)]);
            }

            // TODO:
            // 1. Check limits - may wish to split into array of sub validators or something - add to XSD or model?
            // 2. How do we reflect this to the user - could be lots of errors!
            */

            return ValueTask.FromResult<NodeId?>(fileUploadErrors.HasErrors 
                ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
                : context.CurrentNode.NextNodes[SummaryIndex]);
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FileUploadErrorIndex]);
    }
}