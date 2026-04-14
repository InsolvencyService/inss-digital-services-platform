using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Executing;

public class AddAnotherWorkingPageFlowNodeExecutorTests
{
    private readonly AddAnotherWorkingPageFlowNodeExecutor _executor = new();
    
    [Fact]
    public async Task PageNotAddedToAddAnotherItems_ExecuteAsync_AttachesCopyOfPageToItemsList()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        FlowNode node = new() { Id = "NodeId1", PagePath = fullName.Path, NextNodes = ["NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = fullName
        };
        
        await _executor.ExecuteAsync(context);
        
        Assert.Single(addAnother.Items);
        FullNameModel itemFullName = addAnother.Items.GetFirstOf<FullNameModel>();
        Assert.Equal(fullName.Id, itemFullName.Id);
        Assert.Equal(fullName.Value, itemFullName.Value);
    }
    
    [Fact]
    public async Task PageAlreadyAddedToAddAnotherItems_Process_UpdatesExistingCopyOfPageInItemsList()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        fullName.Value = "Homer Simpson";
        addAnother.Items.Add(fullName.Clone());
        fullName.Value = "Marge Simpson";
        FlowNode node = new() { Id = "NodeId1", PagePath = fullName.Path, NextNodes = ["NodeId2"] };
        ExecuteContext context = new()
        {
            Nodes = [node],
            CurrentNode = node,
            Form = form,
            Section = section,
            UpdatedPage = fullName
        };
        
        await _executor.ExecuteAsync(context);
        
        Assert.Single(addAnother.Items);
        FullNameModel itemFullName = addAnother.Items.GetFirstOf<FullNameModel>();
        Assert.Equal(fullName.Id, itemFullName.Id);
        Assert.Equal(fullName.Value, itemFullName.Value);
    }
}