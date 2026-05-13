using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiPayRecordsContactValidatorTests
{
    private readonly RP14ApiPayRecordsContactValidator _validator = new();
    
    [Fact]
    public void InvalidContactName_TestValidate_ReturnsInvalidResult()
    {
        RP14PayRecordsContact model = new()
        {
            Name = new string('X', 61),
            PhoneNumber = "01234556677"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidContactPhone_TestValidate_ReturnsInvalidResult()
    {
        RP14PayRecordsContact model = new()
        {
            Name = "Lisa Simpson",
            PhoneNumber = new string('0', 13)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidPayRecordContact_TestValidate_ReturnsValidResult()
    {
        RP14PayRecordsContact model = new()
        {
            Name = "Lisa Simpson",
            PhoneNumber = "01234556677"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}