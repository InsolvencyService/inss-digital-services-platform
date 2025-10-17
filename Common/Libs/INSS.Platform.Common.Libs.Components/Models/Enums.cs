namespace INSS.Platform.Common.Libs.Components.Models
{
    /// <summary>
    /// Specifies the type of input field to be rendered.
    /// </summary>
    public enum InputFieldType
    {
        /// <summary>
        /// A standard text input field.
        /// </summary>
        Text,

        /// <summary>
        /// An input field for email addresses.
        /// </summary>
        Email,

        /// <summary>
        /// An input field for telephone numbers.
        /// </summary>
        Telephone,

        /// <summary>
        /// An input field for numeric values.
        /// </summary>
        Number,

        /// <summary>
        /// An input field for currency values.
        /// </summary>
        Currency
    }
}
