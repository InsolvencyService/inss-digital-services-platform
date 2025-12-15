using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace INSS.Platform.Portal.Web.Components.TagHelpers;

[HtmlTargetElement("inss-task-list")]
public class TaskListTagHelper : TagHelper
{
    [HtmlAttributeNotBound]
    [ViewContext]
    public required ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "ul";
        output.Attributes.SetAttribute("class", "govuk-task-list");

        TagHelperContent childContent = await output.GetChildContentAsync();
        output.Content.SetHtmlContent(childContent);
    }
}