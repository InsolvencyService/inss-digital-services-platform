using Inss.Common.IPUpload.Employer.Api;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public sealed class EmployerApiValidator : EmployerValidator
{
    private readonly RP14 _model;

    public EmployerApiValidator(RP14 model)
    {
        _model = model;
    }

    public override ValidatorContext Validate(EmployerDetailsModel employerDetails)
    {
        EmployerValidatorContext context = new();

        ValidateCaseReference(context, _model.Header.CaseReference, employerDetails.CaseReference);
        ValidateBusinessName(context, _model.NameOfBusiness);
        ValidateNatureOfBusiness(context, _model.NatureOfBusiness);
        ValidatePayeReference(context, _model.PAYE?.District, _model.PAYE?.Reference);
        ValidateCompanyNumber(context, _model.CompanyNumber);
        ValidateAddress(context, "Business", _model.Address);
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

    private static void ValidateAddress(EmployerValidatorContext context, string category, AddressType? address)
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

    private static void ValidateDirectors(EmployerValidatorContext context, RP14Directors? directors)
    {
        foreach (RP14DirectorsDirector director in directors?.Director ?? [])
        {
            ValidateDirectorSurname(context, director.Name.Surname);
            ValidateDirectorInitials(context, director.Name.Initials);
            ValidateDirectorNino(context, director.NINO);
        }
    }

    private static void ValidateShareholders(EmployerValidatorContext context, RP14Shareholder[]? shareholders)
    {
        foreach (RP14Shareholder shareholder in shareholders ?? [])
        {
            if (shareholder.Name is not null)
            {
                ValidateShareholderName(context, shareholder.Name.FullName);
            }

            ValidateShareholderPercentage(context,shareholder.Percentage);
        }
    }

    private static void ValidateAssociatedCompanies(EmployerValidatorContext context, RP14AssociatedCompanies? associatedCompanies)
    {
        foreach (RP14AssociatedCompaniesAssociatedCompany associatedCompany in associatedCompanies?.AssociatedCompany ?? [])
        {
            ValidateAssociatedCompanyName(context, associatedCompany.CompanyName);
            ValidateAssociatedCompanyNumber(context, associatedCompany.CompanyNumber);
            ValidateCompanyAssociationReason(context, associatedCompany.ReasonForAssociation);
            ValidateAddress(context, "Associated company", associatedCompany.Address);
        }
    }

    private static void ValidateEmployees(EmployerValidatorContext context, RP14Employees? employees)
    {
        if (employees?.EmployeesClaimingContinuity is not null)
        {
            RP14EmployeesEmployeesClaimingContinuity employeeContinuity = employees.EmployeesClaimingContinuity;
            ValidateContinuityEmployerName(context, employeeContinuity.EmployerName);
            ValidateAddress(context, "Employment continuity", employeeContinuity.Address);   
        }
    }

    private static void ValidateTransferDetails(EmployerValidatorContext context, RP14TransferDetails? transferDetails)
    {
        if (transferDetails?.TransferTo is not null)
        {
            RP14TransferDetailsTransferTo transferTo = transferDetails.TransferTo;
            ValidateTransferToName(context, transferTo.Name);
            ValidateAddress(context, "Transfers", transferTo.Address);
        }
    }

    private static void ValidatePayRecordsContact(EmployerValidatorContext context, RP14PayRecordsContact? payRecordsContact)
    {
        if (payRecordsContact is not null)
        {
            ValidatePayRecordsContactName(context, payRecordsContact.Name);
            ValidatePayRecordsContactPhone(context, payRecordsContact.PhoneNumber);
            ValidatePayRecordsContactEmail(context, payRecordsContact.EmailAddress);
            ValidateAddress(context, "Pay records contact", payRecordsContact.Address);
        }
    }

    private static void ValidateInsolvencyPractitioner(EmployerValidatorContext context, RP14InsolvencyPractitioner? ip)
    {
        if (ip is not null)
        {
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
        }
    }
}