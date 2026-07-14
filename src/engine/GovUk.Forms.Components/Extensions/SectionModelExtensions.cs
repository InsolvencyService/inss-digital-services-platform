using GovUk.Forms.Components.Exceptions;
using GovUk.Forms.Domain;

namespace GovUk.Forms.Components.Extensions;

public static class SectionModelExtensions
{
    extension(SectionModel section)
    {
        public bool IsComplete => section.CompletedDate is not null;

        public string GetState()
        {
            if (section.StartedDate is null)
            {
                return "No started";
            }

            if (section.StartedDate is not null && section.CompletedDate is null)
            {
                return "In progress";
            }
            
            if (section.StartedDate is not null && section.CompletedDate is not null)
            {
                return "Completed";
            }
            
            throw new ComponentException("Invalid section state. Cannot determine progress.");
        }
    }
}