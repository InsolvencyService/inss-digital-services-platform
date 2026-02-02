using System.ComponentModel.DataAnnotations;

namespace INSS.Platform.Cache.Infrastructure.Configuration;

/// <summary>
/// Represents configuration options for connecting to an Azure Cosmos DB instance.
/// </summary>
public class CosmosCacheOptions
{
    /// <summary>
    /// Gets or sets the connection string used to connect to the Cosmos DB account.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Gets or sets the name of the Cosmos DB database.
    /// </summary>
    [Required]
    public string Database { get; set; }

    /// <summary>
    /// Gets or sets the name of the Cosmos DB container.
    /// </summary>
    [Required]
    public string Container { get; set; }
}
