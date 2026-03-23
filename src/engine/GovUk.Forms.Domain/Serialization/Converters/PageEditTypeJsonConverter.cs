using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Enums;

namespace GovUk.Forms.Domain.Serialization.Converters;

internal sealed class PageEditTypeJsonConverter : JsonConverter<PageEditTypes>
{
    public override PageEditTypes Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Enum.Parse<PageEditTypes>(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        PageEditTypes value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}