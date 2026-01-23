using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

/// <summary>
/// Represents a model for capturing the source of income in a form.
/// </summary>
public sealed class SourceOfIncomeModel : IHasValue<IncomeSource?>
{
    /// <summary>
    /// Gets or sets the selected source of income.
    /// </summary>
    /// <remarks>
    /// This property is required. If not provided, a validation error will be raised.
    /// </remarks>
    [Required(ErrorMessage = "Enter source of income")]
    public IncomeSource? Value { get; set; }
}