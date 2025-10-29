using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
namespace INSS.Platform.Web.Components.Components.Controls
{
    public class OLDButtonComponent : ComponentBase
    {
        [Parameter] public string ButtonText { get; set; } = string.Empty;

        [Parameter] public string CssClass { get; set; } = "govuk-button";

        [Parameter] public EventCallback OnClick { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            int seq = 0;
            
            builder.OpenElement(seq++, "button");
            builder.AddAttribute(seq++, "type", "submit");
            builder.AddAttribute(seq++, "class", CssClass);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, OnClick));
            builder.AddContent(seq++, ButtonText);
            builder.CloseElement();
        }
    }
}




