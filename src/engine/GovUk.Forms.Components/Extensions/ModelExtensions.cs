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

        public string? GetSubmitButtonText()
        {
            return content switch
            {
                FormModel => "Submit form",
                PageModel page => page.MetaData.SubmitButtonText,
                _ => "Save and continue"
            };
        }
}
}