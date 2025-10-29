using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Forms.TestHarness.Web.Common.Services
{
    public interface IPropertyValidator
    {
        IList<ValidationResult> ValidateProperties<T>(T model, IEnumerable<string> propertyNames);
        IList<ValidationResult> ValidateAllProperties<T>(T model);
    }
}
