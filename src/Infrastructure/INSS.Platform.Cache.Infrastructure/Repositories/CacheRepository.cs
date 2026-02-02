using INSS.Platform.Cache.Application.Repositories;
using INSS.Platform.Cache.Infrastructure.Configuration;
using INSS.Platform.Portal.Domain.Abstract;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Net;

namespace INSS.Platform.Cache.Infrastructure.Repositories;

/// <inheritdoc />
public class CacheRepository : ICacheRepository
{
    private readonly ILogger<CacheRepository> _logger;
    private readonly Container _container;
    private readonly JsonSerializerSettings _settings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheRepository"/> class,  providing access to form data stored
    /// in a Cosmos DB container.
    /// </summary>
    /// <remarks>This constructor initializes the repository by connecting to the "Forms" database 
    /// and the "Form" container within Cosmos DB. Ensure that the provided  <paramref name="cosmosClient"/>
    /// is properly configured and has access to the target database and container.</remarks>
    /// <param name="logger">The logger used to log diagnostic and operational information.</param>
    /// <param name="cosmosClient">The Cosmos DB client used to interact with the database.</param>
    /// <param name="options">The configuration options for Cosmos DB connection settings.</param>
    public CacheRepository(ILogger<CacheRepository> logger, CosmosClient cosmosClient, IOptions<CosmosCacheOptions> options)
    {
        _logger = logger;

        _settings.Converters.Add(new StringEnumConverter());

        Database database = cosmosClient.GetDatabase(options.Value.Database);
        _container = database.GetContainer(options.Value.Container);
    }

    /// <inheritdoc />
    public async Task<TForm?> GetFormAsync<TForm>(Guid id) where TForm : FormBase
    {
        try
        {
            QueryDefinition queryDefinition = new QueryDefinition(
                "SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id.ToString());

            FeedIterator<dynamic> query = _container.GetItemQueryIterator<dynamic>(queryDefinition);

            if (query.HasMoreResults)
            {
                FeedResponse<dynamic> response = await query.ReadNextAsync().ConfigureAwait(false);
                if (response.Count > 0)
                {
                    return JsonConvert.DeserializeObject<TForm>(response.First().ToString(), _settings);
                }
            }
        }
        catch (CosmosException cex)
        {
            _logger.LogError(cex, "Cosmos DB error while retrieving form by id: {Message}", cex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving form by id: {Message}", ex.Message);
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<HttpStatusCode> SaveFormAsync(FormBase form)
    {
        try
        {
            string json = JsonConvert.SerializeObject(form, _settings);
            JObject jObject = JObject.Parse(json);

            ItemResponse<JObject> response = await _container.UpsertItemAsync(jObject, new PartitionKey(form.FormSetId.ToString())).ConfigureAwait(false);

            return response.StatusCode;
        }
        catch (CosmosException cex)
        {
            _logger.LogError(cex, "Cosmos DB error while creating form from object: {Message}", cex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating form from object: {Message}", ex.Message);
        }

        return HttpStatusCode.BadRequest;
    }
}