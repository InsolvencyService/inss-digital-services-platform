using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public sealed class AddressModel : PageModel
{
    // COPILOT: This regex is comprehensive for UK postcodes, handling special cases and the various valid formats. If you need to support only standard postcodes, you could simplify it, but this version is robust for most real-world scenarios.
    private const string PostcodeRegexPattern = "^(GIR ?0AA|(?:(?:[A-PR-UWYZa-pr-uwyz][0-9][0-9]?|[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][0-9]?|[A-PR-UWYZa-pr-uwyz][0-9][A-HJKSTUWa-hjkstuw]?|[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][ABEHMNPRVWXYabehmnprvwxy])) ?[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})$";
    
    [Required(ErrorMessage = "Enter address line 1")]
    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }
 
    [Required(ErrorMessage = "Enter town or city")]
    public string TownCity { get; set; } = string.Empty;
    
    public string? County { get; set; }
    
    [Required(ErrorMessage = "Enter postcode")]
    [RegularExpression(PostcodeRegexPattern, ErrorMessage = "Enter a full UK postcode")]
    public string Postcode { get; set; } = string.Empty;
    
    public override string[] GetSummaryInfo()
    {
        return [AddressLine1];
    }
    
    public override void CopyTo(PageModel target)
    {
        AddressModel targetAddress = target.As<AddressModel>();
        targetAddress.AddressLine1 = AddressLine1;
        targetAddress.AddressLine2 = AddressLine2;
        targetAddress.TownCity = TownCity;
        targetAddress.County = County;
        targetAddress.Postcode = Postcode;
    }

    public override void ClearValues()
    {
        base.ClearValues();
        AddressLine1 = string.Empty;
        AddressLine2 = null;
        TownCity = string.Empty;
        County = null;
        Postcode = string.Empty;
    }
}