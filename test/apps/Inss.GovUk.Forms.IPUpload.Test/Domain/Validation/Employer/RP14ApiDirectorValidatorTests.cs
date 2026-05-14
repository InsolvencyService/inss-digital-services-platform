using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiDirectorValidatorTests
{
    private readonly RP14ApiDirectorValidator _validator = new();

    [Fact]
    public void InvalidNino_TestValidate_ReturnsInvalidResult()
    {
        RP14DirectorsDirector model = new()
        {
            NINO = "XX112233YY"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidDirector_TestValidate_ReturnsValidResult()
    {
        RP14DirectorsDirector model = new()
        {
            NINO = "AB112233C"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}