using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadErrorFlowNodeLoaderTests
{
    private readonly FileUploadErrorFlowNodeLoader _fileUploadErrorFlowNodeLoader = new();
    
    [Fact]
    public async Task LoadingErrorPage_LoadAsync_LinksNodeToErrorDetails()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel ipUploadSection = form.Sections["IP Upload"];
        IPUploadXmlErrorsModel fileUploadErrors = ipUploadSection.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = fileUploadErrors.Path, NextNodes = ["NodeId2", "NodeId3"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = ipUploadSection,
            CurrentPage = fileUploadErrors
        };

        await _fileUploadErrorFlowNodeLoader.LoadAsync(context);
        
        IPUploadXmlErrorDetailsModel fileUploadErrorDetails = ipUploadSection.Pages.GetFirstOf<IPUploadXmlErrorDetailsModel>();
        Assert.NotNull(fileUploadErrorDetails.LinkedToNode);
        Assert.Equal("NodeId3", fileUploadErrorDetails.LinkedToNode);
    }
}