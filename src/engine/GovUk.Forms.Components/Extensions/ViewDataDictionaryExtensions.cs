using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Forms.Components.Extensions;

[ExcludeFromCodeCoverage]
public static class ViewDataDictionaryExtensions
{
    private const string BackButtonKey = "Back";
    
    extension(ViewDataDictionary viewData)
    {
        public bool HasBackButton => viewData.ContainsKey(BackButtonKey);

        public void AddBackButton(ContentModel? content)
        {
            viewData[BackButtonKey] = content is PageModel page ? page.PreviousPagePath : null;
        }

        public string? GetBackButton()
        {
            return viewData[BackButtonKey]?.ToString();
        }
    }
}