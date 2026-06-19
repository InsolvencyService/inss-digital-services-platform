using GovUk.Forms.HostApp.UI.Test.Models.Settings;
using Notify.Client;
using Notify.Models;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public sealed class NotifyService : INotifyService
{
    private readonly NotificationClient _client;

    public NotifyService(NotifySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _client = new NotificationClient(settings.ApiKey);
    }

    public async Task<Notification?> GetNotificationByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return await Task.Run(() => _client.GetNotificationById(id));
    }
}
