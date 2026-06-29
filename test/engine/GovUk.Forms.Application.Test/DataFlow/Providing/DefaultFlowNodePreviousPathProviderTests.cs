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
    public async Task AnyPage_UpdateAsync_SetsProviderPreviousPathToRoot()
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
            CurrentNode = addressNode,
            Form = form,
            Section = section,
            CurrentPage = address
        };

        await _provider.UpdateAsync(context);
        
        Assert.Equal(new ContentPath("/"), _pagePropertiesProvider.PreviousPagePath);
    }
}