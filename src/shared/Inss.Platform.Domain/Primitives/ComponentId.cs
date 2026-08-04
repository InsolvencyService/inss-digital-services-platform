using System.ComponentModel;
using System.Globalization;

namespace Inss.Platform.Domain.Primitives;

[TypeConverter(typeof(ComponentIdTypeConverter))]
public sealed record ComponentId(string Value = "")
{
    public static implicit operator string(ComponentId id) => id.Value;
    
    public static implicit operator ComponentId(string value) => new(value);
    
    public override string ToString() => Value;
    
    public class ComponentIdTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return new ComponentId((string)value);
        }
    }
}