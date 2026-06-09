using System.Globalization;
using System.Text.RegularExpressions;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

public abstract partial class EmployerValidator : BaseValidator
{
    protected EmployerValidator(ICaseReferenceService caseReferenceService) : base(caseReferenceService)
    {
    }

    protected static void ValidateBusinessName(ValidatorContext context, string businessName)
    {
        if (string.IsNullOrWhiteSpace(businessName))
        {
            context.AddError(BusinessValidationInfo.MissingBusinessName(), businessName);
        }
        else if (businessName.Length > 60)
        {
            context.AddError(BusinessValidationInfo.InvalidBusinessNameLength(), businessName);
        } 
    }
    
    protected static void ValidateNatureOfBusiness(ValidatorContext context, string natureOfBusiness)
    {
        if (!string.IsNullOrEmpty(natureOfBusiness) && natureOfBusiness.Length > 100)
        {
            context.AddError(BusinessValidationInfo.InvalidNatureOfBusinessLength(), natureOfBusiness);
        } 
    }
    
    protected static void ValidateCompanyNumber(ValidatorContext context, string companyNumber)
    {
        if (!string.IsNullOrEmpty(companyNumber) && companyNumber.Length > 12)
        {
            context.AddError(BusinessValidationInfo.InvalidCompanyNumberLength(), companyNumber);
        } 
    }
    
    protected static void ValidateSICCode(ValidatorContext context, string sicCode)
    {
        if (!string.IsNullOrEmpty(sicCode) && sicCode.Length > 255)
        {
            context.AddError(BusinessValidationInfo.InvalidSICLength(), sicCode);
        } 
    }
    
    protected static void ValidateAddress(
        ValidatorContext context, 
        string category,
        string? line1, 
        string? line2, 
        string? line3, 
        string? town, 
        string? county, 
        string? postcode, 
        string? country)
    {
        if (string.IsNullOrWhiteSpace(line1))
        {
            context.AddError(AddressValidationInfo.MissingAddressLine1(category), line1);
        }
        else if (line1.Length > 35)
        {
            context.AddError(AddressValidationInfo.InvalidAddressLineLength(category), line1);
        }
        
        if (!string.IsNullOrEmpty(line2) && line2.Length > 35)
        {
            context.AddError(AddressValidationInfo.InvalidAddressLineLength(category), line2);
        }
        
        if (!string.IsNullOrEmpty(line3) && line3.Length > 35)
        {
            context.AddError(AddressValidationInfo.InvalidAddressLineLength(category), line3);
        }
        
        if (!string.IsNullOrEmpty(town) && town.Length > 35)
        {
            context.AddError(AddressValidationInfo.InvalidAddressTownLength(category), town);
        }
        
        if (!string.IsNullOrEmpty(county) && county.Length > 35)
        {
            context.AddError(AddressValidationInfo.InvalidAddressCountyLength(category), county);
        }
        
        if (!string.IsNullOrEmpty(postcode) && postcode.Length > 10)
        {
            context.AddError(AddressValidationInfo.InvalidAddressPostcodeLength(category), postcode);
        }
        
        if (!string.IsNullOrEmpty(country) && country.Length > 10)
        {
            context.AddError(AddressValidationInfo.InvalidAddressCountryLength(category), country);
        }
    }
    
    protected static void ValidateDirectorSurname(ValidatorContext context, string surname)
    {
        if (!string.IsNullOrEmpty(surname) && surname.Length > 100)
        {
            context.AddError(DirectorValidationInfo.InvalidDirectorSurnameLength(), surname);
        }
    }
    
    protected static void ValidateDirectorInitials(ValidatorContext context, string initials)
    {
        if (!string.IsNullOrEmpty(initials) && initials.Length > 100)
        {
            context.AddError(DirectorValidationInfo.InvalidDirectorInitialsLength(), initials);
        }
    }
    
    protected static void ValidateDirectorNino(ValidatorContext context, string nino)
    {
        if (!string.IsNullOrEmpty(nino) && !NinoFormatRegex().IsMatch(nino.Replace(" ", string.Empty)))
        {
            context.AddError(DirectorValidationInfo.InvalidDirectorNinoFormat(), nino);
        } 
    }
    
    protected static void ValidateShareholderName(ValidatorContext context, string name)
    {
        if (!string.IsNullOrEmpty(name) && name.Length > 100)
        {
            context.AddError(ShareholderValidationInfo.InvalidShareholderNameLength(), name);
        }
    }
    
    protected static void ValidateShareholderPercentage(ValidatorContext context, decimal percentage)
    {
        string value = percentage.ToString(CultureInfo.InvariantCulture);
        
        if (!PercentRegex().IsMatch(value))
        {
            context.AddError(ShareholderValidationInfo.InvalidShareholderPercentage(), value);
        } 
    }

    protected static void ValidateContinuityEmployerName(ValidatorContext context, string employerName)
    {
        if (employerName.Length > 60)
        {
            context.AddError(EmploymentContinuityValidationInfo.InvalidContinuityEmployerNameLength(), employerName);
        }
    }
    
    protected static void ValidateAssociatedCompanyName(ValidatorContext context, string companyName)
    {
        if (!string.IsNullOrEmpty(companyName) && companyName.Length > 60)
        {
            context.AddError(AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNameLength(), companyName);
        }
    }
    
    protected static void ValidateAssociatedCompanyNumber(ValidatorContext context, string companyNumber)
    {
        if (!string.IsNullOrEmpty(companyNumber) && companyNumber.Length > 9)
        {
            context.AddError(AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNumberLength(), companyNumber);
        }
    }
    
    protected static void ValidateCompanyAssociationReason(ValidatorContext context, string reason)
    {
        if (!string.IsNullOrEmpty(reason) && reason.Length > 255)
        {
            context.AddError(AssociatedCompanyValidationInfo.InvalidAssociationReasonLength(), reason);
        }
    }
    
    protected static void ValidateTransferToName(ValidatorContext context, string transferName)
    {
        if (transferName.Length > 60)
        {
            context.AddError(TransfersValidationInfo.InvalidTransferToNameLength(), transferName);
        }
    }
    
    protected static void ValidatePayRecordsContactName(ValidatorContext context, string contactName)
    {
        if (string.IsNullOrWhiteSpace(contactName))
        {
            context.AddError(PayRecordsContactValidationInfo.MissingPayRecordName(), contactName);
        }
        else if (contactName.Length > 60)
        {
            context.AddError(PayRecordsContactValidationInfo.InvalidPayRecordNameLength(), contactName);
        }
    }
    
    protected static void ValidatePayRecordsContactPhone(ValidatorContext context, string contactPhone)
    {
        if (!string.IsNullOrEmpty(contactPhone) && contactPhone.Length > 12)
        {
            context.AddError(PayRecordsContactValidationInfo.InvalidPayRecordPhoneLength(), contactPhone);
        }
    }
    
    protected static void ValidatePayRecordsContactEmail(ValidatorContext context, string contactEmail)
    {
        if (!string.IsNullOrEmpty(contactEmail) && contactEmail.Length > 100)
        {
            context.AddError(PayRecordsContactValidationInfo.InvalidPayRecordEmailLength(), contactEmail);
        }
    }
    
    protected static void ValidateIPRegistrationNumber(ValidatorContext context, string registrationNumber)
    {
        if (!string.IsNullOrEmpty(registrationNumber) && registrationNumber.Length > 9)
        {
            context.AddError(InsolvencyPractitionerValidationInfo.InvalidIPRegistrationNumberLength(), registrationNumber);
        }
    }
    
    protected static void ValidateIPFirmName(ValidatorContext context, string firmName)
    {
        if (!string.IsNullOrEmpty(firmName) && firmName.Length > 255)
        {
            context.AddError(InsolvencyPractitionerValidationInfo.InvalidIPFirmNameLength(), firmName);
        }
    }
    
    protected static void ValidateIPName(ValidatorContext context, string name)
    {
        if (!string.IsNullOrEmpty(name) && name.Length > 60)
        {
            context.AddError(InsolvencyPractitionerValidationInfo.InvalidIPNameLength(), name);
        }
    }
    
    protected static void ValidateIPEmail(ValidatorContext context, string email)
    {
        if (!string.IsNullOrEmpty(email) && email.Length > 100)
        {
            context.AddError(InsolvencyPractitionerValidationInfo.InvalidIPEmailLength(), email);
        }
    }
    
    protected static void ValidateIPPhone(ValidatorContext context, string phone)
    {
        if (!string.IsNullOrEmpty(phone) && phone.Length > 12)
        {
            context.AddError(InsolvencyPractitionerValidationInfo.InvalidIPPhoneLength(), phone);
        }
    }
    
    [GeneratedRegex(ValidationInfo.NinoFormat)]
    private static partial Regex NinoFormatRegex();
    
    [GeneratedRegex(ValidationInfo.PercentFormat)]
    private static partial Regex PercentRegex();
}