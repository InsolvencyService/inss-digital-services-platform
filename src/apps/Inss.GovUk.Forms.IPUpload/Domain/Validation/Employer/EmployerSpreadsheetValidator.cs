using Inss.Common.IPUpload.Employer.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

internal sealed class EmployerSpreadsheetValidator : EmployerValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    internal EmployerSpreadsheetValidator(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }

    internal async Task<ValidatorContext> ValidateAsync(RP14 model)
    {
        EmployerValidatorContext context = new();

        bool caseReferenceExists = await _caseReferenceService.CheckExistsAsync(model.Header.CaseReference);

        if (!caseReferenceExists)
        {
            context.AddError(CaseValidationInfo.UnknownCaseReference(), model.Header.CaseReference);
        }

        ValidateBusinessName(context, model.NameOfBusiness);
        ValidateCompanyNumber(context, model.CompanyNumber);
        ValidateAddress(context, "Business", model.CompanyAddrLine1, model.CompanyAddrLine2, model.CompanyAddrLine3, 
            model.CompanyAddrTown, model.CompanyAddrCounty, model.CompanyAddrPostcode, model.CompanyAddrCountry);
        ValidateSICCode(context, model.SICCode);

        ValidateDirectorSurname(context, model.Directors.Director1.Director1Surname);
        ValidateDirectorInitials(context, model.Directors.Director1.Director1Initials);
        ValidateDirectorNino(context, model.Directors.Director1.Director1NINO);
        ValidateDirectorSurname(context, model.Directors.Director2.Director2Surname);
        ValidateDirectorInitials(context, model.Directors.Director2.Director2Initials);
        ValidateDirectorNino(context, model.Directors.Director2.Director2NINO);
        ValidateDirectorSurname(context, model.Directors.Director3.Director3Surname);
        ValidateDirectorInitials(context, model.Directors.Director3.Director3Initials);
        ValidateDirectorNino(context, model.Directors.Director3.Director3NINO);
        ValidateDirectorSurname(context, model.Directors.Director4.Director4Surname);
        ValidateDirectorInitials(context, model.Directors.Director4.Director4Initials);
        ValidateDirectorNino(context, model.Directors.Director4.Director4NINO);
        ValidateDirectorSurname(context, model.Directors.Director5.Director5Surname);
        ValidateDirectorInitials(context, model.Directors.Director5.Director5Initials);
        ValidateDirectorNino(context, model.Directors.Director5.Director5NINO);
        ValidateDirectorSurname(context, model.Directors.Director6.Director6Surname);
        ValidateDirectorInitials(context, model.Directors.Director6.Director6Initials);
        ValidateDirectorNino(context, model.Directors.Director6.Director6NINO);
        
        ValidateShareholderName(context, model.Shareholders.Shareholder1.Shareholder1FullName);
        ValidateShareholderPercentage(context, model.Shareholders.Shareholder1.Shareholder1Percentage);
        ValidateShareholderName(context, model.Shareholders.Shareholder2.Shareholder2FullName);
        ValidateShareholderPercentage(context, model.Shareholders.Shareholder2.Shareholder2Percentage);
        ValidateShareholderName(context, model.Shareholders.Shareholder3.Shareholder3FullName);
        ValidateShareholderPercentage(context, model.Shareholders.Shareholder3.Shareholder3Percentage);
        ValidateShareholderName(context, model.Shareholders.Shareholder4.Shareholder4FullName);
        ValidateShareholderPercentage(context, model.Shareholders.Shareholder4.Shareholder4Percentage);
        ValidateShareholderName(context, model.Shareholders.Shareholder5.Shareholder5FullName);
        ValidateShareholderPercentage(context, model.Shareholders.Shareholder5.Shareholder5Percentage);
        ValidateShareholderName(context, model.Shareholders.Shareholder6.Shareholder6FullName);
        ValidateShareholderPercentage(context, model.Shareholders.Shareholder6.Shareholder6Percentage);

        RP14AssociatedCompaniesAssociatedCompany1 associatedCompany1 = model.AssociatedCompanies.AssociatedCompany1;
        ValidateAssociatedCompanyName(context, associatedCompany1.AssocCompany1Name);
        ValidateAssociatedCompanyNumber(context, associatedCompany1.AssocCompany1Number);
        ValidateCompanyAssociationReason(context, associatedCompany1.AssocComp1ReasonForAssociation);
        ValidateAddress(context, "Associated company", associatedCompany1.AssocComp1AddrLine1, associatedCompany1.AssocComp1AddrLine2, 
            associatedCompany1.AssocComp1AddrLine3, associatedCompany1.AssocComp1AddrTown, associatedCompany1.AssocComp1AddrCounty, 
            associatedCompany1.AssocComp1AddrPostcode, associatedCompany1.AssocComp1AddrCountry);
        RP14AssociatedCompaniesAssociatedCompany2 associatedCompany2 = model.AssociatedCompanies.AssociatedCompany2;
        ValidateAssociatedCompanyName(context, associatedCompany2.AssocCompany2Name);
        ValidateAssociatedCompanyNumber(context, associatedCompany2.AssocCompany2Number);
        ValidateCompanyAssociationReason(context, associatedCompany2.AssocComp2ReasonForAssociation);
        ValidateAddress(context, "Associated company", associatedCompany2.AssocComp2AddrLine1, associatedCompany2.AssocComp2AddrLine2, 
            associatedCompany2.AssocComp2AddrLine3, associatedCompany2.AssocComp2AddrTown, associatedCompany2.AssocComp2AddrCounty, 
            associatedCompany2.AssocComp2AddrPostcode, associatedCompany2.AssocComp2AddrCountry);

        RP14EmployeesEmployeesClaimingContinuity employeeContinuity = model.Employees.EmployeesClaimingContinuity;
        ValidateContinuityEmployerName(context, employeeContinuity.EmployerName);
        ValidateAddress(context, "Employment continuity", employeeContinuity.EmployerAddrLine1, employeeContinuity.EmployerAddrLine2, 
            employeeContinuity.EmployerAddrLine3, employeeContinuity.EmployerAddrTown, employeeContinuity.EmployerAddrCounty,
            employeeContinuity.EmployerAddrPostcode, employeeContinuity.EmployerAddrCountry);
        
        RP14TransferDetailsTransferTo transferTo = model.TransferDetails.TransferTo;
        ValidateTransferToName(context, transferTo.Name);
        ValidateAddress(context, "Transfers", transferTo.TransferToAddrLine1, transferTo.TransferToAddrLine2, transferTo.TransferToAddrLine3, 
            transferTo.TransferToAddrTown, transferTo.TransferToAddrCounty, transferTo.TransferToAddrPostcode, transferTo.TransferToAddrCountry);

        RP14PayRecordsContact payRecordsContact = model.PayRecordsContact;
        ValidatePayRecordsContactName(context, payRecordsContact.Name);
        ValidatePayRecordsContactPhone(context, payRecordsContact.PayRecordsPhoneNumber);
        ValidatePayRecordsContactEmail(context, payRecordsContact.PayRecordsEmailAddress);
        ValidateAddress(context, "Pay records contact", payRecordsContact.PayRecordsAddrLine1, payRecordsContact.PayRecordsAddrLine2, 
            payRecordsContact.PayRecordsAddrLine3, payRecordsContact.PayRecordsAddrTown, payRecordsContact.PayRecordsAddrCounty, 
            payRecordsContact.PayRecordsAddrPostcode, payRecordsContact.PayRecordsAddrCountry);

        RP14InsolvencyPractitioner ip = model.InsolvencyPractitioner;
        ValidateIPRegistrationNumber(context, ip.IPRegistrationNumber);
        ValidateIPFirmName(context, ip.IPFirmName);
        ValidateIPName(context, ip.IPName);
        ValidateIPEmail(context, ip.IPEmailAddress);
        ValidateIPPhone(context, ip.IPTelephoneNumber);
        ValidateAddress(context, "Insolvency practitioner", ip.IPAddressLine1, ip.IPAddressLine2, ip.IPAddressLine3, 
            ip.IPAddressTown, ip.IPAddressCounty, ip.IPAddressPostcode, ip.IPAddressCountry);
        
        return context;
    }
}