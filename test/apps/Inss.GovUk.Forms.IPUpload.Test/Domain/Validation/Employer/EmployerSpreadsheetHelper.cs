using System.Globalization;
using Inss.Common.IPUpload.Employer.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

internal static class EmployerSpreadsheetHelper
{
    internal static RP14 CreateModel()
    {
        return new RP14
        {
            Header = new RP14Header
            {
                CaseReference = "CN12345678"
            },
            NameOfBusiness = "Springfield Nuclear",
            CompanyNumber = "11223344",
            NatureOfBusiness = "Energy production",
            CompanyAddrLine1 = "Springfield Nuclear House",
            CompanyAddrLine2 = "Nuclear Avenue",
            CompanyAddrTown = "Springfield",
            CompanyAddrPostcode = "TN33 0DN",
            CompanyAddrCounty = "Nevada",
            SICCode = "1234",
            Directors = new RP14Directors
            {
                Director1 = new RP14DirectorsDirector1
                {
                    Director1Initials = "M",
                    Director1Surname = "Burns",
                    Director1NINO = "PQ112233R"
                }
            },
            Shareholders = new RP14Shareholders
            {
                Shareholder1 = new RP14ShareholdersShareholder1
                {
                    Shareholder1FullName = "Montgomery Burns",
                    Shareholder1Percentage = 90.0M
                },
                Shareholder2 = new RP14ShareholdersShareholder2
                {
                    Shareholder2FullName = "Waylon Smithers",
                    Shareholder2Percentage = 10.0M
                }
            },
            AssociatedCompanies = new RP14AssociatedCompanies
            {
                AssociatedCompany1 = new RP14AssociatedCompaniesAssociatedCompany1
                {
                    AssocCompany1Name = "Spring Energy",
                    AssocCompany1Number = "12341234",
                    AssocComp1ReasonForAssociation = "Subsidiary",
                    AssocComp1AddrLine1 = "Springfield Nuclear House",
                    AssocComp1AddrLine2 = "Nuclear Avenue",
                    AssocComp1AddrTown = "Springfield",
                    AssocComp1AddrPostcode = "TN33 0DN",
                    AssocComp1AddrCounty = "Nevada"
                }
            },
            Employees = new RP14Employees
            {
                EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
                {
                    EmployerName = "Springfield Energy",
                    EmployerAddrLine1 = "Springfield Nuclear House",
                    EmployerAddrLine2 = "Nuclear Avenue",
                    EmployerAddrTown = "Springfield",
                    EmployerAddrPostcode = "TN33 0DN",
                    EmployerAddrCounty = "Nevada"
                }
            },
            TransferDetails = new RP14TransferDetails
            {
                TransferTo = new RP14TransferDetailsTransferTo
                {
                    Name = "Springfield Fusion",
                    TransferToAddrLine1 = "Springfield Nuclear House",
                    TransferToAddrLine2 = "Nuclear Avenue",
                    TransferToAddrTown = "Springfield",
                    TransferToAddrPostcode = "TN33 0DN",
                    TransferToAddrCounty = "Nevada"
                }
            }
        };
    }
    
    internal static void AssertError(List<Error> errors, string? value, ValidationInfo validationInfo)
    {
        EmployeeError? error = errors.Cast<EmployeeError>().FirstOrDefault(e => e.Value == value && e.Info.Key == validationInfo.Key);
        Assert.NotNull(error);
    }
}