using Inss.Common.IPUpload.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Xunit;

namespace Inss.GovUk.Forms.IPUpload.Test.Domain.Validation.Employer;

internal static class EmployerApiHelper
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
            Address = new AddressType
            {
                Line = ["Springfield Nuclear House", "Nuclear Avenue"],
                Town = "Springfield",
                Postcode = "TN33 0DN",
                County = "Nevada"
            },
            SICCode = "1234",
            Directors = new RP14Directors
            {
                Director = [
                    new RP14DirectorsDirector
                    {
                        Name = new NameType
                        {
                            Initials = "M",
                            Surname ="Burns" 
                        },
                        NINO = "AB112233C"
                    }
                ]
            },
            Shareholders = [
                new RP14Shareholder
                {
                    Name = new NameType
                    {
                        FullName = "Montgomery Burns"
                    },
                    Percentage = 90.0M
                },
                new RP14Shareholder
                {
                    Name = new NameType
                    {
                        FullName = "Waylon Smithers"
                    },
                    Percentage = 10.0M
                }
            ],
            AssociatedCompanies = new RP14AssociatedCompanies
            {
                AssociatedCompany = [
                    new RP14AssociatedCompaniesAssociatedCompany
                    {
                        CompanyName = "Spring Energy",
                        CompanyNumber = "12341234",
                        ReasonForAssociation = "Subsidiary",
                        Address = new AddressType
                        {
                            Line = ["Springfield Nuclear House", "Nuclear Avenue"],
                            Town = "Springfield",
                            Postcode = "TN33 0DN",
                            County = "Nevada"
                        }
                    }
                ]
            },
            Employees = new RP14Employees
            {
                EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
                {
                    EmployerName = "Springfield Energy",
                    Address = new AddressType
                    {
                        Line = ["Springfield Nuclear House", "Nuclear Avenue"],
                        Town = "Springfield",
                        Postcode = "TN33 0DN",
                        County = "Nevada"
                    }
                }
            },
            TransferDetails = new RP14TransferDetails
            {
                TransferTo = new RP14TransferDetailsTransferTo
                {
                    Name = "Springfield Fusion",
                    Address = new AddressType
                    {
                        Line = ["Springfield Nuclear House", "Nuclear Avenue"],
                        Town = "Springfield",
                        Postcode = "TN33 0DN",
                        County = "Nevada"
                    }
                }
            },
            PayRecordsContact = new RP14PayRecordsContact
            {
                Name = "Marge Simpson",
                EmailAddress = "marge.simpson@springfield.nuclear",
                PhoneNumber = "01234556677",
                Address = new AddressType
                {
                    Line = ["Springfield Nuclear House", "Nuclear Avenue"],
                    Town = "Springfield",
                    Postcode = "TN33 0DN",
                    County = "Nevada"
                }
            },
            InsolvencyPractitioner = new RP14InsolvencyPractitioner
            {
                RegistrationNumber = "123456789",
                FirmName = "Springfield Insolvency",
                Name = "Ned Flanders",
                EmailAddress = "ned.flanders@springfield.insolvency",
                TelephoneNumber = "01234112233",
                Address = new AddressType
                {
                    Line = ["Springfield Insolvency House", "Defunct Avenue"],
                    Town = "Springfield",
                    Postcode = "TN33 0DN",
                    County = "Nevada"
                }
            }
        };
    }
    
    internal static void AssertError(List<Error> errors, ValidationInfo validationInfo)
    {
        EmployerError? error = errors.Cast<EmployerError>().FirstOrDefault(e => e.Info.Key == validationInfo.Key);
        Assert.NotNull(error);
    }
}