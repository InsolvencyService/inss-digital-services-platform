namespace INSS.Platform.Common.Libs.Components.Common
{
    /// <summary>
    /// Represents metadata information for a form, including identifiers and user context.
    /// </summary>
    public class FormMetadata
    {
        /// <summary>
        /// Gets or sets the unique identifier for this specific form instance.
        /// </summary>
        /// <remarks>
        /// Note: This identifier is unique to each form instance and is different from the form definition ID.  This is set as a unique key in Cosmos DB.
        /// </remarks>
        public Guid FormInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the form set instance.
        /// </summary>
        /// <remarks>
        /// This identifier is linked to this specific instance of a forms set defined within the digital service configuration.
        /// All form data associated with this form set instance will be stored under this identifier.
        /// Note: This is the Cosmos DB partition key for form data storage.
        /// </remarks>
        public Guid FormSetInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the form definition.
        /// </summary>
        /// <remarks>
        /// This identifier is linked to the form definition defined within the digital service configuration.
        /// </remarks>
        public Guid FormId { get; set; }

        /// <summary>
        /// Gets or sets the username associated with the form.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of the digital service that this form is assigned to.
        /// </summary>
        public DigitalServiceType? DigitalService { get; set; }

        /// <summary>
        /// Gets or sets the return URL to redirect after form completion.
        /// </summary>
        public string ReturnUrl { get; set; } = string.Empty;
    }
}
