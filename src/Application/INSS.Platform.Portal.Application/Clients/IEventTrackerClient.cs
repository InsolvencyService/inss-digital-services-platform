namespace INSS.Platform.Portal.Application.Clients;

/// <summary>
/// Provides functionality for tracking analytics events within the INSS Forms application.
/// </summary>
public interface IEventTrackerClient
{
    /// <summary>
    /// Asynchronously tracks an analytics event with the specified type, name, and properties.
    /// </summary>
    /// <param name="eventType">The type or category of the event being tracked (e.g., "user_action", "system_event").</param>
    /// <param name="eventName">The specific name of the event being tracked (e.g., "form_submitted", "page_viewed").</param>
    /// <param name="properties">A dictionary containing additional properties and metadata associated with the event.</param>
    /// <returns>A task that represents the asynchronous tracking operation.</returns>
    Task TrackEventAsync(string eventType, string eventName, Dictionary<string, object> properties);
}
