using System.Reflection;
using System.Text.Json;

namespace INSS.Platform.AlphaDemo.Web.Services;

/// <summary>
/// Provides utility methods for persisting and retrieving model data from session storage.
/// </summary>
public static class ModelPersistence
{
    /// <summary>
    /// Retrieves a model object from session storage using the specified session key.
    /// </summary>
    /// <typeparam name="T">The type of model to retrieve. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="session">The session instance to retrieve data from.</param>
    /// <param name="sessionKey">The key used to store the model in session.</param>
    /// <returns>The deserialized model object from session, or a new instance if not found.</returns>
    public static T? GetFromSession<T>(ISession session, string sessionKey) where T : class, new()
    {
        string? modelJson = session.GetString(sessionKey);
        return modelJson != null
            ? JsonSerializer.Deserialize<T>(modelJson)
            : new T();
    }

    /// <summary>
    /// Stores a model object to session storage by merging it with any existing data.
    /// </summary>
    /// <typeparam name="T">The type of model to store. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="session">The session instance to store data in.</param>
    /// <param name="sessionKey">The key to use for storing the model in session.</param>
    /// <param name="incoming">The model data to merge and store.</param>
    public static void SetToSession<T>(ISession session, string sessionKey, T incoming) where T : class, new()
    {
        T existing = GetFromSession<T>(session, sessionKey) ?? new T();
        T merged = Merge(existing, incoming);
        string modelJson = JsonSerializer.Serialize(merged);
        session.SetString(sessionKey, modelJson);
    }

    /// <summary>
    /// Removes the model data associated with the specified session key.
    /// </summary>
    /// <param name="session">The session instance to remove data from.</param>
    /// <param name="sessionKey">The key of the data to remove from session.</param>
    public static void Reset(ISession session, string sessionKey)
    {
        session.Remove(sessionKey);
    }

    /// <summary>
    /// Merges properties from an incoming model into an existing model.
    /// Only non-null properties from the incoming model will overwrite existing properties.
    /// For value types, only non-default values will be merged.
    /// </summary>
    /// <typeparam name="T">The type of model to merge. Must be a class with a parameterless constructor.</typeparam>
    /// <param name="existing">The existing model to merge into.</param>
    /// <param name="incoming">The incoming model containing new values to merge.</param>
    /// <returns>The existing model with merged values from the incoming model.</returns>
    private static T Merge<T>(T existing, T incoming) where T : class, new()
    {
        existing ??= new T();
        if (incoming == null)
        {
            return existing;
        }

        Type type = typeof(T);
        foreach (PropertyInfo prop in type.GetProperties())
        {
            if (!prop.CanRead || !prop.CanWrite)
            {
                continue;
            }

            object? newValue = prop.GetValue(incoming);
            if (newValue != null)
            {
                if (prop.PropertyType.IsValueType)
                {
                    object? defaultValue = Activator.CreateInstance(prop.PropertyType);
                    if (!newValue.Equals(defaultValue))
                    {
                        prop.SetValue(existing, newValue);
                    }
                }
                else
                {
                    prop.SetValue(existing, newValue);
                }
            }
        }

        return existing;
    }
}