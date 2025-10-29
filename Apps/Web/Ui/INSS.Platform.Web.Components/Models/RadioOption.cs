namespace INSS.Platform.Web.Components.Models
{
    /// <summary>
    /// Represents a radio button option with a value and display text.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value associated with the radio option. Must be a reference type.
    /// </typeparam>
    public class RadioOption<T> where T : class
    {
        /// <summary>
        /// Gets or sets the value associated with this radio option.
        /// </summary>
        public required T Value { get; set; }

        /// <summary>
        /// Gets or sets the display text for this radio option.
        /// </summary>
        public required string Text { get; set; }
    }
}
