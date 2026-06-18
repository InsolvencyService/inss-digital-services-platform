using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class DeclarationFlowNodePreviousPathProviderTests
{
    private readonly DeclarationFlowNodePreviousPathProvider _provider;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    
    public DeclarationFlowNodePreviousPathProviderTests()
    {
        _pagePropertiesProvider = Substitute.For<IPagePropertiesProvider>();
        _provider = new DeclarationFlowNodePreviousPathProvider(_pagePropertiesProvider);
    }
    
    [Fact]
    public async Task FirstPageIsDeclaration_LoadAsync_SetsProviderPreviousSiteRoot()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections["IP Upload"];
        IPUploadDeclarationModel declaration = section.Pages.GetFirstOf<IPUploadDeclarationModel>();
        XmlFileUploadModel fileUpload = section.Pages.GetFirstOf<XmlFileUploadModel>();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        PostSubmitModel postSubmit = section.Pages.GetFirstOf<PostSubmitModel>();
        FlowNode declarationNode = new() { Id = "DeclarationId", PagePath = declaration.Path, NextNodes = ["FileUploadId"] };
        FlowNode fileUploadNode = new() { Id = "FileUploadId", PagePath = fileUpload.Path, NextNodes = ["SummaryModeId"] };
        FlowNode summaryNode = new() { Id = "SummaryId", PagePath = summary.Path, NextNodes = ["PostSubmitId"] };
        FlowNode postSubmitNode = new() { Id = "PostSubmitId", PagePath = postSubmit.Path };
        FlowNodeContext context = new()
        {
            Nodes = [declarationNode, fileUploadNode, summaryNode, postSubmitNode],
            CurrentNode = fileUploadNode,
            Form = form,
            Section = section,
            CurrentPage = fileUpload,
            RefererPath = summary.Path
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(new ContentPath("/"), _pagePropertiesProvider.PreviousPagePath);
    }
}