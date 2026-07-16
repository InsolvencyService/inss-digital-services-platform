using GovUk.Forms.Domain.Search.Formatting;
using Xunit;

namespace GovUk.Forms.Domain.Test.Search.Formatting;

public class WebsiteSearchFieldValueFormatterTests
{
    [Fact]
    public void NoValues_Format_ReturnsEmptyString()
    {
        WebsiteSearchFieldValueFormatter formatter = new();

        string result = formatter.Format([]);
        
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingValue_Format_ReturnsEmptyString(string? value)
    {
        WebsiteSearchFieldValueFormatter formatter = new();

        string result = formatter.Format([value]);
        
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://example.co.uk/path/to/page?query=1#section")]
    public void ValidLink_Format_ReturnsLink(string? value)
    {
        WebsiteSearchFieldValueFormatter formatter = new();

        string result = formatter.Format([value]);
        
        Assert.Contains(value!, result);
    }
}