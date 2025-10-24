namespace INSS.Platform.Common.Libs.Components.Shared.Extensions
{
    /// <summary>
    /// Provides extension methods for formatting numbers.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Converts a nullable decimal value to its currency string representation.
        /// </summary>
        /// <param name="number">The nullable decimal value to format.</param>
        /// <returns>
        /// A string that represents the decimal value formatted as currency.
        /// If <paramref name="number"/> is <c>null</c>, returns "£0.00" (or equivalent for current culture).
        /// </returns>
        public static string ToStringAsCurrency(this decimal? number)
        {
            return (number ?? 0m).ToString("C");
        }

        /// <summary>
        /// Converts a decimal value to its currency string representation.
        /// </summary>
        /// <param name="number">The decimal value to format.</param>
        /// <returns>
        /// A string that represents the decimal value formatted as currency.
        /// </returns>
        public static string ToStringAsCurrency(this decimal number)
        {
            return number.ToString("C");
        }
    }
}
