using Inss.Platform.Domain.Components;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Inss.Platform.Component.Helpers;

public static class HtmlHelperExtensions
{
    public static Task<IHtmlContent> RenderPageComponentAsync<T>(this IHtmlHelper<T> helper, ComponentModel component, string? prefix = null)
    {
        if (!string.IsNullOrWhiteSpace(prefix))
        {
            ViewDataDictionary viewData = new(helper.ViewData) { TemplateInfo = { HtmlFieldPrefix = prefix } };
            return helper.PartialAsync(component.ViewName, component, viewData);
        }

        return helper.PartialAsync(component.ViewName, component);
    }
}