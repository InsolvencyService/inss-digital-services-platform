using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Visiting;
using GovUk.Forms.Domain;
using Xunit;

namespace GovUk.Forms.Application.Test.DataFlow.Visiting;

public class ResetTrackingFlowNodeVisitorTests
{
    private readonly ResetTrackingFlowNodeVisitor _visitor = new();
    
    [Fact]
    public async Task HasVisits_VisitAsync_ResetsTrackedVisits()
    {
        FormModel form = TestFormModels.CreateWithAddAnotherSection();
        SectionModel section = form.Sections[0];
        FullNameModel fullName = section.Pages.GetFirstOf<FullNameModel>();
        AgeModel age = section.Pages.GetFirstOf<AgeModel>();
        FlowNode fullNameNode = new() { Id = "FullNameId", PagePath = fullName.Path };
        FlowNode ageNode = new() { Id = "ageId", PagePath = age.Path };
        section.Track(fullNameNode.Id);
        section.Track(ageNode.Id);
        FlowNodeContext context = new()
        {
            Nodes = [fullNameNode, ageNode],
            CurrentNode = ageNode,
            Form = form,
            Section = section,
            CurrentPage = fullName
        };

        await _visitor.VisitAsync(context);

        Assert.Empty(section.VisitedNodes);
    }
}