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
        ValidateAddress(context, "Business", _model.CompanyAddrLine1, _model.CompanyAddrLine2, _model.CompanyAddrLine3, 
            _model.CompanyAddrTown, _model.CompanyAddrCounty, _model.CompanyAddrPostcode, _model.CompanyAddrCountry);
        ValidateSICCode(context, _model.SICCode);

        ValidateDirectorSurname(context, _model.Directors.Director1.Director1Surname);
        ValidateDirectorInitials(context, _model.Directors.Director1.Director1Initials);
        ValidateDirectorNino(context, _model.Directors.Director1.Director1NINO);
        ValidateDirectorSurname(context, _model.Directors.Director2.Director2Surname);
        ValidateDirectorInitials(context, _model.Directors.Director2.Director2Initials);
        ValidateDirectorNino(context, _model.Directors.Director2.Director2NINO);
        ValidateDirectorSurname(context, _model.Directors.Director3.Director3Surname);
        ValidateDirectorInitials(context, _model.Directors.Director3.Director3Initials);
        ValidateDirectorNino(context, _model.Directors.Director3.Director3NINO);
        ValidateDirectorSurname(context, _model.Directors.Director4.Director4Surname);
        ValidateDirectorInitials(context, _model.Directors.Director4.Director4Initials);
        ValidateDirectorNino(context, _model.Directors.Director4.Director4NINO);
        ValidateDirectorSurname(context, _model.Directors.Director5.Director5Surname);
        ValidateDirectorInitials(context, _model.Directors.Director5.Director5Initials);
        ValidateDirectorNino(context, _model.Directors.Director5.Director5NINO);
        ValidateDirectorSurname(context, _model.Directors.Director6.Director6Surname);
        ValidateDirectorInitials(context, _model.Directors.Director6.Director6Initials);
        ValidateDirectorNino(context, _model.Directors.Director6.Director6NINO);
        
        ValidateShareholderName(context, _model.Shareholders.Shareholder1.Shareholder1FullName);
        ValidateShareholderPercentage(context, _model.Shareholders.Shareholder1.Shareholder1Percentage);
        ValidateShareholderName(context, _model.Shareholders.Shareholder2.Shareholder2FullName);
        ValidateShareholderPercentage(context, _model.Shareholders.Shareholder2.Shareholder2Percentage);
        ValidateShareholderName(context, _model.Shareholders.Shareholder3.Shareholder3FullName);
        ValidateShareholderPercentage(context, _model.Shareholders.Shareholder3.Shareholder3Percentage);
        ValidateShareholderName(context, _model.Shareholders.Shareholder4.Shareholder4FullName);
        ValidateShareholderPercentage(context, _model.Shareholders.Shareholder4.Shareholder4Percentage);
        ValidateShareholderName(context, _model.Shareholders.Shareholder5.Shareholder5FullName);
        ValidateShareholderPercentage(context, _model.Shareholders.Shareholder5.Shareholder5Percentage);
        ValidateShareholderName(context, _model.Shareholders.Shareholder6.Shareholder6FullName);
        ValidateShareholderPercentage(context, _model.Shareholders.Shareholder6.Shareholder6Percentage);

        RP14AssociatedCompaniesAssociatedCompany1 associatedCompany1 = _model.AssociatedCompanies.AssociatedCompany1;
        
        // Only check the associated company details if we have a name - the address line 1 is mandatory! 
        if (!string.IsNullOrWhiteSpace(associatedCompany1.AssocCompany1Name))
        {
            ValidateAssociatedCompanyName(context, associatedCompany1.AssocCompany1Name);
            ValidateAssociatedCompanyNumber(context, associatedCompany1.AssocCompany1Number);
            ValidateCompanyAssociationReason(context, associatedCompany1.AssocComp1ReasonForAssociation);
            ValidateAddress(context, "Associated company", associatedCompany1.AssocComp1AddrLine1, associatedCompany1.AssocComp1AddrLine2,
                associatedCompany1.AssocComp1AddrLine3, associatedCompany1.AssocComp1AddrTown, associatedCompany1.AssocComp1AddrCounty,
                associatedCompany1.AssocComp1AddrPostcode, associatedCompany1.AssocComp1AddrCountry);
        }

        RP14AssociatedCompaniesAssociatedCompany2 associatedCompany2 = _model.AssociatedCompanies.AssociatedCompany2;
        
        // Only check the associated company details if we have a name - the address line 1 is mandatory!
        if (!string.IsNullOrWhiteSpace(associatedCompany2.AssocCompany2Name))
        {
            ValidateAssociatedCompanyName(context, associatedCompany2.AssocCompany2Name);
            ValidateAssociatedCompanyNumber(context, associatedCompany2.AssocCompany2Number);
            ValidateCompanyAssociationReason(context, associatedCompany2.AssocComp2ReasonForAssociation);
            ValidateAddress(context, "Associated company", associatedCompany2.AssocComp2AddrLine1, associatedCompany2.AssocComp2AddrLine2,
                associatedCompany2.AssocComp2AddrLine3, associatedCompany2.AssocComp2AddrTown, associatedCompany2.AssocComp2AddrCounty,
                associatedCompany2.AssocComp2AddrPostcode, associatedCompany2.AssocComp2AddrCountry);
        }

        RP14EmployeesEmployeesClaimingContinuity employeeContinuity = _model.Employees.EmployeesClaimingContinuity;
        ValidateContinuityEmployerName(context, employeeContinuity.EmployerName);
        ValidateAddress(context, "Employment continuity", employeeContinuity.EmployerAddrLine1, employeeContinuity.EmployerAddrLine2, 
            employeeContinuity.EmployerAddrLine3, employeeContinuity.EmployerAddrTown, employeeContinuity.EmployerAddrCounty,
            employeeContinuity.EmployerAddrPostcode, employeeContinuity.EmployerAddrCountry);
        
        RP14TransferDetailsTransferTo transferTo = _model.TransferDetails.TransferTo;

        // Only validate the transfer to if we have the name set. The address line 1 is mandatory!
        if (!string.IsNullOrWhiteSpace(transferTo.Name))
        {
            ValidateTransferToName(context, transferTo.Name);
            ValidateAddress(context, "Transfers", transferTo.TransferToAddrLine1, transferTo.TransferToAddrLine2,
                transferTo.TransferToAddrLine3,
                transferTo.TransferToAddrTown, transferTo.TransferToAddrCounty, transferTo.TransferToAddrPostcode,
                transferTo.TransferToAddrCountry);
        }

        RP14PayRecordsContact payRecordsContact = _model.PayRecordsContact;
        ValidatePayRecordsContactName(context, payRecordsContact.Name);
        ValidatePayRecordsContactPhone(context, payRecordsContact.PayRecordsPhoneNumber);
        ValidatePayRecordsContactEmail(context, payRecordsContact.PayRecordsEmailAddress);
        ValidateAddress(context, "Pay records contact", payRecordsContact.PayRecordsAddrLine1, payRecordsContact.PayRecordsAddrLine2, 
            payRecordsContact.PayRecordsAddrLine3, payRecordsContact.PayRecordsAddrTown, payRecordsContact.PayRecordsAddrCounty, 
            payRecordsContact.PayRecordsAddrPostcode, payRecordsContact.PayRecordsAddrCountry);

        RP14InsolvencyPractitioner ip = _model.InsolvencyPractitioner;
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

        return context;
    }
}