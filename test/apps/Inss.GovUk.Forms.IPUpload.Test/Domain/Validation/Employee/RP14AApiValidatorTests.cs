using FluentValidation.TestHelper;
using Inss.Common.IPUpload.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employee;

public class RP14AApiValidatorTests
{
    private readonly RP14AApiValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("XY12345678")]
    public void InvalidCaseReference_TestValidate_ReturnsInvalidResult(string? invalidCaseReference)
    {
        RP14A model = new()
        {
            EmployerName = "The Simpsons",
            Header = new RP14AHeader { CaseReference = invalidCaseReference }
        };
        
        TestValidationResult<RP14A>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidEmployerName_TestValidate_ReturnsInvalidResult()
    {
        RP14A model = new()
        {
            EmployerName = new string('X', 100),
            Header = new RP14AHeader { CaseReference = "CN12345678" }
        };
        
        TestValidationResult<RP14A>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidHeaderDetails_TestValidate_ReturnsValidResult()
    {
        RP14A model = new()
        {
            EmployerName = "The Simpsons",
            Header = new RP14AHeader { CaseReference = "CN12345678" }
        };
        
        TestValidationResult<RP14A>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}