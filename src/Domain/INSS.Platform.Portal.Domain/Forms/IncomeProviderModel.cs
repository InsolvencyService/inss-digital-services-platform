using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

/// <summary>
/// Represents a model for capturing the name of an income provider.
/// </summary>
public sealed class IncomeProviderModel : IHasValue<string>
{
    /// <summary>
    /// Gets the value of the income provider.
    /// </summary>
    /// <remarks>
    /// This field is required. If not provided, a validation error message will be shown.
    /// </remarks>
    [Required(ErrorMessage = "Enter income provider")]
    public string Value { get; init; } = string.Empty;
}