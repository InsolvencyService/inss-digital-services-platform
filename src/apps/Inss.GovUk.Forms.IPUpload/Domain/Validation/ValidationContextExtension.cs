using System.ComponentModel.DataAnnotations;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class ValidationContextExtension
{
    extension(ValidationContext context)
    {
        public ValidationResult? CreateValidationResult(string errorKey, string visitKey, params string[] properties)
        {
            // This provides a workaround to the issues relating to MS Validator and nested types.
            // It turns out that a recent version of .NET (8 or 9) changed how property and class validations operate, and now are mutually
            // exclusive in some cases and not in others, so sometimes it will list issues multiple times.
            // Below uses a visited flag to mitigate this, so each validate pass should only list issues once
            
            if (!context.Items.ContainsKey(visitKey))
            {
                context.Items[visitKey] = true;
                return new ValidationResult(errorKey, properties);
            }

            return null;
        }
    }
}