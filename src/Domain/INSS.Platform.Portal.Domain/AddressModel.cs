using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class AddressModel : BaseModel
{
    // COPILOT: This regex is comprehensive for UK postcodes, handling special cases and the various valid formats. If you need to support only standard postcodes, you could simplify it, but this version is robust for most real-world scenarios.
    private const string PostcodeRegexPattern = "^(GIR ?0AA|(?:(?:[A-PR-UWYZa-pr-uwyz][0-9][0-9]?|[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][0-9]?|[A-PR-UWYZa-pr-uwyz][0-9][A-HJKSTUWa-hjkstuw]?|[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][ABEHMNPRVWXYabehmnprvwxy])) ?[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})$";

    public AddressModel()
    {
        PathName = "address";
        Name = "Address";
    }
    
    [Required(ErrorMessage = "Enter address line 1")]
    public string AddressLine1 { get; init; } = string.Empty;

    public string? AddressLine2 { get; init; }
 
    [Required(ErrorMessage = "Enter town or city")]
    public string TownCity { get; init; } = string.Empty;
 
    public string? County { get; init; }
 
    [Required(ErrorMessage = "Enter postcode")]
    [RegularExpression(PostcodeRegexPattern, ErrorMessage = "Enter a full UK postcode")]
    public string Postcode { get; init; } = string.Empty;
}