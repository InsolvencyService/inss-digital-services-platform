using FluentValidation.TestHelper;
using Inss.GovUk.Forms.IPUpload.Domain.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

public class RP14ApiValidatorTests
{
    private readonly RP14ApiValidator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void InvalidBusinessName_TestValidate_ReturnsInvalidResult(string? invalidBusinessName)
    {
        RP14 model = new()
        {
            NameOfBusiness = invalidBusinessName,
            NatureOfBusiness = "Nuclear Power Enrichment",
            CompanyNumber = "12345678",
            SICCode = "62020",
            Directors = new RP14Directors
            {
                Director = [
                    new RP14DirectorsDirector { NINO = "AB112233C" }
                ]
            },
            Shareholders =
            [
                new RP14Shareholder
                {
                    Percentage = 50.00M,
                    Name = new NameType{ FullName = "Ned Flanders" }
                }
            ],
            AssociatedCompanies = new RP14AssociatedCompanies
            {
                AssociatedCompany = [
                    new RP14AssociatedCompaniesAssociatedCompany
                    {
                        CompanyName = "Simpsons Cleaning Services",
                        ReasonForAssociation = "Non-exec Director"
                    }
                ]
            },
            Employees = new RP14Employees
            {
                EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
                {
                    EmployerName = "Springfield Nuclear"
                }
            },
            TransferDetails = new RP14TransferDetails
            {
                TransferTo = new RP14TransferDetailsTransferTo
                {
                    Name = "Marge Simpson"
                }
            },
            PayRecordsContact = new RP14PayRecordsContact
            {
                Name = "Lisa Simpson",
                PhoneNumber = "01234556677"
            }
        };

        TestValidationResult<RP14>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void InvalidNatureOfBusiness_TestValidate_ReturnsInvalidResult()
    {
        RP14 model = new()
        {
            NameOfBusiness = "Simpsons Tech Services",
            NatureOfBusiness = new string('X', 101),
            CompanyNumber = "12345678",
            SICCode = "62020",
            Directors = new RP14Directors
            {
                Director = [
                    new RP14DirectorsDirector { NINO = "AB112233C" }
                ]
            },
            Shareholders =
            [
                new RP14Shareholder
                {
                    Percentage = 50.00M,
                    Name = new NameType{ FullName = "Ned Flanders" }
                }
            ],
            AssociatedCompanies = new RP14AssociatedCompanies
            {
                AssociatedCompany = [
                    new RP14AssociatedCompaniesAssociatedCompany
                    {
                        CompanyName = "Simpsons Cleaning Services",
                        ReasonForAssociation = "Non-exec Director"
                    }
                ]
            },
            Employees = new RP14Employees
            {
                EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
                {
                    EmployerName = "Springfield Nuclear"
                }
            },
            TransferDetails = new RP14TransferDetails
            {
                TransferTo = new RP14TransferDetailsTransferTo
                {
                    Name = "Marge Simpson"
                }
            },
            PayRecordsContact = new RP14PayRecordsContact
            {
                Name = "Lisa Simpson",
                PhoneNumber = "01234556677"
            }
        };

        TestValidationResult<RP14>? result = _validator.TestValidate(model);
        
        Assert.False(result.IsValid);
    }
    
    [Fact]
    public void ValidBusinessDetails_TestValidate_ReturnsValidResult()
    {
        RP14 model = new()
        {
            NameOfBusiness = "Simpsons Tech Services",
            NatureOfBusiness = "Nuclear Power Enrichment",
            CompanyNumber = "12345678",
            SICCode = "62020",
            Directors = new RP14Directors
            {
                Director = [
                    new RP14DirectorsDirector { NINO = "AB112233C" }
                ]
            },
            Shareholders =
            [
                new RP14Shareholder
                {
                    Percentage = 50.00M,
                    Name = new NameType{ FullName = "Ned Flanders" }
                }
            ],
            AssociatedCompanies = new RP14AssociatedCompanies
            {
                AssociatedCompany = [
                    new RP14AssociatedCompaniesAssociatedCompany
                    {
                        CompanyName = "Simpsons Cleaning Services",
                        ReasonForAssociation = "Non-exec Director"
                    }
                ]
            },
            Employees = new RP14Employees
            {
                EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
                {
                    EmployerName = "Springfield Nuclear"
                }
            },
            TransferDetails = new RP14TransferDetails
            {
                TransferTo = new RP14TransferDetailsTransferTo
                {
                    Name = "Marge Simpson"
                }
            },
            PayRecordsContact = new RP14PayRecordsContact
            {
                Name = "Lisa Simpson",
                PhoneNumber = "01234556677"
            }
        };

        TestValidationResult<RP14>? result = _validator.TestValidate(model);
        
        Assert.True(result.IsValid);
    }
}