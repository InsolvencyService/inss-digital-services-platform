using Demo.GovUk.Forms.ContactUs.Application.DataFlow;
using Demo.GovUk.Forms.ContactUs.Domain;
using Demo.GovUk.Forms.ContactUs.Factories;
using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain;
using Xunit;

namespace Demo.GovUk.Forms.ContactUs.Test.Application.DataFlow;

public class AddAnotherUploadFilesFlowNodeLoaderTests
{
    private readonly AddAnotherUploadFilesFlowNodeLoader _loader = new();

    [Fact]
    public async Task LessThanTwoItems_LoadAsync_SetsCanAddAnotherTrue()
    {
        ContactUsFormFactory factory = new();
        FormModel form = factory.Create();
        SectionModel section = form.Sections["Send Us Files"];
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        addAnother.Items.Add(new FileUploadModel());
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId2", "NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId4", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        
        LoadContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = form,
            Section = section,
            Page = addAnother
        };
        
        await _loader.LoadAsync(context);
        
        Assert.True(addAnother.CanAddAnother);
    }
    
    [Fact]
    public async Task TwoItemsOrMore_LoadAsync_SetsCanAddAnotherFalse()
    {
        ContactUsFormFactory factory = new();
        FormModel form = factory.Create();
        SectionModel section = form.Sections["Send Us Files"];
        AddAnotherModel addAnother = section.Pages.GetFirstOf<AddAnotherModel>();
        RemoveModel remove = section.Pages.GetFirstOf<RemoveModel>();
        addAnother.Items.Add(new FileUploadModel());
        addAnother.Items.Add(new FileUploadModel());
        FlowNode addAnotherNode = new() { Id = "NodeId1", PagePath = addAnother.Path, NextNodes = ["NodeId2", "NodeId3"] };
        FlowNode removeNode = new() { Id = "NodeId4", PagePath = remove.Path, NextNodes = ["NodeId1"] };
        
        LoadContext context = new()
        {
            Nodes = [addAnotherNode, removeNode],
            CurrentNode = addAnotherNode,
            Form = form,
            Section = section,
            Page = addAnother
        };
        
        await _loader.LoadAsync(context);
        
        Assert.False(addAnother.CanAddAnother);
    }
}