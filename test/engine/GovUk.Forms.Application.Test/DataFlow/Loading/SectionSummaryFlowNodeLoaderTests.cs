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
    
    [Fact]
    public async Task AddAnotherItems_LoadAsync_PopulatesFullNameInSummary()
    {
        AddAnotherModel addAnother = CreateAddAnother();
        SectionModel section = _form.Sections.First();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = summary
        };
        
        await _loader.LoadAsync(context);

        CheckAnswersItem[] items = summary.Items;
        FullNameModel fullName = addAnother.Items.GetFirstOf<FullNameModel>();
        Assert.Equal(addAnother.Title, items[0].Title);
        Assert.True(items[0].Values.Contains(fullName.Value));
    }
    
    [Fact]
    public async Task AddAnotherItems_LoadAsync_PopulatesAgeInSummary()
    {
        AddAnotherModel addAnother = CreateAddAnother();
        SectionModel section = _form.Sections.First();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = summary
        };
        
        await _loader.LoadAsync(context);

        CheckAnswersItem[] items = summary.Items;
        AgeModel age = addAnother.Items.GetFirstOf<AgeModel>();
        Assert.Equal(addAnother.Title, items[0].Title);
        Assert.True(items[0].Values.Contains(age.Value.ToString(CultureInfo.InvariantCulture)));
    }
    
    [Fact]
    public async Task AddAnotherItems_LoadAsync_PopulatesChangeUrlInSummary()
    {
        AddAnotherModel addAnother = CreateAddAnother();
        SectionModel section = _form.Sections.First();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = summary
        };
        
        await _loader.LoadAsync(context);

        foreach (CheckAnswersItem item in summary.Items)
        {
            Assert.Contains(addAnother.Path, item.ChangeUrl);
        }
    }
    
    [Fact]
    public async Task CompletedSection_LoadAsync_DoesNotPopulateChangeUrlInSummary()
    {
        SectionModel section = _form.Sections.First();
        section.SetCompleted();
        SummaryModel summary = section.Pages.GetFirstOf<SummaryModel>();
        FlowNode node = new() { Id = "NodeId1", PagePath = summary.Path, NextNodes = ["NodeId2"] };
        FlowNodeContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = _form,
            Section = section,
            CurrentPage = summary
        };
        
        await _loader.LoadAsync(context);
        
        foreach (CheckAnswersItem item in summary.Items)
        {
            Assert.Null(item.ChangeUrl);
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