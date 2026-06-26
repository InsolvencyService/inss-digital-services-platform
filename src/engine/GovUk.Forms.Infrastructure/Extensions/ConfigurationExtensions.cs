using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace GovUk.Forms.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public T BindAndValidate<T>(string key) where T : new()
        {
            T options = new();
            configuration.GetSection(key).Bind(options);
            
            ValidationContext validationContext = new(options);
            List<ValidationResult> validationResults = [];

            bool isValid = Validator.TryValidateObject(options, validationContext, validationResults, validateAllProperties: true);

            if (!isValid)
            {
                throw new ValidationException(string.Join(Environment.NewLine, validationResults.Select(r => r.ErrorMessage)));
            }
            
            return options;
        }
    }
}