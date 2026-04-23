using System.ComponentModel.DataAnnotations;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Attributes;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Attributes;

public class StringLengthPropertyAttributeTests
{
    private readonly ValidationContext _validationContext = new(Value);
    private const string Value = "This is a test";
    
    [Fact]
    public void FromKey_Validate_AssignsErrorKeyToErrorMessage()
    {
        StringLengthPropertyAttribute attribute = new("TestKey", 20);
        
        attribute.Validate(Value, _validationContext);
        
        Assert.Equal(attribute.Key, attribute.ErrorMessage);
    }
}