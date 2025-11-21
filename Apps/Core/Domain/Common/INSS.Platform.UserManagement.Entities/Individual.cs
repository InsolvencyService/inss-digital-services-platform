namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents an individual user entity with personal and verification details.
    /// </summary>
    public class Individual : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the party.
        /// </summary>
        public Guid PartyId { get; set; }

        /// <summary>
        /// Gets or sets the individual's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the individual's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the individual's date of birth.
        /// </summary>
        public DateOnly? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the individual's National Insurance Number.
        /// </summary>
        public string? NationalInsuranceNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the individual's identity has been verified.
        /// </summary>
        public bool IsIdentityVerified { get; set; }

        /// <summary>
        /// Gets or sets the source of the identity verification.
        /// </summary>
        public string? VerificationSource { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Party"/> entity for the individual.
        /// </summary>
        public virtual Party? Party { get; set; }
    }
}
