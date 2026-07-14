using System.ComponentModel;
using GovUk.Forms.Domain.Extensions;
using Xunit;

namespace GovUk.Forms.Domain.Test.Extensions;

public class EnumExtensionsTests
{
    [Fact]
    public void EnumValueWithNoDescription_Description_ReturnsName()
    {
        string description = TestTypes.First.Description();
        
        Assert.Equal("First", description);
    }
    
    [Fact]
    public void EnumValueWithDescription_Description_ReturnsName()
    {
        string description = TestTypes.Second.Description();
        
        Assert.Equal("Second enum", description);
    }

    private enum TestTypes
    {
        First,
        [Description("Second enum")]
        Second
    }
}