using System.ComponentModel;
using System.Reflection;

namespace INSS.Platform.Shared.Web.Utilities;

/// <summary>
/// Provides extension methods for working with <see cref="Enum"/> values.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the <see cref="DescriptionAttribute.Description"/> value for the specified <see cref="Enum"/> member, if present.
    /// </summary>
    /// <param name="value">The enumeration value to inspect.</param>
    /// <returns>
    /// The description defined by <see cref="DescriptionAttribute"/> on the enum member if available;
    /// otherwise, the enum member name.
    /// </returns>
    /// <remarks>
    /// If the enum field cannot be resolved via reflection, the enum member name is returned.
    /// </remarks>
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
