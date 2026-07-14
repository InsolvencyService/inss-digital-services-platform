using GovUk.Forms.Components.Extensions;
using Xunit;

namespace GovUk.Forms.Components.Test.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void InvalidString_Or_ReturnsAlternateValue(string? value)
    {
        const string alternativeValue = "AlternateText";
        
        string result = value.Or(alternativeValue);
        
        Assert.Equal(alternativeValue, result);
    }
    
    [Fact]
    public void ValidString_Or_ReturnsAlternateValue()
    {
        const string alternativeValue = "AlternateText";
        const string value = "Text";
        
        string result = value.Or(alternativeValue);
        
        Assert.Equal(value, result);
    }
}