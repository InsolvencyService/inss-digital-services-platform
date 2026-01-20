using Microsoft.AspNetCore.Razor.TagHelpers;

namespace INSS.Platform.Portal.Web.Components.TagHelpers;

[HtmlTargetElement("inss-summary-list-item")]
public class SummaryListItemTagHelper : TagHelper
{
    public string Text { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public bool BoldText { get; set; }

    public string OnChangeUrl { get; set; } = string.Empty;

    public string OnRemoveUrl{ get; set; } = string.Empty;

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "govuk-radios";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "govuk-summary-list__row");

        string removeLi = string.IsNullOrWhiteSpace(OnRemoveUrl)
            ? string.Empty
            : $@"<li class=""govuk-summary-list__actions-list-item"">
                    <a class=""govuk-link"" href=""{OnRemoveUrl}"">Remove<span class=""govuk-visually-hidden""> item</span></a>
                </li>";

        string boldClass = BoldText
            ? string.Empty
            : "govuk-!-font-weight-regular";

        output.Content.SetHtmlContent($@"
            <dt class=""govuk-summary-list__key {boldClass} hmrc-summary-list__key"">
                {Text}
            </dt>
            <dd class=""govuk-summary-list__value"">
                {Value}
            </dd>
            <dd class=""govuk-summary-list__actions"">
                <ul class=""govuk-summary-list__actions-list"">
                    <li class=""govuk-summary-list__actions-list-item"">
                        <a class=""govuk-link"" href=""{OnChangeUrl}"">Change<span class=""govuk-visually-hidden""> item</span></a>
                    </li>
                    {removeLi}
                </ul>
            </dd>
            ");
    }
}