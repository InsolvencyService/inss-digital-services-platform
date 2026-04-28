using System.ComponentModel.DataAnnotations;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Attributes;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Attributes;

public class RangePropertyAttributeTests
{
    private readonly ValidationContext _validationContext = new(Value);
    private const int Value = 50;
    
    [Fact]
    public void FromKey_Validate_AssignsErrorKeyToErrorMessage()
    {
        RangePropertyAttribute attribute = new("TestKey", 10, 100);
        
        attribute.Validate(Value, _validationContext);
        
        Assert.Equal(attribute.Key, attribute.ErrorMessage);
    }
}