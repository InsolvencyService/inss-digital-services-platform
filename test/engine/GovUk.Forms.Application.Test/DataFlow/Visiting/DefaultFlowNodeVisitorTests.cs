using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Visiting;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Visiting;

public class DefaultFlowNodeVisitorTests
{
    private readonly DefaultFlowNodeVisitor _visitor = new();

    [Fact]
    public async Task NoVisits_VisitAsync_TracksVisit()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        FlowNode fullNameNode = new() { Id = "FullNameId", PagePath = fullName.Path };
        FlowNode ageNode = new() { Id = "ageId", PagePath = age.Path };
        FlowNodeContext context = new()
        {
            Nodes = [fullNameNode, ageNode],
            CurrentNode = fullNameNode,
            Form = form,
            Section = section,
            CurrentPage = fullName
        };

        await _visitor.VisitAsync(context);

        Assert.Single(section.VisitedNodes);
        Assert.Equal(fullNameNode.Id, section.VisitedNodes[0]);
    }
    
    [Fact]
    public async Task HasVisits_VisitAsync_TracksVisit()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        FlowNode fullNameNode = new() { Id = "FullNameId", PagePath = fullName.Path };
        FlowNode ageNode = new() { Id = "ageId", PagePath = age.Path };
        section.Track(fullNameNode.Id);
        FlowNodeContext context = new()
        {
            Nodes = [fullNameNode, ageNode],
            CurrentNode = ageNode,
            Form = form,
            Section = section,
            CurrentPage = fullName
        };

        await _visitor.VisitAsync(context);

        Assert.Equal(2, section.VisitedNodes.Length);
        Assert.Equal(ageNode.Id, section.VisitedNodes[1]);
    }
}