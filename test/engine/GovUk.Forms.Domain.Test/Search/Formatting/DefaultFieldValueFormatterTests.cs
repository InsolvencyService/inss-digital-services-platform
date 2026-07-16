using GovUk.Forms.Domain.Search.Formatting;
using Xunit;

namespace GovUk.Forms.Domain.Test.Search.Formatting;

public class DefaultFieldValueFormatterTests
{
    [Fact]
    public void NullString_Format_ReturnsEmptyValue()
    {
        DefaultFieldValueFormatter formatter = new();

        string value = formatter.Format([null]);
        
        Assert.Empty(value);
    }
    
    [Fact]
    public void NonNullOrEmptyString_Format_ReturnsValue()
    {
        DefaultFieldValueFormatter formatter = new();

        string value = formatter.Format(["Hello"]);
        
        Assert.Equal("Hello", value);
    }
}