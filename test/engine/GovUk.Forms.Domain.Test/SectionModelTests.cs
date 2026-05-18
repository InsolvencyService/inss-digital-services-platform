using Xunit;

namespace GovUk.Forms.Domain.Test;

public class SectionModelTests
{
    [Fact]
    public void AddUntrackedNode_Track_AddsToVisitedList()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        
        section.Track("NodeId1");
        
        Assert.Equal(0, section.VisitedNodes.IndexOf("NodeId1"));
    }
    
    [Fact]
    public void AddingAlreadyTrackedNode_Track_DoesNotAddAgainToVisitedList()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        
        section.Track("NodeId1");
        section.Track("NodeId1");

        Assert.Single(section.VisitedNodes);
        Assert.Equal(0, section.VisitedNodes.IndexOf("NodeId1"));
    }
    
    [Fact]
    public void UntrackingNode_Track_RemovesFromVisitedList()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        section.Track("NodeId1");
        section.Track("NodeId2");

        section.Untrack("NodeId2");
        
        Assert.Single(section.VisitedNodes);
        Assert.Equal(0, section.VisitedNodes.IndexOf("NodeId1"));
    }
    
    [Fact]
    public void UntrackingKnownNode_Track_IsIgnored()
    {
        SectionModel section = TestSectionModels.CreateYourDetailsSection();
        section.Track("NodeId1");
        section.Track("NodeId2");

        section.Untrack("NodeId3");
        
        Assert.Equal(2, section.VisitedNodes.Length);
        Assert.Equal(0, section.VisitedNodes.IndexOf("NodeId1"));
        Assert.Equal(1, section.VisitedNodes.IndexOf("NodeId2"));
    }
}