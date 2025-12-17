using INSS.Platform.Portal.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Portal.Domain;

public sealed class PaymentFrequencyModel : BaseModel
{
    public PaymentFrequencyModel()
    {
        PathName = "payment-frequency";
        Title = "Payment frequency";
    }

    [Required(ErrorMessage = "Enter the payment frequency")]
    public PaymentFrequency? PaymentFrequency { get; init; }
}
