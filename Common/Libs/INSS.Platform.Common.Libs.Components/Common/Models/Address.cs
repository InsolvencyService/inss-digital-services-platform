using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Common.Libs.Components.Common.Models
{
    /// <summary>
    /// Represents a postal address.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        [Required(ErrorMessage = "Address line 1 is required.")]
        public string Address1 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the second line of the address.
        /// </summary>
        public string Address2 { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the town or city of the address.
        /// </summary>
        public string Town { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postcode of the address.
        /// </summary>
        [Required(ErrorMessage = "Postcode is required.")]
        public string Postcode { get; set; } = string.Empty;
    }
}
