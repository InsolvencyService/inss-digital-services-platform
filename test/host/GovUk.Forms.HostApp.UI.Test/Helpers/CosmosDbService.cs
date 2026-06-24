using GovUk.Forms.HostApp.UI.Test.Config.Environments;
using GovUk.Forms.HostApp.UI.Test.Models.Settings;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Text.Json;

namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public sealed class CosmosDbService : ICosmosDbService, IAsyncDisposable
{
    private readonly CosmosClient _client;
    private readonly Container _container;
    private readonly Container _dynamicsContainer;

    public CosmosDbService(CosmosDbSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        IEnvironmentConfig config = EnvironmentConfigFactory.EnvironmentConfig;

        _client = new CosmosClient(config.CosmosEndpoint, settings.PrimaryKey);

        _container = _client
            .GetDatabase(settings.DatabaseName)
            .GetContainer(settings.ContainerName);

        _dynamicsContainer = _client
            .GetDatabase(settings.DynamicsDatabaseName)
            .GetContainer(settings.DynamicsContainerName);
    }

    public async Task DeleteItemAsync(string id, string partitionKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(partitionKey);

        try
        {
            await _container.DeleteItemAsync<object>(
                id,
                new PartitionKey(partitionKey));
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // Item already deleted.
        }
    }

    public Task DeleteIpUploadAsync(string email)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        return DeleteItemAsync(email, "/ip-upload");
    }

    public async Task DeleteDynamicsByUserIdAsync(string userId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        QueryDefinition query = new QueryDefinition(
            "SELECT VALUE c.reference FROM c WHERE c.userId = @userId")
            .WithParameter("@userId", userId);

        HashSet<string> references = [];

        using FeedIterator<string> iterator =
            _dynamicsContainer.GetItemQueryIterator<string>(query);

        while (iterator.HasMoreResults)
        {
            FeedResponse<string> response = await iterator.ReadNextAsync();

            foreach (string reference in response.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                references.Add(reference);
            }
        }

        foreach (string reference in references)
        {
            await DeleteByReferenceAsync(reference);
        }
    }

    public async Task<JsonElement[]> GetByReferenceAsync(string reference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);

        QueryRequestOptions options = new()
        {
            PartitionKey = new PartitionKey(reference)
        };

        List<JsonElement> results = [];

        using FeedIterator<JsonElement> iterator =
            _dynamicsContainer.GetItemQueryIterator<JsonElement>(
                new QueryDefinition("SELECT * FROM c"),
                requestOptions: options);

        while (iterator.HasMoreResults)
        {
            FeedResponse<JsonElement> response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results.ToArray();
    }

    public async Task DeleteByReferenceAsync(string reference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);

        QueryRequestOptions options = new()
        {
            PartitionKey = new PartitionKey(reference)
        };

        List<string> ids = [];

        using FeedIterator<string> iterator =
            _dynamicsContainer.GetItemQueryIterator<string>(
                new QueryDefinition("SELECT VALUE c.id FROM c"),
                requestOptions: options);

        while (iterator.HasMoreResults)
        {
            FeedResponse<string> response = await iterator.ReadNextAsync();

            ids.AddRange(response.Where(id => !string.IsNullOrWhiteSpace(id)));
        }

        foreach (string id in ids)
        {
            try
            {
                await _dynamicsContainer.DeleteItemAsync<object>(
                    id,
                    new PartitionKey(reference));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Item already deleted.
            }
        }
    }

    public async Task<string?> GetIpEmailReceiptAsync(string reference)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reference);

        QueryRequestOptions options = new()
        {
            PartitionKey = new PartitionKey(reference)
        };

        using FeedIterator<string> iterator =
            _dynamicsContainer.GetItemQueryIterator<string>(
                new QueryDefinition("SELECT TOP 1 VALUE c.ipEmailReceipt FROM c WHERE IS_DEFINED(c.ipEmailReceipt)"),
                requestOptions: options);

        while (iterator.HasMoreResults)
        {
            FeedResponse<string> response = await iterator.ReadNextAsync();

            string? value = response.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));

            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    public ValueTask DisposeAsync()
    {
        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}