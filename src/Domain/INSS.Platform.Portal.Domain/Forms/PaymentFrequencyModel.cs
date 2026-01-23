using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain.Forms;

/// <summary>
/// Represents a model for selecting a payment frequency.
/// </summary>
public sealed class PaymentFrequencyModel : IHasValue<PaymentFrequency?>
{
    /// <summary>
    /// Gets or sets the selected payment frequency.
    /// </summary>
    /// <remarks>
    /// This property is required. An error message is shown if not provided.
    /// </remarks>
    [Required(ErrorMessage = "Enter payment frequency")]
    public PaymentFrequency? Value { get; set; }
}