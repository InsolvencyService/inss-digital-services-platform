using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain;

public class IPUploadXmlErrorsModelTests
{
    private readonly IPUploadXmlErrorsModel _model = new();

    [Fact]
    public void ErrorNotExists_AddOrMergeError_AddsToErrorList()
    {
        ErrorInfo errorInfo = new()
        {
            Category = "Case",
            Property = "Case reference",
            Error = "Unknown case reference",
            HeaderCaption = "Case references"
        };

        _model.AddOrMergeError(errorInfo);

        Assert.Single(_model.Errors);
    }
    
    [Fact]
    public void ErrorExists_AddOrMergeError_AddsToExistingErrorList()
    {
        ErrorInfo errorInfo = new()
        {
            Category = "Shareholders",
            Property = "Percentage",
            Error = "Invalid percentage",
            HeaderCaption = "Shareholders"
        };
        errorInfo.AddRow("50.123");
        _model.AddOrMergeError(errorInfo);

        errorInfo = new ErrorInfo
        {
            Category = "Shareholders",
            Property = "Percentage",
            Error = "Invalid percentage",
            HeaderCaption = "Shareholders"
        };
        errorInfo.AddRow("25.123");
        
        _model.AddOrMergeError(errorInfo);
        
        Assert.Single(_model.Errors);
        Assert.Equal(2, _model.Errors[0].RowCount);
    }
}