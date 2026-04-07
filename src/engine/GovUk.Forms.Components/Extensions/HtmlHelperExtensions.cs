using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Forms.Components.Extensions;

public static class HtmlHelperExtensions
{
    extension(IHtmlHelper helper)
    {
        public MvcForm BeginForm(ContentModel content)
        {
            return content.EncodingType is not null 
                ? helper.BeginForm("Edit", "Form", FormMethod.Post, new { enctype = content.EncodingType }) 
                : helper.BeginForm("Edit", "Form", FormMethod.Post);
        }
    }
}