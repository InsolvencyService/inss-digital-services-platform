using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace GovUk.Forms.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public T BindAndValidate<T>(string key) where T : new()
        {
            T options = new();
            configuration.GetSection(key).Bind(options);
            
            ValidationContext validationContext = new ValidationContext(options);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(options, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                Console.WriteLine("Validation failed:");
                foreach (var result in validationResults)
                {
                    Console.WriteLine($" - {result.ErrorMessage}");
                }
            }
            else
            {
                Console.WriteLine("Validation passed.");
            }
            
            return options;
        }
    }
}