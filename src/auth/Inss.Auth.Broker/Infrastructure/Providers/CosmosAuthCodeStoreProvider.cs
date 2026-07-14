using System.Net;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Domain;
using Microsoft.Azure.Cosmos;

namespace Inss.Auth.Broker.Infrastructure.Providers;

public sealed class CosmosAuthCodeStoreProvider : IAuthCodeStoreProvider
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _containerName;
    
    public CosmosAuthCodeStoreProvider(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
        _containerName = containerName;
    }
    
    public async Task StoreAsync(AuthCode authCode)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        await container.UpsertItemAsync(authCode, new PartitionKey(authCode.CodeType));
    }

    public async Task<AuthCode?> GetAsync(string id)
    {
        try
        {
            Database? database = _cosmosClient.GetDatabase(_databaseName);
            Container? container = database.GetContainer(_containerName);
            AuthCode authCode = await container.ReadItemAsync<AuthCode>(id, new PartitionKey(AuthCode.AuthCodeType));
            return authCode;
        }
        catch (CosmosException error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task RemoveAsync(string id)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        await container.DeleteItemAsync<AuthCode>(id, new PartitionKey(AuthCode.AuthCodeType));
    }
}