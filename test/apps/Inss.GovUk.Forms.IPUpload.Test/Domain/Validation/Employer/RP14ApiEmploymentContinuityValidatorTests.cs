using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiEmploymentContinuityValidatorTests
{
    private readonly RP14ApiEmploymentContinuityValidator _validator = new();
    
    [Fact]
    public void InvalidEmployerNameLength_TestValidate_ReturnsInvalidResult()
    {
        RP14EmployeesEmployeesClaimingContinuity model = new()
        {
            EmployerName = new string('X', 61)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidEmploymentContinuity_TestValidate_ReturnsValidResult()
    {
        RP14EmployeesEmployeesClaimingContinuity model = new()
        {
            EmployerName = "Springfield Nuclear"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    } 
}