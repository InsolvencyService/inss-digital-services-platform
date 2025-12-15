using Xunit;

namespace INSS.Platform.Portal.Domain.Tests;

public class NavigationHistoryTests
{
    [Fact]
    public void NoHistory_Peek_ReturnsNull()
    {
        NavigationHistory history = new();

        NavigationItem? item = history.Peek();
        
        Assert.Null(item);
    }
    
    [Fact]
    public void SingleHistory_Peek_ReturnsItem()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));

        NavigationItem? item = history.Peek();
        
        Assert.NotNull(item);
        Assert.Equal("1234", item.PageId);
        Assert.Equal("/form/section/page1", item.PageUrl);
    }
    
    [Fact]
    public void NoHistory_Push_AddsItem()
    {
        NavigationHistory history = new();
        
        history.Push(new NavigationItem("1234", "/form/section/page1"));

        NavigationItem? item = history.Peek();
        Assert.NotNull(item);
        Assert.Equal("1234", item.PageId);
        Assert.Equal("/form/section/page1", item.PageUrl);
    }
    
    [Fact]
    public void ItemExistsAsLastHistoryEntry_Push_DoesntAddItem()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));

        history.Push(new NavigationItem("1234", "/form/section/page1"));
        
        Assert.Equal(1, history.Count);
    }
    
    [Fact]
    public void NoHistory_Pop_ReturnsNull()
    {
        NavigationHistory history = new();

        NavigationItem? item = history.Pop();
        
        Assert.Null(item);
    }
    
    [Fact]
    public void SingleHistory_Pop_ReturnsItem()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));

        NavigationItem? item = history.Pop();
        
        Assert.NotNull(item);
        Assert.Equal("1234", item.PageId);
        Assert.Equal("/form/section/page1", item.PageUrl);
        Assert.Equal(0, history.Count);
    }
    
    [Fact]
    public void MultipleHistory_Pop_ReturnsLastItem()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));
        history.Push(new NavigationItem("5678", "/form/section/page2"));

        NavigationItem? item = history.Pop();
        
        Assert.NotNull(item);
        Assert.Equal("5678", item.PageId);
        Assert.Equal("/form/section/page2", item.PageUrl);
        Assert.Equal(1, history.Count);
    }
    
    [Fact]
    public void HasHistory_Clear_EmptiesAllItems()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));
        history.Push(new NavigationItem("5678", "/form/section/page2"));

        history.Clear();
        
        Assert.Equal(0, history.Count);
    }
    
    [Fact]
    public void PathNotLastItem_IsLastEntry_ReturnsFalse()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));
        history.Push(new NavigationItem("1234", "/form/section/page2"));
        
        bool isLastEntry = history.IsLastEntry("/form/section/page1");
        
        Assert.False(isLastEntry);
    }
    
    [Fact]
    public void PathIsLastItem_IsLastEntry_ReturnsTrue()
    {
        NavigationHistory history = new();
        history.Push(new NavigationItem("1234", "/form/section/page1"));
        history.Push(new NavigationItem("1234", "/form/section/page2"));
        
        bool isLastEntry = history.IsLastEntry("/form/section/page2");
        
        Assert.True(isLastEntry);
    }
}