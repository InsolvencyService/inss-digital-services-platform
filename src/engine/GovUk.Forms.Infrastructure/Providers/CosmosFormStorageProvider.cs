using System.Diagnostics.CodeAnalysis;
using System.Net;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;

namespace GovUk.Forms.Infrastructure.Providers;

[ExcludeFromCodeCoverage]
public sealed class CosmosFormStorageProvider : IFormStorageProvider
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _containerName;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string FormContextItem = "CurrentForm";

    public CosmosFormStorageProvider(
        CosmosClient cosmosClient, 
        string databaseName, 
        string containerName, 
        IHttpContextAccessor httpContextAccessor)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
        _containerName = containerName;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<bool> ExistsAsync(ContentPath path, string sessionId)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        QueryDefinition? query = new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.id = @id").WithParameter("@id", sessionId);
        QueryRequestOptions queryRequestOptions = new() { PartitionKey = new PartitionKey(path), MaxItemCount = 1 };

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
            if (_httpContextAccessor.HttpContext?.Items[FormContextItem] is FormModel form)
            {
                return form;
            }
            
            Database? database = _cosmosClient.GetDatabase(_databaseName);
            Container? container = database.GetContainer(_containerName);
            form = await container.ReadItemAsync<FormModel>(sessionId, new PartitionKey(path));
            _httpContextAccessor.HttpContext?.Items[FormContextItem] = form;
            return form;
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
        await container.UpsertItemAsync(form, new PartitionKey(form.Path));
    }
}