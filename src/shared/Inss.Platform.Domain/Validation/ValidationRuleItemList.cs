using System.Globalization;
using Inss.Platform.Domain.Exceptions;

namespace Inss.Platform.Domain.Validation;

public sealed class ValidationRuleItemList : Dictionary<string, string>
{
    public T GetValue<T>(string key)
    {
        if (!ContainsKey(key))
        {
            throw new ComponentException($"Unable to find a validation rule item for {key}.");
        }
        
        string value = this[key];
        Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        object converted = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        return (T)converted;
    }
}