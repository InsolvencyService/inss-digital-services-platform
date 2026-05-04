using System.Globalization;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Loading;

public class SectionSummaryFlowNodeLoaderTests
{
    private readonly FormModel _form = TestFormModels.CreateWithAddAnotherSection();
    private readonly SectionSummaryFlowNodeLoader _loader = new();
    private const string? NoState = null;
    
    [Fact]
    public async Task AddAnotherItems_LoadAsync_PopulatesFullNameInSummary()
    {
        AddAnotherModel addAnother = CreateAddAnother();
        SectionModel section = _form.Sections.First();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            Page = summary,
            State = NoState
        };
        
        await _loader.LoadAsync(context);

        SummaryModel.SummaryInfo[] infoList = summary.Overview;
        FullNameModel fullName = addAnother.Items.GetFirstOf<FullNameModel>();
        Assert.Equal(addAnother.Title, infoList[0].Title);
        Assert.True(infoList[0].Values.Contains(fullName.Value));
    }
    
    [Fact]
    public async Task AddAnotherItems_LoadAsync_PopulatesAgeInSummary()
    {
        AddAnotherModel addAnother = CreateAddAnother();
        SectionModel section = _form.Sections.First();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            Page = summary,
            State = NoState
        };
        
        await _loader.LoadAsync(context);

        SummaryModel.SummaryInfo[] infoList = summary.Overview;
        AgeModel age = addAnother.Items.GetFirstOf<AgeModel>();
        Assert.Equal(addAnother.Title, infoList[0].Title);
        Assert.True(infoList[0].Values.Contains(age.Value.ToString(CultureInfo.CurrentCulture)));
    }
    
    [Fact]
    public async Task AddAnotherItems_LoadAsync_PopulatesChangeUrlInSummary()
    {
        AddAnotherModel addAnother = CreateAddAnother();
        SectionModel section = _form.Sections.First();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            Page = summary,
            State = NoState
        };
        
        await _loader.LoadAsync(context);

        SummaryModel.SummaryInfo[] infoList = summary.Overview;

        foreach (SummaryModel.SummaryInfo info in infoList)
        {
            Assert.Contains(addAnother.Path, info.ChangeUrl!);
        }
    }
    
    [Fact]
    public async Task CompletedSection_LoadAsync_DoesNotPopulateChangeUrlInSummary()
    {
        SectionModel section = _form.Sections.First();
        section.SetCompleted();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        LoadContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            Page = summary,
            State = NoState
        };
        
        await _loader.LoadAsync(context);

        SummaryModel.SummaryInfo[] infoList = summary.Overview;

        foreach (SummaryModel.SummaryInfo info in infoList)
        {
            Assert.Null(info.ChangeUrl!);
        }
    }
    
    private AddAnotherModel CreateAddAnother()
    {
        SectionModel section = _form.Sections.First();
        AddAnotherGroup groupInfo = section.Pages.GetFirstOf<AddAnotherGroup>();
        AddAnotherModel addAnother = groupInfo.AddAnother;
        FullNameModel fullName = groupInfo.WorkingPages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        fullName.SetCompleted();
        AgeModel age = groupInfo.WorkingPages.GetFirstOf<AgeModel>();
        age.Value = 45;
        age.SetCompleted();
        addAnother.Items.AddRange([fullName.Clone(), age.Clone()]);
        addAnother.SetCompleted();
        return addAnother;
    }
}