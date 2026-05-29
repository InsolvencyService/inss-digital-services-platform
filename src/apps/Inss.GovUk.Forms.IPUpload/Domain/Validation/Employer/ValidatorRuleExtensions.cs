using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

internal static partial class ValidatorRuleExtensions
{
    extension<T>(IRuleBuilder<T, string> rule)
    {
        internal IRuleBuilderOptions<T, string> ValidateCaseReference()
        {
            return rule
                .Matches(ValidationInfo.CaseReferenceFormat)
                .When(p => p is not null && !string.IsNullOrWhiteSpace(p.ToString()))
                .OverridePropertyName(CaseValidationInfo.InvalidCaseReferenceFormat.Property)
                .WithMessage(CaseValidationInfo.InvalidCaseReferenceFormat.Key)
                .MaximumLength(12)
                .When(p => p is not null && !string.IsNullOrWhiteSpace(p.ToString()))
                .OverridePropertyName(CaseValidationInfo.InvalidCaseReferenceLength.Property)
                .WithMessage(CaseValidationInfo.InvalidCaseReferenceLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateBusinessName()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(BusinessValidationInfo.MissingBusinessName.Property)
                .WithMessage(BusinessValidationInfo.MissingBusinessName.Key)
                .MaximumLength(60)
                .OverridePropertyName(BusinessValidationInfo.InvalidBusinessNameLength.Property)
                .WithMessage(BusinessValidationInfo.InvalidBusinessNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateNatureOfBusiness()
        {
            return rule
                .MaximumLength(100)
                .OverridePropertyName(BusinessValidationInfo.InvalidNatureOfBusinessLength.Property)
                .WithMessage(BusinessValidationInfo.InvalidNatureOfBusinessLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateCompanyNumber()
        {
            return rule
                .MaximumLength(12)
                .OverridePropertyName(BusinessValidationInfo.InvalidCompanyNumberLength.Property)
                .WithMessage(BusinessValidationInfo.InvalidCompanyNumberLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateSIC()
        {
            return rule
                .MaximumLength(255)
                .OverridePropertyName(BusinessValidationInfo.InvalidSICLength.Property)
                .WithMessage(BusinessValidationInfo.InvalidSICLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateDirectorNino()
        {
            return rule
                .Matches(ValidationInfo.NinoFormat)
                .OverridePropertyName(DirectorValidationInfo.DirectorInvalidNinoFormat.Property)
                .WithMessage(DirectorValidationInfo.DirectorInvalidNinoFormat.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateDirectorInitials()
        {
            return rule
                .MaximumLength(100)
                .OverridePropertyName(DirectorValidationInfo.InvalidDirectorInitialsLength.Property)
                .WithMessage(DirectorValidationInfo.InvalidDirectorInitialsLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateDirectorSurname()
        {
            return rule
                .MaximumLength(100)
                .OverridePropertyName(DirectorValidationInfo.InvalidDirectorSurnameLength.Property)
                .WithMessage(DirectorValidationInfo.InvalidDirectorSurnameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAssociatedCompanyName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNameLength.Property)
                .WithMessage(AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAssociatedCompanyNumber()
        {
            return rule
                .MaximumLength(9)
                .OverridePropertyName(AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNumberLength.Property)
                .WithMessage(AssociatedCompanyValidationInfo.InvalidAssociatedCompanyNumberLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAssociationReason()
        {
            return rule
                .MaximumLength(255)
                .OverridePropertyName(AssociatedCompanyValidationInfo.InvalidAssociationReasonLength.Property)
                .WithMessage(AssociatedCompanyValidationInfo.InvalidAssociationReasonLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateContinuityEmployerName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(EmploymentContinuityValidationInfo.InvalidContinuityEmployerNameLength.Property)
                .WithMessage(EmploymentContinuityValidationInfo.InvalidContinuityEmployerNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateTransferToName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(TransfersValidationInfo.InvalidTransferToNameLength.Property)
                .WithMessage(TransfersValidationInfo.InvalidTransferToNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidatePayRecordContactName()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(PayRecordsContactValidationInfo.MissingPayRecordName.Property)
                .WithMessage(PayRecordsContactValidationInfo.MissingPayRecordName.Key)
                .MaximumLength(60)
                .OverridePropertyName(PayRecordsContactValidationInfo.InvalidPayRecordNameLength.Property)
                .WithMessage(PayRecordsContactValidationInfo.InvalidPayRecordNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidatePayRecordContactPhone()
        {
            return rule
                .MaximumLength(12)
                .OverridePropertyName(PayRecordsContactValidationInfo.InvalidPayRecordPhoneLength.Property)
                .WithMessage(PayRecordsContactValidationInfo.InvalidPayRecordPhoneLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidatePayRecordContactEmail()
        {
            return rule
                .MaximumLength(100)
                .OverridePropertyName(PayRecordsContactValidationInfo.InvalidPayRecordEmailLength.Property)
                .WithMessage(PayRecordsContactValidationInfo.InvalidPayRecordEmailLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressLine(string category)
        {
            return rule
                .MaximumLength(35)
                .OverridePropertyName(AddressValidationInfo.InvalidAddressLineLength.Property.Replace("[CATEGORY]", category))
                .WithMessage(AddressValidationInfo.InvalidAddressLineLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressTown(string category)
        {
            return rule
                .MaximumLength(35)
                .OverridePropertyName(AddressValidationInfo.InvalidAddressTownLength.Property.Replace("[CATEGORY]", category))
                .WithMessage(AddressValidationInfo.InvalidAddressTownLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressCounty(string category)
        {
            return rule
                .MaximumLength(35)
                .OverridePropertyName(AddressValidationInfo.InvalidAddressCountyLength.Property.Replace("[CATEGORY]", category))
                .WithMessage(AddressValidationInfo.InvalidAddressCountyLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressPostcode(string category)
        {
            return rule
                .MaximumLength(10)
                .OverridePropertyName(AddressValidationInfo.InvalidAddressPostcodeLength.Property.Replace("[CATEGORY]", category))
                .WithMessage(AddressValidationInfo.InvalidAddressPostcodeLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressCountry(string category)
        {
            return rule
                .MaximumLength(10)
                .OverridePropertyName(AddressValidationInfo.InvalidAddressCountryLength.Property.Replace("[CATEGORY]", category))
                .WithMessage(AddressValidationInfo.InvalidAddressCountryLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateIPRegistrationNumber()
        {
            return rule
                .MaximumLength(9)
                .OverridePropertyName(InsolvencyPractitionerValidationInfo.InvalidIPRegistrationNumberLength.Property)
                .WithMessage(InsolvencyPractitionerValidationInfo.InvalidIPRegistrationNumberLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateIPFirmName()
        {
            return rule
                .MaximumLength(255)
                .OverridePropertyName(InsolvencyPractitionerValidationInfo.InvalidIPFirmNameLength.Property)
                .WithMessage(InsolvencyPractitionerValidationInfo.InvalidIPFirmNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateIPName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(InsolvencyPractitionerValidationInfo.InvalidIPNameLength.Property)
                .WithMessage(InsolvencyPractitionerValidationInfo.InvalidIPNameLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateIPEmail()
        {
            return rule
                .MaximumLength(100)
                .OverridePropertyName(InsolvencyPractitionerValidationInfo.InvalidIPEmailLength.Property)
                .WithMessage(InsolvencyPractitionerValidationInfo.InvalidIPEmailLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateIPPhone()
        {
            return rule
                .MaximumLength(40)
                .OverridePropertyName(InsolvencyPractitionerValidationInfo.InvalidIPPhoneLength.Property)
                .WithMessage(InsolvencyPractitionerValidationInfo.InvalidIPPhoneLength.Key);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateShareholderName()
        {
            return rule
                .MaximumLength(100)
                .OverridePropertyName(ShareholderValidationInfo.InvalidShareholderNameLength.Property)
                .WithMessage(ShareholderValidationInfo.InvalidShareholderNameLength.Key);
        }
    }

    extension<T>(IRuleBuilder<T, string[]> rule)
    {
        internal IRuleBuilderOptions<T, string[]> ValidateAddressLines(string category)
        {
            return rule
                .Must(p => p.Length <= 4)
                .OverridePropertyName(AddressValidationInfo.InvalidAddressLinesLength.Property.Replace("[CATEGORY]", category))
                .WithMessage(AddressValidationInfo.InvalidAddressLinesLength.Key);
        }
    }

    extension<T>(IRuleBuilder<T, decimal> rule)
    {
        internal IRuleBuilderOptions<T, decimal> ValidateShareholderPercentage()
        {
            return rule
                .Must(value => PercentRegex().IsMatch(value.ToString(CultureInfo.CurrentCulture)))
                .OverridePropertyName(ShareholderValidationInfo.InvalidShareholderPercentage.Property)
                .WithMessage(ShareholderValidationInfo.InvalidShareholderPercentage.Key);
        }
    }
    
    [GeneratedRegex(ValidationInfo.PercentFormat)]
    private static partial Regex PercentRegex();
}