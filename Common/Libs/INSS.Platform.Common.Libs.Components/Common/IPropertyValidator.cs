using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Common.Libs.Components.Common
{
    public interface IPropertyValidator
    {
        IList<ValidationResult> ValidateProperties<T>(T model, IEnumerable<string> propertyNames);
        IList<ValidationResult> ValidateAllProperties<T>(T model);
    }
}
