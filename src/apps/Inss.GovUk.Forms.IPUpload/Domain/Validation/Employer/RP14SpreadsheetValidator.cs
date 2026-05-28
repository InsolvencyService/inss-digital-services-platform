using FluentValidation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class RP14SpreadsheetValidator : AbstractValidator<Inss.Common.IPUpload.Employer.Spreadsheet.RP14>
{
    public RP14SpreadsheetValidator()
    {
        RuleFor(p => p.Header.CaseReference).ValidateCaseReference();
        
        RuleFor(p => p.NameOfBusiness).ValidateBusinessName();
        RuleFor(p => p.CompanyNumber).ValidateCompanyNumber();
        RuleFor(p => p.CompanyAddrLine1).ValidateAddressLine("Business");
        RuleFor(p => p.CompanyAddrLine2).ValidateAddressLine("Business");
        RuleFor(p => p.CompanyAddrLine3).ValidateAddressLine("Business");
        RuleFor(p => p.CompanyAddrLine4).ValidateAddressLine("Business");
        RuleFor(p => p.CompanyAddrTown).ValidateAddressTown("Business");
        RuleFor(p => p.CompanyAddrCounty).ValidateAddressCounty("Business");
        RuleFor(p => p.CompanyAddrPostcode).ValidateAddressPostcode("Business");
        RuleFor(p => p.CompanyAddrCountry).ValidateAddressCountry("Business");
        RuleFor(p => p.SICCode).ValidateSIC();
        
        RuleFor(p => p.Directors.Director1.Director1Surname).ValidateDirectorSurname();
        RuleFor(p => p.Directors.Director1.Director1Initials).ValidateDirectorInitials();
        RuleFor(p => p.Directors.Director1.Director1NINO).ValidateDirectorNino();
        RuleFor(p => p.Directors.Director2.Director2Surname).ValidateDirectorSurname();
        RuleFor(p => p.Directors.Director2.Director2Initials).ValidateDirectorInitials();
        RuleFor(p => p.Directors.Director2.Director2NINO).ValidateDirectorNino();
        RuleFor(p => p.Directors.Director3.Director3Surname).ValidateDirectorSurname();
        RuleFor(p => p.Directors.Director3.Director3Initials).ValidateDirectorInitials();
        RuleFor(p => p.Directors.Director3.Director3NINO).ValidateDirectorNino();
        RuleFor(p => p.Directors.Director4.Director4Surname).ValidateDirectorSurname();
        RuleFor(p => p.Directors.Director4.Director4Initials).ValidateDirectorInitials();
        RuleFor(p => p.Directors.Director4.Director4NINO).ValidateDirectorNino();
        RuleFor(p => p.Directors.Director5.Director5Surname).ValidateDirectorSurname();
        RuleFor(p => p.Directors.Director5.Director5Initials).ValidateDirectorInitials();
        RuleFor(p => p.Directors.Director5.Director5NINO).ValidateDirectorNino();
        RuleFor(p => p.Directors.Director6.Director6Surname).ValidateDirectorSurname();
        RuleFor(p => p.Directors.Director6.Director6Initials).ValidateDirectorInitials();
        RuleFor(p => p.Directors.Director6.Director6NINO).ValidateDirectorNino();
        
        RuleFor(p => p.Shareholders.Shareholder1.Shareholder1FullName).ValidateShareholderName();
        RuleFor(p => p.Shareholders.Shareholder1.Shareholder1Percentage).ValidateShareholderPercentage();
        RuleFor(p => p.Shareholders.Shareholder2.Shareholder2FullName).ValidateShareholderName();
        RuleFor(p => p.Shareholders.Shareholder2.Shareholder2Percentage).ValidateShareholderPercentage();
        RuleFor(p => p.Shareholders.Shareholder3.Shareholder3FullName).ValidateShareholderName();
        RuleFor(p => p.Shareholders.Shareholder3.Shareholder3Percentage).ValidateShareholderPercentage();
        RuleFor(p => p.Shareholders.Shareholder4.Shareholder4FullName).ValidateShareholderName();
        RuleFor(p => p.Shareholders.Shareholder4.Shareholder4Percentage).ValidateShareholderPercentage();
        RuleFor(p => p.Shareholders.Shareholder5.Shareholder5FullName).ValidateShareholderName();
        RuleFor(p => p.Shareholders.Shareholder5.Shareholder5Percentage).ValidateShareholderPercentage();
        RuleFor(p => p.Shareholders.Shareholder6.Shareholder6FullName).ValidateShareholderName();
        RuleFor(p => p.Shareholders.Shareholder6.Shareholder6Percentage).ValidateShareholderPercentage();
        
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocCompany1Name).ValidateAssociatedCompanyName();
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocCompany1Number).ValidateAssociatedCompanyNumber();
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1ReasonForAssociation).ValidateAssociationReason();
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine1).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine2).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine3).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrLine4).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrTown).ValidateAddressTown("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrCounty).ValidateAddressCounty("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrPostcode).ValidateAddressPostcode("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany1.AssocComp1AddrCountry).ValidateAddressCountry("Associated company");
        
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocCompany2Name).ValidateAssociatedCompanyName();
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocCompany2Number).ValidateAssociatedCompanyNumber();
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2ReasonForAssociation).ValidateAssociationReason();
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrLine1).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrLine2).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrLine3).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrLine4).ValidateAddressLine("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrTown).ValidateAddressTown("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrCounty).ValidateAddressCounty("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrPostcode).ValidateAddressPostcode("Associated company");
        RuleFor(p => p.AssociatedCompanies.AssociatedCompany2.AssocComp2AddrCountry).ValidateAddressCountry("Associated company");
        
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerName).ValidateContinuityEmployerName();
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrLine1).ValidateAddressLine("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrLine2).ValidateAddressLine("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrLine3).ValidateAddressLine("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrLine4).ValidateAddressLine("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrTown).ValidateAddressTown("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrCounty).ValidateAddressCounty("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrPostcode).ValidateAddressPostcode("Employment continuity");
        RuleFor(p => p.Employees.EmployeesClaimingContinuity.EmployerAddrCountry).ValidateAddressCountry("Employment continuity");
        
        RuleFor(p => p.TransferDetails.TransferTo.Name).ValidateTransferToName();
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrLine1).ValidateAddressLine("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrLine2).ValidateAddressLine("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrLine3).ValidateAddressLine("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrLine4).ValidateAddressLine("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrTown).ValidateAddressTown("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrCounty).ValidateAddressCounty("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrPostcode).ValidateAddressPostcode("Transfers");
        RuleFor(p => p.TransferDetails.TransferTo.TransferToAddrCountry).ValidateAddressCountry("Transfers");
        
        RuleFor(p => p.PayRecordsContact.Name).ValidatePayRecordContactName();
        RuleFor(p => p.PayRecordsContact.PayRecordsPhoneNumber).ValidatePayRecordContactPhone();
        RuleFor(p => p.PayRecordsContact.PayRecordsEmailAddress).ValidatePayRecordContactEmail();
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrLine1).ValidateAddressLine("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrLine2).ValidateAddressLine("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrLine3).ValidateAddressLine("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrLine4).ValidateAddressLine("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrTown).ValidateAddressTown("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrCounty).ValidateAddressCounty("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrPostcode).ValidateAddressPostcode("Pay records contact");
        RuleFor(p => p.PayRecordsContact.PayRecordsAddrCountry).ValidateAddressCountry("Pay records contact");
        
        RuleFor(p => p.InsolvencyPractitioner.IPRegistrationNumber).ValidateIPRegistrationNumber();
        RuleFor(p => p.InsolvencyPractitioner.IPFirmName).ValidateIPFirmName();
        RuleFor(p => p.InsolvencyPractitioner.IPName).ValidateIPName();
        RuleFor(p => p.InsolvencyPractitioner.IPEmailAddress).ValidateIPEmail();
        RuleFor(p => p.InsolvencyPractitioner.IPTelephoneNumber).ValidateIPPhone();
        RuleFor(p => p.InsolvencyPractitioner.IPAddressLine1).ValidateAddressLine("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressLine2).ValidateAddressLine("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressLine3).ValidateAddressLine("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressLine4).ValidateAddressLine("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressTown).ValidateAddressTown("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressCounty).ValidateAddressCounty("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressPostcode).ValidateAddressPostcode("Insolvency practitioner");
        RuleFor(p => p.InsolvencyPractitioner.IPAddressCountry).ValidateAddressCountry("Insolvency practitioner");
    }
}