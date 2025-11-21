namespace INSS.Platform.UserManagement.Entities
{
    /// <summary>
    /// Represents a product entity with audit information.
    /// </summary>
    public class Product : AuditableEntity
    {
        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL associated with the product.
        /// </summary>
        public string? Url { get; set; }
    }
}
