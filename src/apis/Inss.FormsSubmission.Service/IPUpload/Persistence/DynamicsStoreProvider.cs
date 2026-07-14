using System.Diagnostics.CodeAnalysis;
using System.Net;
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
    
    public async Task StoreAsync(DynamicsSubmission submission, CancellationToken cancellationToken)
    {
        Database? database = _cosmosClient.GetDatabase(_databaseName);
        Container? container = database.GetContainer(_containerName);
        await container.UpsertItemAsync(submission, new PartitionKey(submission.Reference), cancellationToken: cancellationToken);
    }
    
    public async Task<DynamicsSubmission?> GetAsync(string id, string reference, CancellationToken cancellationToken)
    {
        try
        {
            Database? database = _cosmosClient.GetDatabase(_databaseName);
            Container? container = database.GetContainer(_containerName);
            DynamicsSubmission dynamicsSubmission = await container.ReadItemAsync<DynamicsSubmission>(
                id, new PartitionKey(reference), cancellationToken: cancellationToken);
            return dynamicsSubmission;
        }
        catch (CosmosException error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
    
    public async Task<DynamicsSubmission[]> GetByReferenceAsync(string reference, CancellationToken cancellationToken)
    {
        try
        {
            Database? database = _cosmosClient.GetDatabase(_databaseName);
            Container? container = database.GetContainer(_containerName);
            QueryRequestOptions options = new() { PartitionKey = new PartitionKey(reference) };
            List<DynamicsSubmission> results = [];
            
            using (FeedIterator<DynamicsSubmission> iterator = container.GetItemQueryIterator<DynamicsSubmission>("SELECT * from c", requestOptions: options))
            {
                while (iterator.HasMoreResults)
                {
                    FeedResponse<DynamicsSubmission> response = await iterator.ReadNextAsync(cancellationToken);
                    results.AddRange(response);
                }
            }

            return results.ToArray();
        }
        catch (CosmosException error) when (error.StatusCode == HttpStatusCode.NotFound)
        {
            return [];
        }
    }
}