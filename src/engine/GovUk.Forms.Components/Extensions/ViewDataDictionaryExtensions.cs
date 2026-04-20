using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GovUk.Forms.Components.Extensions;

[ExcludeFromCodeCoverage]
public static class ViewDataDictionaryExtensions
{
    private const string BackButtonKey = "Back";
    private const string FullWidthLayoutKey = "Layout";
    
    extension(ViewDataDictionary viewData)
    {
        public bool HasBackButton => viewData.ContainsKey(BackButtonKey);

        public void AddBackButton(string? path)
        {
            viewData[BackButtonKey] = path;
        }

        public string? GetBackButton()
        {
            return viewData[BackButtonKey]?.ToString();
        }
        
        public void AddFullWidthLayout(bool layout)
        {
            viewData[FullWidthLayoutKey] = layout;
        }

        public bool GetFullWidthLayout()
        {
            return viewData[FullWidthLayoutKey] is true;
        }
    }
}