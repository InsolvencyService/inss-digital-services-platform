using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class StaticHtmlFlowNodeLoaderTests : TestBase<StaticHtmlFlowNodeLoader>
{
    private const string? NoState = null;
    
    [Fact]
    public async Task FromKey_LoadAsync_SetsHtmlToDisplay()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections[0];
        StaticHtmlModel staticHtml = section.Pages.GetFirstOf<StaticHtmlModel>();
        MockFor<IStaticContentProvider>().GetAsync(staticHtml.Key).Returns("<html/>");
        FlowNode staticHtmlNode = new() { Id = "NodeId1", PagePath = staticHtml.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [staticHtmlNode],
            CurrentNode = staticHtmlNode,
            Form = form,
            Section = section,
            Page = staticHtml,
            State = NoState
        };
        
        await Subject.LoadAsync(context);
        
        Assert.Equal("<html/>", staticHtml.Html);
    }
    
    [Fact]
    public async Task StaticHtmlAlreadySet_LoadAsync_DoesNotCallProvider()
    {
        FormModel form = TestFormModels.CreateWithIPUploadSection();
        SectionModel section = form.Sections[0];
        StaticHtmlModel staticHtml = section.Pages.GetFirstOf<StaticHtmlModel>();
        staticHtml.Html = "<html/>";
        FlowNode staticHtmlNode = new() { Id = "NodeId1", PagePath = staticHtml.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [staticHtmlNode],
            CurrentNode = staticHtmlNode,
            Form = form,
            Section = section,
            Page = staticHtml,
            State = NoState
        };
        
        await Subject.LoadAsync(context);

        await MockFor<IStaticContentProvider>().Received(0).GetAsync(staticHtml.Key);
    }
}