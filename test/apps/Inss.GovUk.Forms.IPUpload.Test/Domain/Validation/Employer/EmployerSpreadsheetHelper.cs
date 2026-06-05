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
                    Director1NINO = "AB112233C"
                },
                Director2 = new RP14DirectorsDirector2(),
                Director3 = new RP14DirectorsDirector3(),
                Director4 = new RP14DirectorsDirector4(),
                Director5 = new RP14DirectorsDirector5(),
                Director6 = new RP14DirectorsDirector6()
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
                },
                Shareholder3 = new RP14ShareholdersShareholder3(),
                Shareholder4 = new RP14ShareholdersShareholder4(),
                Shareholder5 = new RP14ShareholdersShareholder5(),
                Shareholder6 = new RP14ShareholdersShareholder6()
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
                },
                AssociatedCompany2 = new RP14AssociatedCompaniesAssociatedCompany2()
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
            },
            PayRecordsContact = new RP14PayRecordsContact
            {
                Name = "Marge Simpson",
                PayRecordsEmailAddress = "marge.simpson@springfield.nuclear",
                PayRecordsPhoneNumber = "01234556677",
                PayRecordsAddrLine1 = "Springfield Nuclear House",
                PayRecordsAddrLine2 = "Nuclear Avenue",
                PayRecordsAddrTown = "Springfield",
                PayRecordsAddrPostcode = "TN33 0DN",
                PayRecordsAddrCounty = "Nevada"
            },
            InsolvencyPractitioner = new RP14InsolvencyPractitioner
            {
                IPRegistrationNumber = "123456789",
                IPFirmName = "Springfield Insolvency",
                IPName = "Ned Flanders",
                IPEmailAddress = "ned.flanders@springfield.insolvency",
                IPTelephoneNumber = "01234112233",
                IPAddressLine1 = "Springfield Insolvency House",
                IPAddressLine2 = "Defunct Avenue",
                IPAddressTown = "Springfield",
                IPAddressPostcode = "TN33 0DN",
                IPAddressCounty = "Nevada"
            }
        };
    }
    
    internal static void AssertError(List<Error> errors, ValidationInfo validationInfo)
    {
        EmployerError? error = errors.Cast<EmployerError>().FirstOrDefault(e => e.Info.Key == validationInfo.Key);
        Assert.NotNull(error);
    }
}