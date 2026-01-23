using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a postal address, including validation for UK postcodes.
/// </summary>
public sealed class UserAddressModel
{
    /// <summary>
    /// Regular expression pattern for validating UK postcodes, including special cases and all valid formats.
    /// </summary>
    private const string PostcodeRegexPattern = "^(GIR ?0AA|(?:(?:[A-PR-UWYZa-pr-uwyz][0-9][0-9]?|[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][0-9]?|[A-PR-UWYZa-pr-uwyz][0-9][A-HJKSTUWa-hjkstuw]?|[A-PR-UWYZa-pr-uwyz][A-HK-Ya-hk-y][0-9][ABEHMNPRVWXYabehmnprvwxy])) ?[0-9][ABD-HJLNP-UW-Zabd-hjlnp-uw-z]{2})$";

    /// <summary>
    /// Gets the first line of the address.
    /// </summary>
    [Required(ErrorMessage = "Enter address line 1")]
    public string AddressLine1 { get; init; } = string.Empty;

    /// <summary>
    /// Gets the second line of the address (optional).
    /// </summary>
    public string? AddressLine2 { get; init; }

    /// <summary>
    /// Gets the town or city of the address.
    /// </summary>
    [Required(ErrorMessage = "Enter town or city")]
    public string TownCity { get; init; } = string.Empty;

    /// <summary>
    /// Gets the county of the address (optional).
    /// </summary>
    public string? County { get; init; }

    /// <summary>
    /// Gets the postcode of the address. Must be a valid UK postcode.
    /// </summary>
    [Required(ErrorMessage = "Enter postcode")]
    [RegularExpression(PostcodeRegexPattern, ErrorMessage = "Enter a full UK postcode")]
    public string Postcode { get; init; } = string.Empty;

    /// <summary>
    /// Returns a string representation of the address, combining all non-empty parts.
    /// </summary>
    /// <returns>A comma-separated string of the address components.</returns>
    public override string ToString()
    {
        string?[] parts = [AddressLine1, AddressLine2, TownCity, County, Postcode];
        return string.Join(", ", Enumerable.Where(parts, static part => !string.IsNullOrWhiteSpace(part)));
    }
}
