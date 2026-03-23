using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Executing;

public class SectionSummaryFlowNodeExecutorTests
{
    private readonly SectionSummaryFlowNodeExecutor _executor = new();
    
    [Fact]
    public async Task SummaryPage_ExecuteAsync_ReturnsNullNextNode()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = summary.Path};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = summary
        };

        NodeId? nextNodeId = await _executor.ExecuteAsync(context);

        Assert.Null(nextNodeId);
    }
    
    [Fact]
    public async Task SummaryPage_ExecuteAsync_SetsSectionCompleted()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        section.State = SectionStateTypes.InProgress;
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = summary.Path};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = summary
        };

        await _executor.ExecuteAsync(context);

        Assert.Equal(SectionStateTypes.Completed, section.State);
    }
    
    [Fact]
    public async Task SummaryPageCompleted_ExecuteAsync_SetsEachPageLocked()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        section.State = SectionStateTypes.InProgress;
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = summary.Path};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = summary
        };

        await _executor.ExecuteAsync(context);

        PageModel[] unlockedPages = section.Pages.GetCompletedPages().Where(
            p => (p.EditMode & PageEditTypes.Locked) != PageEditTypes.Locked).ToArray();
        Assert.Empty(unlockedPages);
    }
}