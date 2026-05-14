using System.Globalization;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadErrorDetailsFlowNodeLoaderTests
{
    private readonly FileUploadErrorDetailsFlowNodeLoader _loader = new();

    [Fact]
    public async Task NullState_LoadAsync_ThrowsException()
    {
        LoadContext context = new() { State = null };

        IPUploadException exception = await Assert.ThrowsAsync<IPUploadException>(() => _loader.LoadAsync(context).AsTask());
        
        Assert.Equal("Unable to load the IP upload error details as the state is unset.", exception.Message);
    }

    [Fact]
    public async Task WithState_LoadAsync_InitializesErrorDetails()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        IPUploadXmlErrorsModel fileUploadErrors = ipUploadSection.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.AddOrMergeError(new EmployeeSpreadsheetErrorInfo(
            "Case", "Case reference", "[COUNT] unknown case reference", null,
            "Homer", "Simpson", DateOnly.Parse("10/04/1990", CultureInfo.CurrentCulture), 
            "AB112233H", "CN12345678"));
        ErrorInfo[] errors = fileUploadErrors.GetErrors("Case");
        IPUploadXmlErrorDetailsModel fileUploadErrorDetails = ipUploadSection.Pages.GetFirstOf<IPUploadXmlErrorDetailsModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = fileUploadErrorDetails.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            Page = fileUploadErrorDetails,
            State = errors[0].Id
        };

        await _loader.LoadAsync(context);
        
        Assert.Equal(errors[0], fileUploadErrorDetails.CurrentErrorDetail);
        Assert.Equal(fileUploadErrors.Path, fileUploadErrorDetails.PreviousPagePath);
        Assert.True(fileUploadErrorDetails.FullWidthLayout);
    }
}