using Notify.Models;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public interface INotifyService
{
    Task<Notification?> GetNotificationByIdAsync(string id);
}
