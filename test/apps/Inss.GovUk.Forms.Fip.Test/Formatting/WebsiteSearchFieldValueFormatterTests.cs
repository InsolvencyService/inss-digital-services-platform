using Inss.GovUk.Forms.Fip.Formatting;
using Xunit;

namespace Inss.GovUk.Forms.Fip.Test.Formatting;

public class WebsiteSearchFieldValueFormatterTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MissingValue_Format_ReturnsEmptyString(string? value)
    {
        WebsiteSearchFieldValueFormatter formatter = new();

        string result = formatter.Format(value);
        
        Assert.Empty(result);
    }
    
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("https://www.example.com")]
    [InlineData("https://example.co.uk/path/to/page?query=1#section")]
    public void ValidLink_Format_ReturnsLink(string? value)
    {
        WebsiteSearchFieldValueFormatter formatter = new();

        string result = formatter.Format(value);
        
        Assert.Contains(value!, result);
    }
}