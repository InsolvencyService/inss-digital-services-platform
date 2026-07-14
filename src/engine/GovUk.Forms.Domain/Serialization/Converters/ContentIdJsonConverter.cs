using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain.Serialization.Converters;

internal sealed class ContentIdJsonConverter : JsonConverter<ContentId>
{
    public override ContentId Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options) =>
        new(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        ContentId value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}