using Microsoft.Extensions.Primitives;

namespace INSS.Platform.UserManagement.Web.Components.Helpers
{
    /// <summary>
    /// Provides helper methods for working with query string values.
    /// </summary>
    internal static class QueryHelper
    {
        /// <summary>
        /// Attempts to retrieve a value from the query dictionary for the specified key.
        /// </summary>
        /// <param name="query">The query dictionary containing key-value pairs.</param>
        /// <param name="key">The key to look for in the query dictionary.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        /// if the key is found and the value is not null or empty; otherwise, <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the key exists in the query dictionary and the value is not null or empty; otherwise, <c>false</c>.
        /// </returns>
        internal static bool TryGetQueryValue(IDictionary<string, StringValues> query, string key, out string? value)
        {
            value = null;

            if (query.TryGetValue(key, out StringValues stringValue) && !StringValues.IsNullOrEmpty(stringValue))
            {
                value = stringValue.ToString();
                return true;
            }

            return false;
        }
    }
}
