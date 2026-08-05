using System.Text.Json;
using System.Text.Json.Serialization;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Serialization.Converters;

internal sealed class SessionIdConverter : JsonConverter<SessionId>
{
    public override SessionId Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options) =>
        new(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        SessionId value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}