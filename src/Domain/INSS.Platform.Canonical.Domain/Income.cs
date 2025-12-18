namespace INSS.Platform.Canonical.Domain;

/// <summary>
/// Represents an income record associated with a user.
/// </summary>
public class Income : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the user associated with this income.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the source of the income (e.g., employment, pension).
    /// </summary>
    public string SourceOfIncome { get; set; }

    /// <summary>
    /// Gets or sets the gross amount of the income.
    /// </summary>
    public decimal GrossIncome { get; set; }

    /// <summary>
    /// Gets or sets the payment frequency for the income (e.g., monthly, weekly).
    /// </summary>
    public string PaymentFrequency { get; set; }

    /// <summary>
    /// Gets or sets the name of the income provider.
    /// </summary>
    public string IncomeProvider { get; set; }

    /// <summary>
    /// Gets or sets the user entity associated with this income.
    /// </summary>
    public virtual User? User { get; set; }
}
