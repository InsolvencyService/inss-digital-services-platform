using System.Globalization;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Exceptions;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class CheckAnswersPreProcessNodeTests
{
    private readonly CheckAnswersFlowNodeLoader _loader = new();
    private readonly FormModel _form = TestFormModels.CreateWithAddAnotherSection();
    private const string? NoState = null;
    
    [Fact]
    public async Task NullState_LoadAsync_ListsFullNameInAnswers()
    {
        SectionModel section = _form.Sections.First();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        FlowNode node = new() { Id = "NodeId1", PagePath = fullName.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = checkAnswers,
            State = NoState
        };
        
        await _loader.LoadAsync(context);

        Assert.Equal(2, checkAnswers.Items.Length);
        Assert.Contains(fullName.Value, checkAnswers.Items[0].Values);
    }
    
    [Fact]
    public async Task NullState_LoadAsync_ListsAgeInAnswers()
    {
        SectionModel section = _form.Sections.First();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        age.Value = 45;
        FlowNode node = new() { Id = "NodeId1", PagePath = age.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = checkAnswers,
            State = NoState
        };
        
        await _loader.LoadAsync(context);

        Assert.Equal(2, checkAnswers.Items.Length);
        Assert.Contains(age.Value.ToString(CultureInfo.CurrentCulture), checkAnswers.Items[1].Values);
    }
    
    [Fact]
    public async Task HasMismatchedIdState_LoadAsync_ThrowsException()
    {
        const string changeState = "0";
        SectionModel section = _form.Sections.First();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = checkAnswers.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = checkAnswers,
            State = changeState
        };
        
        FlowchartException exception = await Assert.ThrowsAsync<FlowchartException>(async () => await _loader.LoadAsync(context));

        Assert.Equal("The expected working pages not not match edit Ids.", exception.Message);
    }
    
    [Fact]
    public async Task HasIdState_LoadAsync_AddsAddAnotherFullNameToWorkingPages()
    {
        const string changeState = "0";
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = checkAnswers.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = checkAnswers,
            State = changeState
        };
        
        await _loader.LoadAsync(context);

        Assert.Equal(2, checkAnswers.Items.Length);

        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        FullNameModel itemFullName = addAnother.Items.GetFirstOf<FullNameModel>();
        FullNameModel fullName = groupInfo.WorkingPages.GetFirstOf<FullNameModel>();
        Assert.Equal(itemFullName.Id, fullName.Id);
        Assert.Equal(itemFullName.Value, fullName.Value);
    }
    
    [Fact]
    public async Task HasIdState_Load_AddsAddAnotherAgeToWorkingPages()
    {
        const string changeState = "0";
        SectionModel section = _form.Sections.First();
        AddAnotherModel addAnother = CreateAddAnother();
        CheckAnswersModel checkAnswers = section.Pages.GetFirstOf<CheckAnswersModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = checkAnswers.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = checkAnswers,
            State = changeState
        };
        
        await _loader.LoadAsync(context);

        Assert.Equal(2, checkAnswers.Items.Length);
        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        AgeModel itemAge = addAnother.Items.GetFirstOf<AgeModel>();
        AgeModel age = groupInfo.WorkingPages.GetFirstOf<AgeModel>();
        Assert.Equal(itemAge.Id, age.Id);
        Assert.Equal(itemAge.Value, age.Value);
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