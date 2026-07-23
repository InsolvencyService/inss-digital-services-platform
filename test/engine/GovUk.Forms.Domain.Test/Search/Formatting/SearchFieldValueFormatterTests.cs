using GovUk.Forms.Domain.Search.Formatting;
using Xunit;

namespace GovUk.Forms.Domain.Test.Search.Formatting;

public class SearchFieldValueFormatterTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void InvalidType_CreateFormatter_ReturnsDefaultFormatter(string? type)
    {
        SearchFieldValueFormatter formatter = SearchFieldValueFormatter.CreateFormatter(type);

        Assert.IsType<DefaultSearchFieldValueFormatter>(formatter);
    }
    
    [Fact]
    public void UnknownType_CreateFormatter_ReturnsDefaultFormatter()
    {
        SearchFieldValueFormatter formatter = SearchFieldValueFormatter.CreateFormatter("Type,Assembly");

        Assert.IsType<DefaultSearchFieldValueFormatter>(formatter);
    }
    
    [Fact]
    public void KnownType_CreateFormatter_ReturnsFormatter()
    {
        SearchFieldValueFormatter formatter = SearchFieldValueFormatter.CreateFormatter(typeof(WebsiteSearchFieldValueFormatter).FullName);

        Assert.IsType<WebsiteSearchFieldValueFormatter>(formatter);
    }
}