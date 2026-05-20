using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Cosmos;

namespace Inss.FormsSubmission.Service.IPUpload.Persistence;

[ExcludeFromCodeCoverage]
public class DynamicsStoreProvider : IDynamicsStoreProvider
{
    private readonly CosmosClient _cosmosClient;
    private readonly string _databaseName;
    private readonly string _containerName;
    
    public DynamicsStoreProvider(CosmosClient cosmosClient, string databaseName, string containerName)
    {
        _cosmosClient = cosmosClient;
        _databaseName = databaseName;
        _containerName = containerName;
    }
    
    public async Task StoreAsync(DynamicsSubmission submission)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        await container.UpsertItemAsync(submission, new PartitionKey(submission.Reference));
    }
}