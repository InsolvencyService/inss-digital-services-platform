using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Enums;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Executing;

public class SubmitSectionFlowNodeExecutorTests
{
    private readonly SubmitSectionFlowNodeExecutor _executor;
    private readonly ISubmitSectionService _submitSectionService;
    private readonly IUserSessionProvider _userSessionProvider;
    
    public SubmitSectionFlowNodeExecutorTests()
    {
        _submitSectionService = Substitute.For<ISubmitSectionService>();
        _userSessionProvider = Substitute.For<IUserSessionProvider>();
        ServiceCollection services = [];
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        services.AddKeyedScoped<ISubmitSectionService>(section.Path, (_, _) => _submitSectionService);
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        _executor = new SubmitSectionFlowNodeExecutor(serviceProvider, _userSessionProvider);
    }
    
    [Fact]
    public async Task SummaryPage_ExecuteAsync_ReturnsNullNextNode()
    {
        _userSessionProvider.ResolveAsync().Returns("UserId");
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
    public async Task FinalExecuteTrue_ExecuteAsync_CallsSubmitService()
    {
        _userSessionProvider.ResolveAsync().Returns("UserId");
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = summary.Path, NextNodes = []};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = summary,
            FinalExecuteStep = true
        };

        await _executor.ExecuteAsync(context);

        await _submitSectionService.Received(1).SubmitAsync(section, "UserId");
    }
    
    [Fact]
    public async Task FinalExecuteTrue_ExecuteAsync_DoesNotCallSubmitService()
    {
        _userSessionProvider.ResolveAsync().Returns("UserId");
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
            UpdatedPage = summary,
            FinalExecuteStep = false
        };

        await _executor.ExecuteAsync(context);

        await _submitSectionService.Received(0).SubmitAsync(section, "UserId");
    }
    
    [Fact]
    public async Task FinalExecuteTrueAndSectionComplete_ExecuteAsync_SetsEachPageLocked()
    {
        _userSessionProvider.ResolveAsync().Returns("UserId");
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
            UpdatedPage = summary,
            FinalExecuteStep = false
        };

        await _executor.ExecuteAsync(context);

        PageModel[] unlockedPages = section.Pages.GetCompletedPages().Where(
            p => (p.EditMode & PageEditTypes.Locked) != PageEditTypes.Locked).ToArray();
        Assert.Empty(unlockedPages);
    }
}