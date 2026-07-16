using GovUk.Forms.Domain.Search.Formatting;
using Xunit;

namespace GovUk.Forms.Domain.Test.Search.Formatting;

public class ResultDetailLinkFieldValueFormatterTests
{
    [Fact]
    public void SingleText_Format_ReturnsDetailLink()
    {
        ResultDetailLinkFieldValueFormatter formatter = new("/form/section/path");

        string value = formatter.Format(["Hello"]);
        
        Assert.Equal($"<a href='/form/section/path/?key=Hello' class='govuk-link'>Hello</a>", value);
    }
    
    [Fact]
    public void MultiText_Format_ReturnsDetailLink()
    {
        ResultDetailLinkFieldValueFormatter formatter = new("/form/section/path");

        string value = formatter.Format(["Hello World"]);
        
        Assert.Equal($"<a href='/form/section/path/?key=Hello+World' class='govuk-link'>Hello World</a>", value);
    }
}