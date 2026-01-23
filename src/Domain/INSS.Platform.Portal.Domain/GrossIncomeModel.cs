using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for capturing gross income input with validation.
/// </summary>
public sealed class GrossIncomeModel : IHasValue<decimal?>
{
    /// <summary>
    /// Gets or sets the gross income value.
    /// </summary>
    /// <remarks>
    /// This value is required and must be greater than 0.
    /// </remarks>
    [Required(ErrorMessage = "Enter your gross income")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Gross income must be greater than 0")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal? Value { get; set; }
}