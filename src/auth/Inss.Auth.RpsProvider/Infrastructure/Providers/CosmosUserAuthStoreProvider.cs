using System.Net;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Domain;
using Microsoft.Azure.Cosmos;

namespace Inss.Auth.RpsProvider.Infrastructure.Providers;

public sealed class CosmosUserAuthStoreProvider : IUserAuthStoreProvider
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _containerName;
    
    public CosmosUserAuthStoreProvider(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
        _containerName = containerName;
    }
    
    public async Task StoreAsync(UserAuth user)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        await container.UpsertItemAsync(user, new PartitionKey(user.CodeType));
    }

    public async Task<UserAuth?> GetAsync(string code)
    {
        try
        {
            Database? database = _cosmosClient.GetDatabase(_databaseName);
            Container? container = database.GetContainer(_containerName);
            UserAuth authCode = await container.ReadItemAsync<UserAuth>(code, new PartitionKey(UserAuth.AuthCodeType));
            return authCode;
        }
        catch (CosmosException error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task RemoveAsync(string code)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        await container.DeleteItemAsync<UserAuth>(code, new PartitionKey(UserAuth.AuthCodeType));
    }
}