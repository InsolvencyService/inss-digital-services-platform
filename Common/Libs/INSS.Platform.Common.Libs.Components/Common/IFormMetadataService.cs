namespace INSS.Platform.Common.Libs.Components.Common
{
    /// <summary>
    /// Provides functionality to create <see cref="FormMetadata"/> from the current query string context.
    /// </summary>
    public interface IFormMetadataService
    {
        /// <summary>
        /// Creates a <see cref="FormMetadata"/> instance by extracting relevant information from the query string.
        /// </summary>
        /// <returns>
        /// A <see cref="FormMetadata"/> object populated with data parsed from the query string.
        /// </returns>
        FormMetadata? CreateFromQueryString();
    }
}
