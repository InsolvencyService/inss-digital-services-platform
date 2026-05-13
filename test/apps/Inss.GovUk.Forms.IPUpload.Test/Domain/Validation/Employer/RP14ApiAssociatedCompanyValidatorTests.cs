using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiAssociatedCompanyValidatorTests
{
    private readonly RP14ApiAssociatedCompanyValidator _validator = new();
    
    [Fact]
    public void InvalidCompanyName_TestValidate_ReturnsInvalidResult()
    {
        RP14AssociatedCompaniesAssociatedCompany model = new()
        {
            CompanyName = new string('X', 61),
            ReasonForAssociation = "Non-exec Director"
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidAssociationReason_TestValidate_ReturnsInvalidResult()
    {
        RP14AssociatedCompaniesAssociatedCompany model = new()
        {
            CompanyName = "Simpsons Cleaning Services",
            ReasonForAssociation = new string('X', 256)
        };

        var result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidAssociatedCompany_TestValidate_ReturnsValidResult()
    {
        RP14AssociatedCompaniesAssociatedCompany model = new()
        {
            CompanyName = "Simpsons Cleaning Services",
            ReasonForAssociation = "Non-exec Director"
        };

        var result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}