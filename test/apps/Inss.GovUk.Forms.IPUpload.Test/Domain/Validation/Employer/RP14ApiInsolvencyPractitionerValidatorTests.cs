using FluentValidation.TestHelper;
using Inss.Common.IPUpload.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiInsolvencyPractitionerValidatorTests
{
    private readonly RP14ApiInsolvencyPractitionerValidator _validator = new();
    
    [Fact]
    public void InvalidRegNo_TestValidate_ReturnsInvalidResult()
    {
        RP14InsolvencyPractitioner model = new()
        {
            RegistrationNumber = new string('X', 10)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidFirmName_TestValidate_ReturnsInvalidResult()
    {
        RP14InsolvencyPractitioner model = new()
        {
            RegistrationNumber = "AB112233C",
            FirmName = new string('X', 256)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidIP_TestValidate_ReturnsValidResult()
    {
        RP14InsolvencyPractitioner model = new()
        {
            RegistrationNumber = "AB112233C",
            FirmName = "Springfield Insolvency"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}