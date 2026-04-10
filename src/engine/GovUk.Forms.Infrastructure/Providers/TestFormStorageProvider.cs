using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Serialization;
using GovUk.Forms.Infrastructure.Exceptions;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace GovUk.Forms.Infrastructure.Providers;

[ExcludeFromCodeCoverage]
public sealed class TestFormStorageProvider : IFormStorageProvider
{
    private static readonly ConcurrentDictionary<string, FormModel> _cache = new();
    
    public Task<bool> ExistsAsync(ContentPath path, string sessionId)
    {
        string key = GetKey(path.GetRoot(), sessionId);
        return Task.FromResult(_cache.ContainsKey(key));
    }

    public Task<FormModel> GetAsync(ContentPath path, string sessionId)
    {
        string key = GetKey(path.GetRoot(), sessionId);
        return _cache.TryGetValue(key, out FormModel? form) 
            ? Task.FromResult(form) 
            : throw new StorageProviderException($"Unable to find the form model for the session {sessionId}");
    }
    
    public Task SaveAsync(string sessionId, FormModel form)
    {
        string key = GetKey(form.Path, sessionId);
        _cache[key] = form;
        return Task.CompletedTask;
    }

    private static string GetKey(ContentPath path, string sessionId)
    {
        return $"{path}-{sessionId}";
    }
}

[ExcludeFromCodeCoverage]
public sealed class CosmosFormStorageProvider : IFormStorageProvider
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _containerName;

    public CosmosFormStorageProvider(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
        _containerName = containerName;
    }
    
    public async Task<bool> ExistsAsync(ContentPath path, string sessionId)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        QueryDefinition? query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.id = @id").WithParameter("@id", sessionId);
        QueryRequestOptions queryRequestOptions = new() { PartitionKey = new PartitionKey("Default"), MaxItemCount = 1 };

        using FeedIterator<int> resultSet = container.GetItemQueryIterator<int>(query, requestOptions: queryRequestOptions);

        if (resultSet.HasMoreResults)
        {
            FeedResponse<int> response = await resultSet.ReadNextAsync();
            return response.Resource.FirstOrDefault() > 0;
        }

        return false;
    }

    public async Task<FormModel> GetAsync(ContentPath path, string sessionId)
    {
        try
        {
            Database? database = _cosmosClient.GetDatabase(_databaseName);
            Container? container = database.GetContainer(_containerName);
            CosmosForm cosmosForm = await container.ReadItemAsync<CosmosForm>(sessionId, new PartitionKey("Default"));
            return FormSerializer.DeserializeForm(cosmosForm.Contents);

        }
        catch (CosmosException error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            throw new StorageProviderException($"Unable to find the form model for the session {sessionId}");
        }
    }
    
    public async Task SaveAsync(string sessionId, FormModel form)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        CosmosForm cosmosForm = new() { Id = sessionId, Contents = FormSerializer.SerializeForm(form) };
        await container.UpsertItemAsync(cosmosForm, new PartitionKey(cosmosForm.Form));
    }

    public sealed class CosmosForm
    {
        [JsonProperty("id")]
        public required string Id { get; init; }

#pragma warning disable CA1822
        public string Form => "Default";
#pragma warning restore CA1822
        
        public required string Contents { get; init; }
    }
}