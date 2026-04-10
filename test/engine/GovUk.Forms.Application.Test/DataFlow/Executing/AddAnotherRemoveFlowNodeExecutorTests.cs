using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Executing;

public class AddAnotherRemoveFlowNodeExecutorTests
{
    private readonly AddAnotherRemoveFlowNodeExecutor _executor = new();
    
    [Fact]
    public async Task RemovePage_ExecuteAsync_ReturnsNextNode()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode node = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"]};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = remove
        };

        NodeId? nextNodeId = await _executor.ExecuteAsync(context);

        Assert.NotNull(nextNodeId);
        Assert.Equal("NodeId1", nextNodeId);
    }
    
    [Fact]
    public async Task RemoveNotConfirmed_ExecuteAsync_DoesNotDeleteAddAnotherItems()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        remove.RemoveConfirmed = false;
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        addAnother.Items.Add(fullName.Clone());
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.Value = 45;
        addAnother.Items.Add(age.Clone());
        FlowNode node = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"]};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = remove
        };

        await _executor.ExecuteAsync(context);

        Assert.NotEmpty(addAnother.Items);
    }
    
    [Fact]
    public async Task RemoveConfirmed_ExecuteAsync_DeletesAddAnotherItems()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        remove.RemoveConfirmed = true;
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        addAnother.Items.Add(fullName.Clone());
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.Value = 45;
        addAnother.Items.Add(age.Clone());
        FlowNode node = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"]};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = remove
        };

        await _executor.ExecuteAsync(context);

        Assert.Empty(addAnother.Items);
    }
    
    [Fact]
    public async Task RemoveConfirmed_ExecuteAsync_ClearsWorkingPageValues()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        remove.RemoveConfirmed = true;
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        addAnother.Items.Add(fullName.Clone());
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.Value = 45;
        addAnother.Items.Add(age.Clone());
        FlowNode node = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"]};
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = remove
        };

        await _executor.ExecuteAsync(context);

        Assert.Empty(fullName.Value);
        Assert.Equal(0, age.Value);
    }
}