using FluentValidation.TestHelper;
using Inss.Common.IPUpload.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiShareholderValidatorTests
{
    private readonly RP14ApiShareholderValidator _validator = new();

    [Fact]
    public void InvalidShareholderPercentFormat_TestValidate_ReturnsInvalidResult()
    {
        RP14Shareholder model = new()
        {
            Percentage = 50.123M,
            Name = new NameType
            {
                FullName = "Ned Flanders"
            }
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidShareholderName_TestValidate_ReturnsInvalidResult()
    {
        RP14Shareholder model = new()
        {
            Percentage = 50.123M,
            Name = new NameType
            {
                FullName = new string('X', 101)
            }
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidShareholder_TestValidate_ReturnsValidResult()
    {
        RP14Shareholder model = new()
        {
            Percentage = 50.00M,
            Name = new NameType
            {
                FullName = "Ned Flanders"
            }
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}