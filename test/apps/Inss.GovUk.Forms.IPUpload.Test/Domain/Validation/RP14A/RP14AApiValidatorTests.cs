using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.RP14A;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.RP14A;

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
        IPUpload.Domain.Employee.Api.RP14A model = new()
        {
            EmployerName = "The Simpsons",
            Header = new RP14AHeader { CaseReference = invalidCaseReference }
        };
        
        TestValidationResult<IPUpload.Domain.Employee.Api.RP14A>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidEmployerName_TestValidate_ReturnsInvalidResult()
    {
        IPUpload.Domain.Employee.Api.RP14A model = new()
        {
            EmployerName = new string('X', 100),
            Header = new RP14AHeader { CaseReference = "CN12345678" }
        };
        
        TestValidationResult<IPUpload.Domain.Employee.Api.RP14A>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidHeaderDetails_TestValidate_ReturnsValidResult()
    {
        IPUpload.Domain.Employee.Api.RP14A model = new()
        {
            EmployerName = "The Simpsons",
            Header = new RP14AHeader { CaseReference = "CN12345678" }
        };
        
        TestValidationResult<IPUpload.Domain.Employee.Api.RP14A>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}