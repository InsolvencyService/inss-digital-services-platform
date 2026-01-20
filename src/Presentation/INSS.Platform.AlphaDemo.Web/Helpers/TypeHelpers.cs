namespace INSS.Platform.AlphaDemo.Web.Helpers;

/// <summary>
/// Provides helper methods for type inspection.
/// </summary>
public static class TypeHelpers
{
    /// <summary>
    /// Determines whether the specified object is a complex object (i.e., not a primitive, string, enum, or other simple type).
    /// </summary>
    /// <param name="value">The object to inspect.</param>
    /// <returns>
    /// <c>true</c> if the object is considered complex; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <c>null</c>.</exception>
    public static bool IsComplexObject(object value)
    {
        Type type = value.GetType();

        // Use TypeCode to efficiently check for String, Decimal, DateTime, Enums, and most Primitives
        if (Type.GetTypeCode(type) != TypeCode.Object)
        {
            return false;
        }

        // Check for remaining simple types (IntPtr/UIntPtr via IsPrimitive, and specific structs/classes)
        return !type.IsPrimitive &&
               type != typeof(Guid) &&
               type != typeof(TimeSpan) &&
               type != typeof(DateTimeOffset) &&
               type != typeof(DateOnly) &&
               type != typeof(TimeOnly) &&
               type != typeof(Uri);
    }
}