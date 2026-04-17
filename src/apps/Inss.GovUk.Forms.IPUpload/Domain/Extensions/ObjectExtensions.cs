using System.ComponentModel.DataAnnotations;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;

namespace Inss.GovUk.Forms.IPUpload.Domain.Extensions;

public static class ObjectExtensions
{
    extension(object instance)
    {
        public bool TryValidateRecursive(List<ExtendedValidationResult> results)
        {
            List<ValidationResult> internalResults = [];
            
            ValidationContext context = new(instance, serviceProvider: null, items: null);
            bool isValid = Validator.TryValidateObject(instance, context, internalResults, validateAllProperties: true);

            if (!isValid)
            {
                foreach (var result in internalResults)
                {
                    string propertyName = result.MemberNames.First();
                    var property = instance.GetType().GetProperties().First(p => p.Name == propertyName);
                    results.Add(new ExtendedValidationResult(result, property));
                }
            }
            
            var properties = instance.GetType().GetProperties().Where(
                p => p.CanRead && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType);

            foreach (var property in properties)
            {
                var value = property.GetValue(instance);
            
                if (value is null)
                {
                    continue;
                }

                if (value is IEnumerable<object> collection)
                {
                    foreach (var element in collection)
                    {
                        isValid &= element.TryValidateRecursive(results);
                    }
                }
                else
                {
                    isValid &= value.TryValidateRecursive(results);
                }
            }

            return isValid;
        }
    }
}