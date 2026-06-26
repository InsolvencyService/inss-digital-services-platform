namespace GovUk.Forms.Infrastructure.Options;

public sealed class SearchPersonOptions
{
    public string Endpoint { get; init; }

    public string IndexName { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;

    public string ApiVersion { get; init; } = string.Empty;
}

public sealed class AzureSearchOptions
{
    public string Endpoint { get; init; }

    public string IndexName { get; init; }

    public string ApiKey { get; init; }

    public string ApiVersion { get; init; }
    
    public string ConfigPath { get; init; }
}

