using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Azure.Cosmos;

namespace Inss.FormsSubmission.Service.Infrastructure.Serialization;

[ExcludeFromCodeCoverage]
public sealed class CosmosModelSerializer : CosmosSerializer
{
    private static readonly JsonSerializerOptions _options = new() 
    {
        WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
    
    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            return JsonSerializer.Deserialize<T>(stream, _options)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream stream = new();
        JsonSerializer.Serialize(stream, input, _options);
        stream.Position = 0;
        return stream;
    }
}