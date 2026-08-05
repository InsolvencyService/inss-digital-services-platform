using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Validation;
using Xunit;

namespace Inss.Platform.Domain.Tests.Validation;

public class ValidationRuleItemListTests
{
    [Fact]
    public void UnknownKey_GetValue_ThrowsException()
    {
        ValidationRuleItemList rules = new()
        {
            ["Key1"] = "Value1"
        };

        ComponentException exception = Assert.Throws<ComponentException>(() => rules.GetValue<string>("UnknownKey"));
        
        Assert.Equal($"Unable to find a validation rule item for UnknownKey.", exception.Message);
    }
    
    [Fact]
    public void KnownKeyForStringValue_GetValue_ReturnsStringValue()
    {
        ValidationRuleItemList rules = new()
        {
            ["Key1"] = "Value1"
        };

        string value = rules.GetValue<string>("Key1");
        
        Assert.Equal("Value1", value);
    }
    
    [Fact]
    public void KnownKeyForIntValue_GetValue_ReturnsStringValue()
    {
        ValidationRuleItemList rules = new()
        {
            ["Key1"] = "123"
        };

        int value = rules.GetValue<int>("Key1");
        
        Assert.Equal(123, value);
    }
}