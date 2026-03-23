using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.Primitives;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Executing;

public class AddAnotherFlowNodeExecutorTests
{
    private readonly AddAnotherFlowNodeExecutor _executor = new();
    
    [Fact]
    public async Task PageNotAddingAnother_ExecuteAsync_ThrowsException()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        FlowNode node = new() { Id = "NodeId", PagePath = fullName.Path };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = fullName
        };
        
        ModelException exception = await Assert.ThrowsAsync<ModelException>(async () => await _executor.ExecuteAsync(context));
        
        Assert.Equal($"Cannot cast to type {typeof(AddAnotherModel)}.", exception.Message);
    }
    
    [Fact]
    public async Task NotAddingAnotherItem_ExecuteAsync_ReturnsContinueNode()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        addAnother.AddAnotherItem = false;
        FlowNode node = new() { Id = "NodeId", PagePath = addAnother.Path, NextNodes = ["NodeId1", "NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = addAnother
        };
        
        NodeId? result = await _executor.ExecuteAsync(context);

        Assert.NotNull(result);
        Assert.Equal("NodeId2", result);
    }
    
    [Fact]
    public async Task AddingAnotherItem_ExecuteAsync_ReturnsFirstWorkingPageNode()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        addAnother.AddAnotherItem = true;
        FlowNode node = new() { Id = "NodeId", PagePath = addAnother.Path, NextNodes = ["NodeId1", "NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = addAnother
        };
        
        NodeId? result = await _executor.ExecuteAsync(context);
        
        Assert.NotNull(result);
        Assert.Equal("NodeId1", result);
    }
    
    [Fact]
    public async Task AddingAnotherItem_ExecuteAsync_ClearsWorkingPageValues()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        AddAnotherModel addAnother = groupInfo.AddAnother;
        addAnother.AddAnotherItem = true;
        FullNameModel fullName = groupInfo.WorkingPages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        AgeModel age = groupInfo.WorkingPages.GetFirstOf<AgeModel>();
        age.Value = 45;
        FlowNode node = new() { Id = "NodeId", PagePath = addAnother.Path, NextNodes = ["NodeId1", "NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = addAnother
        };
        
        await _executor.ExecuteAsync(context);
        
        Assert.Null(fullName.Value);
        Assert.Equal(0, age.Value);
    }
}