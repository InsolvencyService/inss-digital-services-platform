using GovUk.Forms.Domain;

namespace GovUk.Forms.Components.Extensions;

public static class ContentModelExtensions
{
    extension(ContentModel content)
    {
        public string? GetButtonText()
        {
            return content switch
            {
                FormModel => "Submit form", // TODO: Move to form and add metadata support
                PageModel page => page.GetButtonText(),
                _ => "Save and continue"
            };
        }
    }
}