namespace INSS.Platform.Portal.Domain.Abstract;

/// <summary>
/// Defines a contract for objects that expose a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public interface IHasValue<T>
{
    /// <summary>
    /// Gets the value associated with the implementing object.
    /// </summary>
    T Value { get; }
}
