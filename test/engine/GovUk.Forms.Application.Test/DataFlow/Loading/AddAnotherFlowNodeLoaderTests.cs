using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class AddAnotherFlowNodeLoaderTests
{
    private readonly AddAnotherFlowNodeLoader _loader = new();
    private readonly FormModel _form = TestFormModels.CreateWithAddAnotherSection();

    [Fact]
    public async Task ItemsExist_LoadAsync_PopulatesDetailsInSummary()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        FlowNodeContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = _form,
            Section = section,
            CurrentPage = addAnother
        };
        
        await _loader.LoadAsync(context);

        FullNameModel itemFullName = addAnother.Items.GetFirstOf<FullNameModel>();
        Assert.Equal(itemFullName.Value, addAnother.SummaryInfo[0].Value);
    }

    [Fact]
    public async Task ItemsExist_LoadAsync_PopulatesChangeUrl()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        FlowNodeContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = _form,
            Section = section,
            CurrentPage = addAnother
        };

        await _loader.LoadAsync(context);
        
        Assert.Equal($"{checkAnswers.Path}/?index=0", addAnother.SummaryInfo[0].ChangeUrl);
    }
    
    [Fact]
    public async Task ItemsExist_LoadAsync_PopulatesRemoveUrl()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId2", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        FlowNodeContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = _form,
            Section = section,
            CurrentPage = addAnother
        };

        await _loader.LoadAsync(context);
        
        Assert.Equal($"{remove.Path}/?index=0", addAnother.SummaryInfo[0].RemoveUrl);
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