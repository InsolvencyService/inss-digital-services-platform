using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace INSS.Platform.Portal.Web.Components.TagHelpers;

[HtmlTargetElement("inss-error-message", Attributes = ForAttributeName)]
public class ErrorMessageTagHelper : TagHelper
{
    private const string ForAttributeName = "asp-for";

    [HtmlAttributeName(ForAttributeName)]
    public required ModelExpression For { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public required ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (ViewContext.ViewData.ModelState.ErrorCount > 0 &&
            ViewContext.ViewData.ModelState.TryGetValue(For.Name, out var value) &&
            value.Errors.Count > 0)
        {           
            output.TagName = "p";
            output.Attributes.SetAttribute("class", "govuk-error-message");
            output.Content.SetHtmlContent($@"<span class=""govuk-visually-hidden"">Error:</span>{value.Errors.First().ErrorMessage}");
        }
    }
}