using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace INSS.Platform.Forms.Web.TestHarness.Common.Services
{
    public class PropertyValidator : IPropertyValidator
    {
        /// <summary>
        /// Validates only the specified properties of a model using data annotations.
        /// </summary>
        /// <typeparam name="T">Type of the model.</typeparam>
        /// <param name="model">The model instance to validate.</param>
        /// <param name="propertyNames">List of property names to validate.</param>
        /// <returns>List of validation results for the specified properties.</returns>
        public IList<ValidationResult> ValidateProperties<T>(T model, IEnumerable<string> propertyNames)
        {
            List<ValidationResult> results = [];

            if(model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ValidationContext context = new (model);

            foreach (string propertyName in propertyNames)
            {
                PropertyInfo? property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                {
                    continue;
                }

                object? value = property.GetValue(model);
                context.MemberName = propertyName;

                Validator.TryValidateProperty(value, context, results);
            }

            return results;
        }

        /// <summary>
        /// Validates all public instance properties of a model using data annotations.
        /// </summary>
        /// <typeparam name="T">Type of the model.</typeparam>
        /// <param name="model">The model instance to validate.</param>
        /// <returns>List of validation results for all properties.</returns>
        public IList<ValidationResult> ValidateAllProperties<T>(T model)
        {
            List<ValidationResult> results = [];

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            ValidationContext context = new (model);

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                object? value = property.GetValue(model);
                context.MemberName = property.Name;

                Validator.TryValidateProperty(value, context, results);
            }

            return results;
        }
    }
}
