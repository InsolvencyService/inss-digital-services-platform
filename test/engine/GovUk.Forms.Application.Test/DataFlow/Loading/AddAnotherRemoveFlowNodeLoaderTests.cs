using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Serialization;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class AddAnotherRemoveFlowNodeLoaderTests
{
    private readonly AddAnotherRemoveFlowNodeLoader _loader = new();
    private readonly FormModel _form = TestFormModels.CreateWithAddAnotherSection();

    public AddAnotherRemoveFlowNodeLoaderTests()
    {
        FormSerializer.Initialize(typeof(PageModel).Assembly);
    }
    
    [Fact]
    public async Task RemoveDetails_LoadAsync_SetsSetIndex()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        LoadContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = _form,
            Section = section,
            Page = remove,
            State = "0"
        };
        
        await _loader.LoadAsync(context);

        Assert.Equal(0, remove.SetIndex);
    }
    
    [Fact]
    public async Task RemoveDetails_LoadAsync_UsesFirstPageInQuestion()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        LoadContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = _form,
            Section = section,
            Page = remove,
            State = "0"
        };
        
        await _loader.LoadAsync(context);

        Assert.Equal("Do you want to remove Homer Simpson?", remove.RemoveQuestion);
    }
    
    [Fact]
    public async Task RemoveDetails_LoadAsync_ReturnsNullNextNode()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        LoadContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = _form,
            Section = section,
            Page = remove,
            State = "0"
        };
        
        NodeId? nextNodeId = await _loader.LoadAsync(context);

        Assert.Null(nextNodeId);
    }
    
    private AddAnotherModel CreateAddAnother()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        AddAnotherModel addAnother = groupInfo.AddAnother;
        addAnother.AddAnotherItem = true;
        FullNameModel fullName = groupInfo.WorkingPages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        AgeModel age = groupInfo.WorkingPages.GetFirstOf<AgeModel>();
        age.Value = 45;
        addAnother.Items.AddRange([fullName.Clone(), age.Clone()]);
        return addAnother;
    }
}