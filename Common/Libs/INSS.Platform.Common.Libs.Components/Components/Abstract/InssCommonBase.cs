using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using INSS.Platform.Common.Libs.Components.Common;

namespace INSS.Platform.Common.Libs.Components.Components.Abstract
{
    /// <summary>
    /// Provides a base class for Blazor components with common functionality,
    /// including session management, unique element ID/name generation, and
    /// utility methods for value handling.
    /// </summary>
    public abstract class InssCommonBase : ComponentBase
    {
        /// <summary>
        /// Gets or sets the HTTP context accessor for accessing session and request data.
        /// </summary>
        [Inject] protected IHttpContextAccessor HttpContextAccessor { get; set; } = default!;

        /// <summary>
        /// Gets or sets the list index of the current list item stored in the session.
        /// </summary>
        //public int ListIndex
        //{
        //    get
        //    {
        //        var value = HttpContextAccessor.HttpContext!.Session.GetInt32($"{Config.SessionKey}_ListIndex");
        //        return value ?? 0;
        //    }
        //    set
        //    {
        //        HttpContextAccessor.HttpContext!.Session.SetInt32($"{Config.SessionKey}_ListIndex", value);
        //    }
        //}

        /// <summary>
        /// Gets or sets the index of the current page stored in the session.
        /// </summary>
        //public int PageIndex
        //{
        //    get
        //    {
        //        var value = HttpContextAccessor.HttpContext!.Session.GetInt32($"{Config.SessionKey}_PageIndex");
        //        return value ?? 0;
        //    }
        //    set
        //    {
        //        if (value < 0)
        //        {
        //            value = 0;
        //        }

        //        HttpContextAccessor.HttpContext!.Session.SetInt32($"{Config.SessionKey}_PageIndex", value);
        //    }
        //}

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
        /// Returns an array containing the specified value, or an empty array if the value is null or empty.
        /// </summary>
        /// <param name="value">The string value to wrap in an array.</param>
        /// <returns>
        /// An array containing the value if it is not null or empty; otherwise, an empty array.
        /// </returns>
        protected string[] GetValue(string? value)
        {
            return string.IsNullOrEmpty(value) ? [] : [value];
        }

        /// <summary>
        /// Generates an indexed name for an entity by appending the specified index.
        /// </summary>
        /// <param name="entityName">The base name of the entity.</param>
        /// <param name="index">The index to append to the entity name.</param>
        /// <returns>
        /// A string in the format "{entityName}-{index}" representing the indexed entity name.
        /// </returns>
        protected string IndexedName(string entityName, int index)
        {
            return $"{entityName}-{index.ToString()}";
        }

        /// <summary>
        /// Returns the full field name for a property of the <see cref="Form"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to generate the field name.</param>
        /// <returns>
        /// A string representing the full field name in the format "PropertyName".
        /// </returns>
        protected virtual string FieldName(string propertyName)
        {
            return propertyName;
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
            var sb = new System.Text.StringBuilder(Config.AppIdPrefix);

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
