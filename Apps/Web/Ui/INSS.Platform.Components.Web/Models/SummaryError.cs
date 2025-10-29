namespace INSS.Platform.Components.Web.Models
{
    /// <summary>
    /// Represents a validation error associated with a specific UI element.
    /// </summary>
    public class SummaryError
    {
        /// <summary>
        /// Gets or sets the identifier of the UI element associated with the error.
        /// </summary>
        public string ElementId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message to display.
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }
}
