using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiTransferToValidatorTests
{
    private readonly RP14ApiTransferToValidator _validator = new();
    
    [Fact]
    public void InvalidTransferName_TestValidate_ReturnsInvalidResult()
    {
        RP14TransferDetailsTransferTo model = new()
        {
            Name = new string('X', 61)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidTransferTo_TestValidate_ReturnsValidResult()
    {
        RP14TransferDetailsTransferTo model = new()
        {
            Name = "Marge Simpson"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}