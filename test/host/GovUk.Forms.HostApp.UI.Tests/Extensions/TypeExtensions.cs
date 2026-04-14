using System.Reflection;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class TypeExtensions
{
    public static bool HasMemberOrPropertyDerivedFrom<TBase>(this Type type)
        where TBase : class
    {
        ArgumentNullException.ThrowIfNull(type);

        Type baseType = typeof(TBase);

        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        bool IsMatch(Type? candidateType)
        {
            if (candidateType == null)
            {
                return false;
            }

            // Direct match or subclass
            if (baseType.IsAssignableFrom(candidateType))
            {
                return true;
            }

            // Handle generics (e.g. List<BasePage>)
            if (candidateType.IsGenericType)
            {
                return candidateType
                    .GetGenericArguments()
                    .Any(baseType.IsAssignableFrom);
            }

            return false;
        }

        // Properties
        bool hasMatchingProperty = type
            .GetProperties(flags)
            .Where(p => p.GetIndexParameters().Length == 0) // skip indexers
            .Any(p => IsMatch(UnwrapType(p.PropertyType)));

        if (hasMatchingProperty)
        {
            return true;
        }

        // Fields
        bool hasMatchingField = type
            .GetFields(flags)
            .Any(f => IsMatch(UnwrapType(f.FieldType)));

        return hasMatchingField;
    }

    private static Type? UnwrapType(Type type)
    {
        Type? underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null)
        {
            return underlying;
        }

        return type;
    }
}
