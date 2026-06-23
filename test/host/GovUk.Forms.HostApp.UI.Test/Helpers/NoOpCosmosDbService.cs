using System.Text.Json;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

internal sealed class NoOpCosmosDbService : ICosmosDbService
{
    public Task DeleteItemAsync(string id, string partitionKey) => Task.CompletedTask;
    public Task DeleteIpUploadAsync(string email) => Task.CompletedTask;
    public Task DeleteDynamicsByUserIdAsync(string userId) => Task.CompletedTask;
    public Task DeleteByReferenceAsync(string reference) => Task.CompletedTask;
    public Task<JsonElement[]> GetByReferenceAsync(string reference) => Task.FromResult(Array.Empty<JsonElement>());
    public Task<string?> GetIpEmailReceiptAsync(string reference) => Task.FromResult<string?>(null);
}
