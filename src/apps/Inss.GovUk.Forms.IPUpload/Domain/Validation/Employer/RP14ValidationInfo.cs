namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;

internal sealed class RP14ValidationInfo : ValidationInfo
{
    // Business
    internal static readonly RP14ValidationInfo MissingBusinessName = new("Business", "Name of business", "[COUNT] missing name of business");
    internal static readonly RP14ValidationInfo InvalidBusinessNameLength = new("Business", "Name of business", "[COUNT] too long name of business", "Up to 60 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidNatureOfBusinessLength = new("Business", "Nature of business", "[COUNT] too long nature of business", "Up to 100 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidCompanyNumberLength = new("Business", "Company number", "[COUNT] too long company number", "Up to 12 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidSICLength = new("Business", "Standard industrial classification", "[COUNT] too long standard industrial classification", "Up to 255 characters are allowed");
    
    // Directors
    internal static readonly RP14ValidationInfo DirectorInvalidNinoFormat = new("Director", "Director national insurance number", "[COUNT] invalid director national insurance number format", "Format is AB112233C");
    
    // Shareholders
    internal static readonly RP14ValidationInfo InvalidShareholderPercentage = new("Shareholders", "Shareholder percentage", "[COUNT] invalid shareholder percentage", "Expected format is 50.50 or 100");
    internal static readonly RP14ValidationInfo InvalidShareholderNameLength = new("Shareholders", "Shareholder name", "[COUNT] too long name of shareholder", "Up to 100 characters are allowed");
    
    // Associated companies
    internal static readonly RP14ValidationInfo InvalidAssociatedCompanyNameLength = new("Associated company", "Associated company name", "[COUNT] too long name of associated company", "Up to 60 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidAssociationReasonLength = new("Associated company", "Reason for association", "[COUNT] too long reason for association", "Up to 255 characters are allowed");
    
    // Employment continuity
    internal static readonly RP14ValidationInfo InvalidContinuityEmployerNameLength = new("Employment continuity", "Employer name", "[COUNT] too long employer name", "Up to 60 characters are allowed");

    // Transfers to
    internal static readonly RP14ValidationInfo InvalidTransferToNameLength = new("Transfers", "Transfer to name", "[COUNT] too long transfer to name", "Up to 60 characters are allowed");
    
    // Pay records contact
    internal static readonly RP14ValidationInfo MissingPayRecordName = new("Pay records contact", "Pay records contact name", "[COUNT] missing a Pay records contact name");
    internal static readonly RP14ValidationInfo InvalidPayRecordNameLength = new("Pay records contact", "Pay records contact name", "[COUNT] too long Pay records contact name", "Up to 60 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidPayRecordPhoneLength = new("Pay records contact", "Pay records contact phone number", "[COUNT] too long Pay records contact phone number", "Up to 12 characters are allowed");

    // Address
    internal static readonly RP14ValidationInfo InvalidLinesLength = new("[CATEGORY]", "Address lines", "[COUNT] too many address lines provided", "Up to 4 lines are allowed");
    internal static readonly RP14ValidationInfo InvalidLineLength = new("[CATEGORY]", "Address lines", "[COUNT] address line too long", "Up to 35 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidTownLength = new("[CATEGORY]", "Address town", "[COUNT] address town too long", "Up to 35 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidCountyLength = new("[CATEGORY]", "Address county", "[COUNT] address county too long", "Up to 35 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidPostcodeLength = new("[CATEGORY]", "Address postcode", "[COUNT] address postcode too long", "Up to 10 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidCountryLength = new("[CATEGORY]", "Address country", "[COUNT] address country too long", "Up to 10 characters are allowed");
    
    // IP
    internal static readonly RP14ValidationInfo InvalidIPRegistrationNumberLength = new("Insolvency practitioner", "Registration number", "[COUNT] too long registration number", "Up to 9 characters are allowed");
    internal static readonly RP14ValidationInfo InvalidIPFirmNameLength = new("Insolvency practitioner", "Firm name", "[COUNT] too long form name", "Up to 255 characters are allowed");

    private RP14ValidationInfo(string category, string property, string error, string? hint = null) : base(category, property, error, hint)
    {
    }
}