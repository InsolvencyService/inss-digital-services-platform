using GovUk.Forms.Domain.Types;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Forms.Components.Helpers;

public static class HtmlHelperExtensions
{
    public static Task<IHtmlContent> RenderTypeAsync<T>(this IHtmlHelper<T> helper, TypeBase model, string prefix)
    {
        ViewDataDictionary viewData = new(helper.ViewData) { TemplateInfo = { HtmlFieldPrefix = prefix } };
        return helper.PartialAsync(model.ViewName, model, viewData);
    }
}