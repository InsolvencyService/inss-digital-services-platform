using Inss.Common.IPUpload.Employer.Api;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class EmployerApiValidator : EmployerValidator
{
    private readonly RP14 _model;

    public EmployerApiValidator(RP14 model, ICaseReferenceService caseReferenceService) : base(caseReferenceService)
    {
        _model = model;
    }

    public override async Task<ValidatorContext> ValidateAsync()
    {
        EmployerValidatorContext context = new();

        await ValidateCaseReferenceAsync(context, _model.Header.CaseReference);
        
        ValidateBusinessName(context, _model.NameOfBusiness);
        ValidateNatureOfBusiness(context, _model.NatureOfBusiness);
        ValidateCompanyNumber(context, _model.CompanyNumber);
        ValidateAddress(context, "Business", _model.Address);
        ValidateSICCode(context, _model.SICCode);
        
        foreach (RP14DirectorsDirector director in _model.Directors.Director)
        {
            ValidateDirectorSurname(context, director.Name.Surname);
            ValidateDirectorInitials(context, director.Name.Initials);
            ValidateDirectorNino(context, director.NINO);
        }

        foreach (RP14Shareholder shareholder in _model.Shareholders)
        {
            ValidateShareholderName(context, shareholder.Name.FullName);
            ValidateShareholderPercentage(context,shareholder.Percentage);
        }

        foreach (RP14AssociatedCompaniesAssociatedCompany associatedCompany in _model.AssociatedCompanies.AssociatedCompany)
        {
            ValidateAssociatedCompanyName(context, associatedCompany.CompanyName);
            ValidateAssociatedCompanyNumber(context, associatedCompany.CompanyNumber);
            ValidateCompanyAssociationReason(context, associatedCompany.ReasonForAssociation);
            ValidateAddress(context, "Associated company", associatedCompany.Address);
        }

        RP14EmployeesEmployeesClaimingContinuity employeeContinuity = _model.Employees.EmployeesClaimingContinuity;
        ValidateContinuityEmployerName(context, employeeContinuity.EmployerName);
        ValidateAddress(context, "Employment continuity", employeeContinuity.Address);
        
        RP14TransferDetailsTransferTo transferTo = _model.TransferDetails.TransferTo;
        ValidateTransferToName(context, transferTo.Name);
        ValidateAddress(context, "Transfers", transferTo.Address);

        RP14PayRecordsContact payRecordsContact = _model.PayRecordsContact;
        ValidatePayRecordsContactName(context, payRecordsContact.Name);
        ValidatePayRecordsContactPhone(context, payRecordsContact.PhoneNumber);
        ValidatePayRecordsContactEmail(context, payRecordsContact.EmailAddress);
        ValidateAddress(context, "Pay records contact", payRecordsContact.Address);

        RP14InsolvencyPractitioner ip = _model.InsolvencyPractitioner;
        ValidateIPRegistrationNumber(context, ip.RegistrationNumber);
        ValidateIPFirmName(context, ip.FirmName);
        ValidateIPName(context, ip.Name);
        ValidateIPEmail(context, ip.EmailAddress);
        ValidateIPPhone(context, ip.TelephoneNumber);

        // Only validate the IP address if the address line 1 is set as its optional in this case but mandatory in all other cases!
        // Aligned with the spreadsheet for consistency as the same JSON gets sent to Dynamics!
        if (ip.Address.Line.Length > 0 && !string.IsNullOrWhiteSpace(ip.Address.Line[0]))
        {
            ValidateAddress(context, "Insolvency practitioner", ip.Address);
        }

        return context;
    }

    private static void ValidateAddress(ValidatorContext context, string category, AddressType? address)
    {
        if (address is not null)
        {
            if (address.Line.Length > 4)
            {
                context.AddError(AddressValidationInfo.InvalidAddressLinesLength(category), $"{address.Line.Length:N0}");
            }
            
            string? line1 = address.Line.Length > 0 ? address.Line[0] : null;
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