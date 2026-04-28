using GovUk.Forms.Domain;

namespace GovUk.Forms.Components.Extensions;

public static class ModelExtensions
{
    extension(ContentModel content)
    {
        public bool IsPageModel()
        {
            return content is PageModel;
        }

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