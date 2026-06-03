using Inss.Common.IPUpload.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

internal sealed class EmployerApiValidator : EmployerValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    internal EmployerApiValidator(ICaseReferenceService caseReferenceService)
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
        ValidateAddress(context, "Business", model.Address);
        ValidateSICCode(context, model.SICCode);
        
        foreach (RP14DirectorsDirector director in model.Directors.Director)
        {
            ValidateDirectorSurname(context, director.Name.Surname);
            ValidateDirectorInitials(context, director.Name.Initials);
            ValidateDirectorNino(context, director.NINO);
        }

        foreach (RP14Shareholder shareholder in model.Shareholders)
        {
            ValidateShareholderName(context, shareholder.Name.FullName);
            ValidateShareholderPercentage(context,shareholder.Percentage);
        }

        foreach (RP14AssociatedCompaniesAssociatedCompany associatedCompany in model.AssociatedCompanies.AssociatedCompany)
        {
            ValidateAssociatedCompanyName(context, associatedCompany.CompanyName);
            ValidateAssociatedCompanyNumber(context, associatedCompany.CompanyNumber);
            ValidateCompanyAssociationReason(context, associatedCompany.ReasonForAssociation);
            ValidateAddress(context, "Associated company", associatedCompany.Address);
        }

        RP14EmployeesEmployeesClaimingContinuity employeeContinuity = model.Employees.EmployeesClaimingContinuity;
        ValidateContinuityEmployerName(context, employeeContinuity.EmployerName);
        ValidateAddress(context, "Employment continuity", employeeContinuity.Address);
        
        RP14TransferDetailsTransferTo transferTo = model.TransferDetails.TransferTo;
        ValidateTransferToName(context, transferTo.Name);
        ValidateAddress(context, "Transfers", transferTo.Address);

        RP14PayRecordsContact payRecordsContact = model.PayRecordsContact;
        ValidatePayRecordsContactName(context, payRecordsContact.Name);
        ValidatePayRecordsContactPhone(context, payRecordsContact.PhoneNumber);
        ValidatePayRecordsContactEmail(context, payRecordsContact.EmailAddress);
        ValidateAddress(context, "Pay records contact", payRecordsContact.Address);

        RP14InsolvencyPractitioner ip = model.InsolvencyPractitioner;
        ValidateIPRegistrationNumber(context, ip.RegistrationNumber);
        ValidateIPFirmName(context, ip.FirmName);
        ValidateIPName(context, ip.Name);
        ValidateIPEmail(context, ip.EmailAddress);
        ValidateIPPhone(context, ip.TelephoneNumber);
        ValidateAddress(context, "Insolvency practitioner", ip.Address);
        
        return context;
    }

    private static void ValidateAddress(ValidatorContext context, string category, AddressType? address)
    {
        if (address is not null)
        {
            string line1 = address.Line[0];
            string? line2 = address.Line.Length > 1 ? address.Line[1] : null;
            string? line3 = address.Line.Length > 2 ? address.Line[2] : null;
            string? town = address.Town;
            string? county = address.County;
            string postcode = address.Postcode;
            string? country = address.Country;
            ValidateAddress(context, category, line1, line2, line3, town, county, postcode, country);
        }
    }
}