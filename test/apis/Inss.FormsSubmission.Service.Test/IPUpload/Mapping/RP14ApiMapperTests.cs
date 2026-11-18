using System.Text.Json;
using Inss.Common.IPUpload.Employer.Api;
using Inss.FormsSubmission.Service.IPUpload;
using Inss.FormsSubmission.Service.IPUpload.Employer;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Xunit;

namespace Inss.FormsSubmission.Service.Test.IPUpload.Mapping;

public sealed class RP14ApiMapperTests
{
    [Fact]
    public void SparseModel_Map_DoesNotThrowAndUsesNullGuardFallbacks()
    {
        RP14 model = BuildSparseModel();
        RP14ApiMapper sut = new(model);

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
        Assert.Null(mappedValue.AssociatedCompanies[0].Address);
        Assert.Equal(string.Empty, mappedValue.AssociatedCompanies[0].Name);
        Assert.Null(mappedValue.AssociatedCompanies[0].Number);

        Assert.NotNull(mappedValue.Employees);
        Assert.NotNull(mappedValue.Employees.PreviousEmployer);
        Assert.Null(mappedValue.Employees.PreviousEmployer.Address);
    }

    private static RP14 BuildSparseModel()
    {
        return new RP14
        {
            Header = new RP14Header(),
            NameOfBusiness = "Business",
            CompanyNumber = "123",
            Address = new AddressType { Line = [] },
            Directors = new RP14Directors
            {
                Director =
                [
                    new RP14DirectorsDirector
                    {
                        Name = new NameType { Surname = "Doe", Initials = "JD" },
                        NINO = null
                    }
                ]
            },
            AssociatedCompanies = new RP14AssociatedCompanies
            {
                AssociatedCompany =
                [
                    new RP14AssociatedCompaniesAssociatedCompany
                    {
                        CompanyName = string.Empty,
                        Address = null
                    }
                ]
            },
            Employees = new RP14Employees
            {
                EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
                {
                    EmployerName = string.Empty,
                    Address = null
                }
            }
        };
    }
}
