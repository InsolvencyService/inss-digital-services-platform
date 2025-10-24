using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace INSS.Platform.Forms.Web.TestHarness.Shared.Services
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
            var results = new List<ValidationResult>();

            if(model == null) throw new ArgumentNullException(nameof(model));
            var context = new ValidationContext(model);

            foreach (var propertyName in propertyNames)
            {
                var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null) continue;

                var value = property.GetValue(model);
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
            var results = new List<ValidationResult>();

            if (model == null) throw new ArgumentNullException(nameof(model));
            var context = new ValidationContext(model);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var value = property.GetValue(model);
                context.MemberName = property.Name;

                Validator.TryValidateProperty(value, context, results);
            }

            return results;
        }
    }
}
