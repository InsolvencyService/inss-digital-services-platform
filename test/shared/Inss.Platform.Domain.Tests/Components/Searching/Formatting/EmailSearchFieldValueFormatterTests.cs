using Inss.Platform.Domain.Components.Searching.Formatting;
using Xunit;

namespace Inss.Platform.Domain.Tests.Components.Searching.Formatting;

public class EmailSearchFieldValueFormatterTests
{
    [Fact]
    public void NoValues_Format_ReturnsEmptyString()
    {
        EmailSearchFieldValueFormatter formatter = new();

        string result = formatter.Format([]);
        
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingValue_Format_ReturnsEmptyString(string? value)
    {
        EmailSearchFieldValueFormatter formatter = new();

        string result = formatter.Format([value]);
        
        Assert.Empty(result);
    }
    
    [Fact]
    public void ValidEmail_Format_ReturnsLink()
    {
        EmailSearchFieldValueFormatter formatter = new();

        string result = formatter.Format(["homer@simpsons.com"]);
        
        Assert.Contains("mailto:homer@simpsons.com", result);
    }
}