using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Domain.Serialization;
using Microsoft.Azure.Cosmos;

namespace GovUk.Forms.Infrastructure.Serialization;

[ExcludeFromCodeCoverage]
public sealed class CosmosFormCosmosSerializer : CosmosSerializer
{
    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            return FormSerializer.Deserialize<T>(stream);
        }
    }

    public override Stream ToStream<T>(T input)
    {
        MemoryStream stream = new();
        FormSerializer.Serialize(stream, input);
        stream.Position = 0;
        return stream;
    }
}