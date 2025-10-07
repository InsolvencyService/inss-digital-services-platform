using Microsoft.AspNetCore.Components;

namespace INSS.Platform.Common.Libs.Components.Components.Abstract
{
    /// <summary>
    /// Serves as the base class for INSS components, providing common functionality and shared services.
    /// It also allows components access to functionality that is common across all components including forms.
    /// </summary>
    /// <remarks>This class extends <see cref="InssCommonBase"/> and includes support for navigation and URI
    /// management through the <see cref="NavigationManager"/> service. Derived components can utilize this
    /// functionality to handle navigation-related tasks.</remarks>
    public abstract class InssComponentBase : InssCommonBase
    {
        /// <summary>
        /// Gets or sets the <see cref="NavigationManager"/> instance used for handling navigation and URI management in
        /// the component.
        /// </summary>
        [Inject] protected NavigationManager NavigationManager { get; set; } = default!;
    }
}
