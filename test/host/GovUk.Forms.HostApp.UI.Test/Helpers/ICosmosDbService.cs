using System.Text.Json;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public interface ICosmosDbService
{
    Task DeleteItemAsync(string id, string partitionKey);
    Task DeleteIpUploadAsync(string email);
    Task DeleteDynamicsByUserIdAsync(string userId);
    Task DeleteByReferenceAsync(string reference);
    Task<JsonElement[]> GetByReferenceAsync(string reference);
}
