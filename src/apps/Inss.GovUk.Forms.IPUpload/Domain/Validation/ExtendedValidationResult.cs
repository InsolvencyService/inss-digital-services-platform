using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public sealed class ExtendedValidationResult : ValidationResult
{
    public ExtendedValidationResult(ValidationResult validationResult, PropertyInfo property) : base(validationResult)
    {
        PropertyAnnotation = property.GetCustomAttribute<PropertyAnnotationAttribute>() ??
                             new PropertyAnnotationAttribute("Other", property.Name);
    }

    public PropertyAnnotationAttribute PropertyAnnotation { get; }
}