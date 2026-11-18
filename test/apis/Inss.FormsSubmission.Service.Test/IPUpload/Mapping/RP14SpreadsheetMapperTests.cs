using System.Text.Json;
using Inss.FormsSubmission.Service.IPUpload;
using Inss.FormsSubmission.Service.IPUpload.Employer;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using RP14Spreadsheet = Inss.Common.IPUpload.Employer.Spreadsheet.RP14;
using RP14SpreadsheetAssociatedCompanies = Inss.Common.IPUpload.Employer.Spreadsheet.RP14AssociatedCompanies;
using RP14SpreadsheetAssociatedCompany1 = Inss.Common.IPUpload.Employer.Spreadsheet.RP14AssociatedCompaniesAssociatedCompany1;
using RP14SpreadsheetDirectors = Inss.Common.IPUpload.Employer.Spreadsheet.RP14Directors;
using RP14SpreadsheetDirector1 = Inss.Common.IPUpload.Employer.Spreadsheet.RP14DirectorsDirector1;
using RP14SpreadsheetEmployees = Inss.Common.IPUpload.Employer.Spreadsheet.RP14Employees;
using RP14SpreadsheetEmployeesClaimingContinuity = Inss.Common.IPUpload.Employer.Spreadsheet.RP14EmployeesEmployeesClaimingContinuity;
using RP14SpreadsheetHeader = Inss.Common.IPUpload.Employer.Spreadsheet.RP14Header;
using Xunit;

namespace Inss.FormsSubmission.Service.Test.IPUpload.Mapping;

public sealed class RP14SpreadsheetMapperTests
{
    [Fact]
    public void SparseModel_Map_DoesNotThrowAndUsesNullGuardFallbacks()
    {
        RP14Spreadsheet model = BuildSparseModel();
        RP14SpreadsheetMapper sut = new(model);

        Exception? exception = Record.Exception(sut.Map);
        Assert.Null(exception);

        JsonMessage[] result = sut.Map();
        Assert.NotEmpty(result);

        CompanyInformation? mapped = JsonSerializer.Deserialize<CompanyInformation>(result[0].Json);

        Assert.NotNull(mapped);

        CompanyInformation mappedValue = mapped;

        Assert.Null(mappedValue.CaseReference);
        Assert.NotEmpty(mappedValue.Directors);
        Assert.Null(mappedValue.Directors[0].NationalInsuranceNumber);

        Assert.NotEmpty(mappedValue.AssociatedCompanies);
        Assert.Null(mappedValue.AssociatedCompanies[0].Name);
        Assert.Null(mappedValue.AssociatedCompanies[0].Number);
        Assert.Null(mappedValue.AssociatedCompanies[0].Address.Line1);

        Assert.NotNull(mappedValue.Employees);
        Assert.NotNull(mappedValue.Employees.PreviousEmployer);
        Assert.NotNull(mappedValue.Employees.PreviousEmployer.Address);
        Assert.Null(mappedValue.Employees.PreviousEmployer.Address.Line1);
    }

    private static RP14Spreadsheet BuildSparseModel()
    {
        return new RP14Spreadsheet
        {
            Header = new RP14SpreadsheetHeader(),
            NameOfBusiness = "Business",
            CompanyNumber = "123",
            Directors = new RP14SpreadsheetDirectors
            {
                Director1 = new RP14SpreadsheetDirector1
                {
                    Director1Surname = "Doe",
                    Director1Initials = "JD",
                    Director1NINO = null
                }
            },
            AssociatedCompanies = new RP14SpreadsheetAssociatedCompanies
            {
                AssociatedCompany1 = new RP14SpreadsheetAssociatedCompany1
                {
                    AssocCompany1Name = null,
                    AssocCompany1Number = null
                }
            },
            Employees = new RP14SpreadsheetEmployees
            {
                EmployeesClaimingContinuity = new RP14SpreadsheetEmployeesClaimingContinuity()
            }
        };
    }
}
