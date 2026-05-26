using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public sealed class FileUploadErrorDetailsFlowNodePreviousPathProviderTests
{
    private readonly FileUploadErrorDetailsFlowNodePreviousPathProvider _provider;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    
    public FileUploadErrorDetailsFlowNodePreviousPathProviderTests()
    {
        _pagePropertiesProvider = Substitute.For<IPagePropertiesProvider>();
        _provider = new FileUploadErrorDetailsFlowNodePreviousPathProvider(_pagePropertiesProvider);
    }
    
    [Fact]
    public async Task OnUploadErrorsPage_LoadAsync_UpdatesProviderPreviousPagePath()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        IPUploadXmlErrorsModel fileUploadErrors = section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = fileUploadErrors.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            CurrentPage = fileUploadErrors
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(fileUploadErrors.Path, _pagePropertiesProvider.PreviousPagePath);
    }
}