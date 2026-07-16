using GovUk.Forms.Domain.Search.Formatting;
using Xunit;

namespace GovUk.Forms.Domain.Test.Search.Formatting;

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