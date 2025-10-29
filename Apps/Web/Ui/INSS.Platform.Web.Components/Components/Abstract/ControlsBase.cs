using INSS.Platform.Web.Components.Components.Constants;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace INSS.Platform.Web.Components.Components.Abstract
{
    public abstract class ControlsBase : ComponentBase
    {
        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/> instance used for handling navigation and URI management in
        /// the component.
        /// </summary>
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;

        /// <summary>
        /// Gets or sets the unique name for the component.
        /// This name is used as part of the generated element IDs and names.
        /// </summary>
        [Parameter]
        [EditorRequired] // This makes the parameter mandatory
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Generates an element ID using the specified context and key.
        /// </summary>
        /// <param name="context">An optional context string to further qualify the ID.</param>
        /// <param name="key">An optional key to further qualify the ID.</param>
        /// <returns>
        /// An element ID string constructed from the component name, context, and key.
        /// </returns>
        protected string GenerateElementId(string? context = null, string? key = null)
        {
            return BuildElementString(context, key, true);
        }

        /// <summary>
        /// Generates an element name using the specified context and key.
        /// </summary>
        /// <param name="context">An optional context string to further qualify the name.</param>
        /// <param name="key">An optional key to further qualify the name.</param>
        /// <returns>An element name string.</returns>
        protected string GenerateElementName(string? context = null, string? key = null)
        {
            return BuildElementString(context, key, false);
        }

        /// <summary>
        /// Builds the element string for ID or name generation.
        /// </summary>
        /// <param name="context">An optional context string.</param>
        /// <param name="key">An optional key string.</param>
        /// <param name="includeGuid">Whether to include a unique GUID in the result.</param>
        /// <returns>The constructed element string.</returns>
        private string BuildElementString(string? context, string? key, bool includeId)
        {
            StringBuilder sb = new (Config.AppIdPrefix);

            if (includeId)
            {
                sb.Append("id-");
            }

            sb.Append(Name.ToLowerInvariant());

            if (!string.IsNullOrEmpty(context))
            {
                sb.Append('-').Append(context);
            }

            if (!string.IsNullOrEmpty(key))
            {
                sb.Append('-').Append(key);
            }

            return sb.ToString();
        }

    }
}
