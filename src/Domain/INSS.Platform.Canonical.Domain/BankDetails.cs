namespace INSS.Platform.Canonical.Domain;

/// <summary>
/// Represents the bank details associated with a user.
/// </summary>
public class BankDetails : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the user associated with these bank details.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the name on the bank account.
    /// </summary>
    public string AccountName { get; set; }

    /// <summary>
    /// Gets or sets the sort code of the bank account.
    /// </summary>
    public string SortCode { get; set; }

    /// <summary>
    /// Gets or sets the account number of the bank account.
    /// </summary>
    public string AccountNumber { get; set; }

    /// <summary>
    /// Gets or sets the building society roll number, if applicable.
    /// </summary>
    public string? BuildingSocietyRollNumber { get; set; }

    /// <summary>
    /// Gets or sets the user associated with these bank details.
    /// </summary>
    public virtual User? User { get; set; }
}
