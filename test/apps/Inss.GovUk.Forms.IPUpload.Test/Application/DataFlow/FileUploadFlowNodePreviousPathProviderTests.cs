using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Application.DataFlow;
using Inss.GovUk.Forms.IPUpload.Domain;
using NSubstitute;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Application.DataFlow;

public class FileUploadFlowNodePreviousPathProviderTests
{
    private readonly FileUploadFlowNodePreviousPathProvider _provider;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    
    public FileUploadFlowNodePreviousPathProviderTests()
    {
        _pagePropertiesProvider = Substitute.For<IPagePropertiesProvider>();
        _provider = new FileUploadFlowNodePreviousPathProvider(_pagePropertiesProvider);
    }
    
    [Fact]
    public async Task FromSummaryToUpload_LoadAsync_SetsProviderPreviousPagePathAsSummary()
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
        
        Assert.Equal(summary.Path, _pagePropertiesProvider.PreviousPagePath);
    }
    
    [Fact]
    public async Task FromDeclarationToUpload_LoadAsync_SetsProviderPreviousPagePathAsDeclaration()
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
            CurrentPage = fileUpload
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(declaration.Path, _pagePropertiesProvider.PreviousPagePath);
    }
}