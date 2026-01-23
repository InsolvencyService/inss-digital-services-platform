using INSS.Platform.Portal.Domain;

namespace INSS.Platform.AlphaDemo.Web.Models;

/// <summary>
/// Represents the income details for a form, including source, gross amount, payment frequency, and provider.
/// </summary>
public class IncomeModel : FormBase
{
    /// <summary>
    /// Gets or sets the source of income.
    /// </summary>
    public SourceOfIncomeModel SourceOfIncome { get; set; }

    /// <summary>
    /// Gets or sets the gross income details.
    /// </summary>
    public GrossIncomeModel GrossIncome { get; set; }

    /// <summary>
    /// Gets or sets the payment frequency information.
    /// </summary>
    public PaymentFrequencyModel PaymentFrequency { get; set; }

    /// <summary>
    /// Gets or sets the income provider details.
    /// </summary>
    public IncomeProviderModel IncomeProvider { get; set; }
}
