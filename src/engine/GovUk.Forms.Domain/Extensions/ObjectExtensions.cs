using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace GovUk.Forms.Domain.Extensions;

public static class ObjectExtensions
{
    extension(object instance)
    {
        public bool TryValidateRecursive(List<ExtendedValidationResult> results)
        {
            List<ValidationResult> x = [];
            
            ValidationContext context = new(instance, serviceProvider: null, items: null);
            bool isValid = Validator.TryValidateObject(instance, context, x, validateAllProperties: true);

            if (!isValid)
            {
                foreach (var q in x)
                {
                    string propertyName = q.MemberNames.First();
                    var z = instance.GetType().GetProperties().First(p => p.Name == propertyName);
                    results.Add(new ExtendedValidationResult(q, z));
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

public sealed class ExtendedValidationResult : ValidationResult
{
    public ExtendedValidationResult(ValidationResult validationResult, PropertyInfo property) : base(validationResult)
    {
        PropertyAnnotation = property.GetCustomAttribute<PropertyAnnotationAttribute>() ??
                                                         new PropertyAnnotationAttribute("Other", property.Name);
    }

    public PropertyAnnotationAttribute PropertyAnnotation { get; }
    
    // public ExtendedValidationResult(string? errorMessage) : base(errorMessage)
    // {
    // }
    //
    // public ExtendedValidationResult(string? errorMessage, IEnumerable<string>? memberNames) : base(errorMessage, memberNames)
    // {
    // }
    
    //public PropertyInfo Property { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyAnnotationAttribute : Attribute
{
    public PropertyAnnotationAttribute(string category, string propertyName)
    {
        Category = category;
        PropertyName = propertyName;
    }
    
    public string Category { get; }

    public string PropertyName { get; }
}