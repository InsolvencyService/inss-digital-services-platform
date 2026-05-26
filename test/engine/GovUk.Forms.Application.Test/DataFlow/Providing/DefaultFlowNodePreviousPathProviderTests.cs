using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Providing;

public class DefaultFlowNodePreviousPathProviderTests
{
    private readonly DefaultFlowNodePreviousPathProvider _provider;
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    
    public DefaultFlowNodePreviousPathProviderTests()
    {
        _pagePropertiesProvider = Substitute.For<IPagePropertiesProvider>();
        _provider = new DefaultFlowNodePreviousPathProvider(_pagePropertiesProvider);
    }

    [Fact]
    public async Task RefererIsSummary_UpdateAsync_SetsProviderPreviousPathToSummaryPage()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel section = form.Sections[0];
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        FlowNode fullNameNode = new() { Id = "FullNameId", PagePath = fullName.Path };
        FlowNode summaryNode = new() { Id = "SummaryId", PagePath = summary.Path };
        FlowNodeContext context = new()
        {
            Nodes = [fullNameNode, summaryNode],
            CurrentNode = fullNameNode,
            Form = form,
            Section = section,
            CurrentPage = fullName,
            RefererPath = summary.Path
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(summary.Path, _pagePropertiesProvider.PreviousPagePath);
    }
     
    [Fact]
    public async Task RefererIsFullName_UpdateAsync_SetsProviderPreviousPathToFullName()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel section = form.Sections[0];
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        FlowNode fullNameNode = new() { Id = "FullNameId", PagePath = fullName.Path };
        FlowNode addressNode = new() { Id = "AddressId", PagePath = address.Path };
        section.Track(fullNameNode.Id);
        FlowNodeContext context = new()
        {
            Nodes = [fullNameNode, addressNode],
            CurrentNode = addressNode,
            Form = form,
            Section = section,
            CurrentPage = address,
            RefererPath = fullName.Path
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(fullName.Path, _pagePropertiesProvider.PreviousPagePath);
    }
    
    [Fact]
    public async Task CurrentPageIsFirstPageInSingleSection_UpdateAsync_SetsProviderPreviousPathToEmptyPath()
    {
        FormModel form = TestFormModels.CreateWithYourDetailsSection();
        SectionModel section = form.Sections[0];
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AddressModel address = section.Pages.GetFirstOf<AddressModel>();
        FlowNode fullNameNode = new() { Id = "FullNameId", PagePath = fullName.Path };
        FlowNode addressNode = new() { Id = "AddressId", PagePath = address.Path };
        FlowNodeContext context = new()
        {
            Nodes = [fullNameNode, addressNode],
            CurrentNode = fullNameNode,
            Form = form,
            Section = section,
            CurrentPage = fullName
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(new ContentPath("/"), _pagePropertiesProvider.PreviousPagePath);
    }
}