using Inss.Common.IPUpload.Employer.Spreadsheet;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class EmployerSpreadsheetValidator : EmployerValidator
{
    private readonly RP14 _model;

    public EmployerSpreadsheetValidator(RP14 model)
    {
        _model = model;
    }

    public override ValidatorContext Validate(EmployerDetailsModel employerDetails)
    {
        EmployerValidatorContext context = new();

        ValidateCaseReference(context, _model.Header.CaseReference, employerDetails.CaseReference);
        ValidateBusinessName(context, _model.NameOfBusiness);
        ValidateCompanyNumber(context, _model.CompanyNumber);
        ValidateNatureOfBusiness(context, _model.NatureOfBusiness);
        ValidatePayeReference(context, _model.PAYE?.District, _model.PAYE?.Reference);
        ValidateAddress(context, "Business", _model.CompanyAddrLine1, _model.CompanyAddrLine2, _model.CompanyAddrLine3, 
            _model.CompanyAddrTown, _model.CompanyAddrCounty, _model.CompanyAddrPostcode, _model.CompanyAddrCountry);
        ValidateSICCode(context, _model.SICCode);
        ValidateDirectors(context, _model.Directors);
        ValidateShareholders(context, _model.Shareholders);
        ValidateAssociatedCompanies(context, _model.AssociatedCompanies);
        ValidateEmployees(context, _model.Employees);
        ValidateTransferDetails(context, _model.TransferDetails);
        ValidatePayRecordsContact(context, _model.PayRecordsContact);
        ValidateInsolvencyPractitioner(context, _model.InsolvencyPractitioner);

        return context;
    }

    private static void ValidateDirectors(EmployerValidatorContext context, RP14Directors? directors)
    {
        if (directors is not null)
        {
            if (directors.Director1 is not null)
            {
                ValidateDirectorSurname(context, directors.Director1.Director1Surname);
                ValidateDirectorInitials(context, directors.Director1.Director1Initials);
                ValidateDirectorNino(context, directors.Director1.Director1NINO);
            }

            if (directors.Director2 is not null)
            {
                ValidateDirectorSurname(context, directors.Director2.Director2Surname);
                ValidateDirectorInitials(context, directors.Director2.Director2Initials);
                ValidateDirectorNino(context, directors.Director2.Director2NINO);
            }

            if (directors.Director3 is not null)
            {
                ValidateDirectorSurname(context, directors.Director3.Director3Surname);
                ValidateDirectorInitials(context, directors.Director3.Director3Initials);
                ValidateDirectorNino(context, directors.Director3.Director3NINO);
            }

            if (directors.Director4 is not null)
            {
                ValidateDirectorSurname(context, directors.Director4.Director4Surname);
                ValidateDirectorInitials(context, directors.Director4.Director4Initials);
                ValidateDirectorNino(context, directors.Director4.Director4NINO);
            }

            if (directors.Director5 is not null)
            {
                ValidateDirectorSurname(context, directors.Director5.Director5Surname);
                ValidateDirectorInitials(context, directors.Director5.Director5Initials);
                ValidateDirectorNino(context, directors.Director5.Director5NINO);
            }

            if (directors.Director6 is not null)
            {
                ValidateDirectorSurname(context, directors.Director6.Director6Surname);
                ValidateDirectorInitials(context, directors.Director6.Director6Initials);
                ValidateDirectorNino(context, directors.Director6.Director6NINO);
            }
        }
    }

    private static void ValidateShareholders(EmployerValidatorContext context, RP14Shareholders? shareholders)
    {
        if (shareholders is not null)
        {
            if (shareholders.Shareholder1 is not null)
            {
                ValidateShareholderName(context, shareholders.Shareholder1.Shareholder1FullName);
                ValidateShareholderPercentage(context, shareholders.Shareholder1.Shareholder1Percentage);
            }

            if (shareholders.Shareholder2 is not null)
            {
                ValidateShareholderName(context, shareholders.Shareholder2.Shareholder2FullName);
                ValidateShareholderPercentage(context, shareholders.Shareholder2.Shareholder2Percentage);
            }

            if (shareholders.Shareholder3 is not null)
            {
                ValidateShareholderName(context, shareholders.Shareholder3.Shareholder3FullName);
                ValidateShareholderPercentage(context, shareholders.Shareholder3.Shareholder3Percentage);
            }

            if (shareholders.Shareholder4 is not null)
            {
                ValidateShareholderName(context, shareholders.Shareholder4.Shareholder4FullName);
                ValidateShareholderPercentage(context, shareholders.Shareholder4.Shareholder4Percentage);
            }

            if (shareholders.Shareholder5 is not null)
            {
                ValidateShareholderName(context, shareholders.Shareholder5.Shareholder5FullName);
                ValidateShareholderPercentage(context, shareholders.Shareholder5.Shareholder5Percentage);
            }

            if (shareholders.Shareholder6 is not null)
            {
                ValidateShareholderName(context, shareholders.Shareholder6.Shareholder6FullName);
                ValidateShareholderPercentage(context, shareholders.Shareholder6.Shareholder6Percentage);
            }
        }
    }

    private static void ValidateAssociatedCompanies(EmployerValidatorContext context, RP14AssociatedCompanies? associatedCompanies)
    {
        if (associatedCompanies is not null && associatedCompanies.LegallyAssociatedCompanies == YesNoType.yes)
        {
            if (associatedCompanies.AssociatedCompany1 is not null)
            {
                RP14AssociatedCompaniesAssociatedCompany1 associatedCompany1 = associatedCompanies.AssociatedCompany1;
                
                // Only check the associated company details if we have a name - the address line 1 is mandatory! 
                if (!string.IsNullOrWhiteSpace(associatedCompany1.AssocCompany1Name))
                {
                    ValidateAssociatedCompanyName(context, associatedCompany1.AssocCompany1Name);
                    ValidateAssociatedCompanyNumber(context, associatedCompany1.AssocCompany1Number);
                    ValidateCompanyAssociationReason(context, associatedCompany1.AssocComp1ReasonForAssociation);
                    ValidateAddress(context, "Associated company", associatedCompany1.AssocComp1AddrLine1,
                        associatedCompany1.AssocComp1AddrLine2,
                        associatedCompany1.AssocComp1AddrLine3, associatedCompany1.AssocComp1AddrTown,
                        associatedCompany1.AssocComp1AddrCounty,
                        associatedCompany1.AssocComp1AddrPostcode, associatedCompany1.AssocComp1AddrCountry);
                }
            }

            if (associatedCompanies.AssociatedCompany2 is not null)
            {
                RP14AssociatedCompaniesAssociatedCompany2 associatedCompany2 = associatedCompanies.AssociatedCompany2;

                // Only check the associated company details if we have a name - the address line 1 is mandatory!
                if (!string.IsNullOrWhiteSpace(associatedCompany2.AssocCompany2Name))
                {
                    ValidateAssociatedCompanyName(context, associatedCompany2.AssocCompany2Name);
                    ValidateAssociatedCompanyNumber(context, associatedCompany2.AssocCompany2Number);
                    ValidateCompanyAssociationReason(context, associatedCompany2.AssocComp2ReasonForAssociation);
                    ValidateAddress(context, "Associated company", associatedCompany2.AssocComp2AddrLine1,
                        associatedCompany2.AssocComp2AddrLine2,
                        associatedCompany2.AssocComp2AddrLine3, associatedCompany2.AssocComp2AddrTown,
                        associatedCompany2.AssocComp2AddrCounty,
                        associatedCompany2.AssocComp2AddrPostcode, associatedCompany2.AssocComp2AddrCountry);
                }
            }
        }
    }

    private static void ValidateEmployees(EmployerValidatorContext context, RP14Employees? employees)
    {
        if (employees is not null)
        {
            ValidateNumberOfEmployees(context, employees.NoOfEmployees);

            if (employees.EmployeesClaimingContinuity is not null)
            {
                RP14EmployeesEmployeesClaimingContinuity employeeContinuity = employees.EmployeesClaimingContinuity;

                if (employeeContinuity.ClaimingContinuity == YesNoType.no)
                {
                    return;
                }
                
                ValidateContinuityEmployerName(context, employeeContinuity.EmployerName);
                ValidateAddress(context, "Employment continuity", employeeContinuity.EmployerAddrLine1,
                    employeeContinuity.EmployerAddrLine2,
                    employeeContinuity.EmployerAddrLine3, employeeContinuity.EmployerAddrTown, employeeContinuity.EmployerAddrCounty,
                    employeeContinuity.EmployerAddrPostcode, employeeContinuity.EmployerAddrCountry);
            }
        }
    }

    private static void ValidateTransferDetails(EmployerValidatorContext context, RP14TransferDetails? transferDetails)
    {
        if (transferDetails?.TransferTo is not null)
        {
            RP14TransferDetailsTransferTo transferTo = transferDetails.TransferTo;

            // Only validate the transfer to if we have the name set. The address line 1 is mandatory!
            if (!string.IsNullOrWhiteSpace(transferTo.Name))
            {
                ValidateTransferToName(context, transferTo.Name);
                ValidateAddress(context, "Transfers", transferTo.TransferToAddrLine1, transferTo.TransferToAddrLine2,
                    transferTo.TransferToAddrLine3,
                    transferTo.TransferToAddrTown, transferTo.TransferToAddrCounty, transferTo.TransferToAddrPostcode,
                    transferTo.TransferToAddrCountry);
            }
        }
    }

    private static void ValidatePayRecordsContact(EmployerValidatorContext context, RP14PayRecordsContact? payRecordsContact)
    {
        if (payRecordsContact is not null)
        {
            ValidatePayRecordsContactName(context, payRecordsContact.Name);
            ValidatePayRecordsContactPhone(context, payRecordsContact.PayRecordsPhoneNumber);
            ValidatePayRecordsContactEmail(context, payRecordsContact.PayRecordsEmailAddress);
            ValidateAddress(context, "Pay records contact", payRecordsContact.PayRecordsAddrLine1, payRecordsContact.PayRecordsAddrLine2,
                payRecordsContact.PayRecordsAddrLine3, payRecordsContact.PayRecordsAddrTown, payRecordsContact.PayRecordsAddrCounty,
                payRecordsContact.PayRecordsAddrPostcode, payRecordsContact.PayRecordsAddrCountry);
        }
    }

    private static void ValidateInsolvencyPractitioner(EmployerValidatorContext context, RP14InsolvencyPractitioner? ip)
    {
        if (ip is not null)
        {
            ValidateIPRegistrationNumber(context, ip.IPRegistrationNumber);
            ValidateIPFirmName(context, ip.IPFirmName);
            ValidateIPName(context, ip.IPName);
            ValidateIPEmail(context, ip.IPEmailAddress);
            ValidateIPPhone(context, ip.IPTelephoneNumber);

            // Only validate the IP address if the address line 1 is set as its optional in this case but mandatory in all other cases!
            if (!string.IsNullOrWhiteSpace(ip.IPAddressLine1))
            {
                ValidateAddress(context, "Insolvency practitioner", ip.IPAddressLine1, ip.IPAddressLine2, ip.IPAddressLine3,
                    ip.IPAddressTown, ip.IPAddressCounty, ip.IPAddressPostcode, ip.IPAddressCountry);
            }
        }
    }
}