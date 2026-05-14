using System.Globalization;
using System.Text.RegularExpressions;
using FluentValidation;
using Inss.GovUk.Forms.IPUpload.Application.Services;

// ReSharper disable UnusedMethodReturnValue.Global

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

internal static partial class ValidatorRuleExtensions
{
    extension<T>(IRuleBuilder<T, string> rule)
    {
        internal IRuleBuilderOptions<T, string> ValidateBusinessName()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(RP14ValidationInfo.MissingBusinessName.PropertyFormat)
                .WithMessage(RP14ValidationInfo.MissingBusinessName.ErrorFormat)
                .MaximumLength(60)
                .OverridePropertyName(RP14ValidationInfo.InvalidBusinessNameLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidBusinessNameLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateCompanyNumber()
        {
            return rule
                .MaximumLength(12)
                .OverridePropertyName(RP14ValidationInfo.InvalidCompanyNumberLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidCompanyNumberLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateSIC()
        {
            return rule
                .MaximumLength(255)
                .OverridePropertyName(RP14ValidationInfo.InvalidSICLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidSICLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateDirectorNino()
        {
            return rule
                .Matches(ValidationInfo.NinoFormat)
                .OverridePropertyName(RP14ValidationInfo.DirectorInvalidNinoFormat.PropertyFormat)
                .WithMessage(RP14ValidationInfo.DirectorInvalidNinoFormat.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAssociatedCompanyName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(RP14ValidationInfo.InvalidAssociatedCompanyNameLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidAssociatedCompanyNameLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAssociationReason()
        {
            return rule
                .MaximumLength(255)
                .OverridePropertyName(RP14ValidationInfo.InvalidAssociationReasonLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidAssociationReasonLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateContinuityEmployerName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(RP14ValidationInfo.InvalidContinuityEmployerNameLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidContinuityEmployerNameLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateTransferToName()
        {
            return rule
                .MaximumLength(60)
                .OverridePropertyName(RP14ValidationInfo.InvalidTransferToNameLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidTransferToNameLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidatePayRecordContactName()
        {
            return rule
                .NotEmpty()
                .OverridePropertyName(RP14ValidationInfo.MissingPayRecordName.PropertyFormat)
                .WithMessage(RP14ValidationInfo.MissingPayRecordName.ErrorFormat)
                .MaximumLength(60)
                .OverridePropertyName(RP14ValidationInfo.InvalidPayRecordNameLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidPayRecordNameLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidatePayRecordContactPhone()
        {
            return rule
                .MaximumLength(12)
                .OverridePropertyName(RP14ValidationInfo.InvalidPayRecordPhoneLength.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidPayRecordPhoneLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressLine(string category)
        {
            return rule
                .MaximumLength(35)
                .OverridePropertyName(RP14ValidationInfo.InvalidLineLength.PropertyFormat.Replace("[CATEGORY]", category))
                .WithMessage(RP14ValidationInfo.InvalidLineLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressTown(string category)
        {
            return rule
                .MaximumLength(35)
                .OverridePropertyName(RP14ValidationInfo.InvalidTownLength.PropertyFormat.Replace("[CATEGORY]", category))
                .WithMessage(RP14ValidationInfo.InvalidTownLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressCounty(string category)
        {
            return rule
                .MaximumLength(35)
                .OverridePropertyName(RP14ValidationInfo.InvalidCountyLength.PropertyFormat.Replace("[CATEGORY]", category))
                .WithMessage(RP14ValidationInfo.InvalidCountyLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressPostcode(string category)
        {
            return rule
                .MaximumLength(10)
                .OverridePropertyName(RP14ValidationInfo.InvalidPostcodeLength.PropertyFormat.Replace("[CATEGORY]", category))
                .WithMessage(RP14ValidationInfo.InvalidPostcodeLength.ErrorFormat);
        }
        
        internal IRuleBuilderOptions<T, string> ValidateAddressCountry(string category)
        {
            return rule
                .MaximumLength(10)
                .OverridePropertyName(RP14ValidationInfo.InvalidCountryLength.PropertyFormat.Replace("[CATEGORY]", category))
                .WithMessage(RP14ValidationInfo.InvalidCountryLength.ErrorFormat);
        }
    }

    extension<T>(IRuleBuilder<T, string[]> rule)
    {
        internal IRuleBuilderOptions<T, string[]> ValidateAddressLines(string category)
        {
            return rule
                .Must(p => p.Length <= 4)
                .OverridePropertyName(RP14ValidationInfo.InvalidLinesLength.PropertyFormat.Replace("[CATEGORY]", category))
                .WithMessage(RP14ValidationInfo.InvalidPayRecordPhoneLength.ErrorFormat);
        }
    }

    extension<T>(IRuleBuilder<T, decimal> rule)
    {
        internal IRuleBuilderOptions<T, decimal> ValidateShareholderPercentage()
        {
            return rule
                .Must(value => PercentRegex().IsMatch(value.ToString(CultureInfo.CurrentCulture)))
                .OverridePropertyName(RP14ValidationInfo.InvalidShareholderPercentage.PropertyFormat)
                .WithMessage(RP14ValidationInfo.InvalidShareholderPercentage.ErrorFormat);
        }
    }
    
    [GeneratedRegex(ValidationInfo.PercentFormat)]
    private static partial Regex PercentRegex();
}