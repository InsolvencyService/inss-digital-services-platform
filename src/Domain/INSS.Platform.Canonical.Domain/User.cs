namespace INSS.Platform.Canonical.Domain;

/// <summary>
/// Represents a user entity within the canonical domain.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Gets or sets the user's full name.
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// Gets or sets the user's date of birth.
    /// </summary>
    public DateTime DateOfBirth { get; set; }
    
    /// <summary>
    /// Gets or sets the user's telephone number.
    /// </summary>
    public string TelephoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public string EmailAddress { get; set; }

    /// <summary>
    /// Gets or sets the collection of addresses associated with the user.
    /// </summary>
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    /// <summary>
    /// Gets or sets the collection of incomes associated with the user.
    /// </summary>
    public virtual ICollection<Income> Incomes { get; set; } = new List<Income>();

    /// <summary>
    /// Gets or sets the collection of bank details associated with the user.
    /// </summary>
    public virtual ICollection<BankDetails> BankDetails { get; set; } = new List<BankDetails>();
}
