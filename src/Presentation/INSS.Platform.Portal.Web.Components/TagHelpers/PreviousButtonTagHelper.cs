using Microsoft.AspNetCore.Razor.TagHelpers;

namespace INSS.Platform.Portal.Web.Components.TagHelpers;

[HtmlTargetElement("inss-previous-button")]
public class PreviousButtonTagHelper : TagHelper
{
    public string PreviousUrl { get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "govuk-back-link");
        output.Attributes.SetAttribute("href", PreviousUrl);

        output.Content.SetHtmlContent("Back");
    }
}
