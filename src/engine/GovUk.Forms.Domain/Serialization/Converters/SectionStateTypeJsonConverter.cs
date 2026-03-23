using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Enums;

namespace GovUk.Forms.Domain.Serialization.Converters;

internal sealed class SectionStateTypeJsonConverter : JsonConverter<SectionStateTypes>
{
    public override SectionStateTypes Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Enum.Parse<SectionStateTypes>(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        SectionStateTypes value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}