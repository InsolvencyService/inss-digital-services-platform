using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GovUk.Forms.Components.Extensions;

public static class HtmlHelperExtensions
{
    extension(IHtmlHelper helper)
    {
        public MvcForm BeginForm(ContentModel content)
        {
            return content is FileUploadModel 
                ? helper.BeginForm("Edit", "Form", FormMethod.Post, new { enctype = "multipart/form-data" }) 
                : helper.BeginForm("Edit", "Form", FormMethod.Post);
        }
    }
}