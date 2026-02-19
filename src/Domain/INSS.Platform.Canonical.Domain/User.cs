using INSS.Platform.Events.Domain;

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
    public DateOnly DateOfBirth { get; set; }

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

    /// <summary>
    /// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
    /// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
    /// In a properly defined application the events would be documented and also adhere to a defined contract.
    /// </summary>
    public void UserDetailsAdded(string actor, Guid correlationId, string fullName, DateOnly dateOfBirth, string telephoneNumber, string emailAddress)
    {
        UserDetailsAddedEvent userDetailsAddedEvent = new (
            actor: actor,
            aggregateRootId: Id,
            correlationId: correlationId,
            fullName: fullName,
            dateOfBirth: dateOfBirth,
            telephoneNumber: telephoneNumber,
            emailAddress: emailAddress
        );

        AddDomainEvent(userDetailsAddedEvent);
    }

    /// <summary>
    /// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
    /// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
    /// In a properly defined application the events would be documented and also adhere to a defined contract.
    /// </summary>
    public void UserIncomeAdded(string actor, Guid correlationId, decimal grossIncome, string incomeProvider)
    {
        UserIncomeAddedEvent userIncomeAddedEvent = new (
            actor: actor,
            aggregateRootId: Id,
            correlationId: correlationId,
            incomeProvider: incomeProvider,
            grossIncome : grossIncome
        );

        AddDomainEvent(userIncomeAddedEvent);
    }

    /// <summary>
    /// Audit Example: This snippet forms part of the example code that demonstrates how to raise domain events for auditing purposes.
    /// This is a simplified example and does not form part of a specification, at time of writing there isn't a specification.  
    /// In a properly defined application the events would be documented and also adhere to a defined contract.
    /// </summary>
    public void UserBankDetailsAdded(string actor, Guid correlationId, string accountName, string sortCode)
    {
        UserBankDetailsAddedEvent bankDetailsAddedEvent = new(
            actor: actor,
            aggregateRootId: Id,
            correlationId: correlationId,
            accountName: accountName,
            sortCode: sortCode
        );

        AddDomainEvent(bankDetailsAddedEvent);
    }
}
