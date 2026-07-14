using System.ComponentModel;
using System.Reflection;

namespace GovUk.Forms.Domain.Extensions;

public static class EnumExtensions
{
    public static string Description(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());

        if (field is null)
        {
            return value.ToString();
        }

        DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}