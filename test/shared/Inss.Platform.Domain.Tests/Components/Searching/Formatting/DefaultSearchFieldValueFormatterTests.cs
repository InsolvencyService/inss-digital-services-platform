using Inss.Platform.Domain.Components.Searching.Formatting;
using Xunit;

namespace Inss.Platform.Domain.Tests.Components.Searching.Formatting;

public class DefaultSearchFieldValueFormatterTests
{
    [Fact]
    public void NullString_Format_ReturnsEmptyValue()
    {
        DefaultSearchFieldValueFormatter formatter = new();

        string value = formatter.Format([null]);
        
        Assert.Empty(value);
    }
    
    [Fact]
    public void NonNullOrEmptyString_Format_ReturnsValue()
    {
        DefaultSearchFieldValueFormatter formatter = new();

        string value = formatter.Format(["Hello"]);
        
        Assert.Equal("Hello", value);
    }
    
    [Fact]
    public void ListOfValues_Format_ReturnsValue()
    {
        DefaultSearchFieldValueFormatter formatter = new();

        string value = formatter.Format(["Hello", "World"]);
        
        Assert.Equal("Hello World", value);
    }
}