using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Types;

namespace GovUk.Forms.Domain.Serialization.Converters;

internal sealed class LabelSizesJsonConverter : JsonConverter<LabelSizes>
{
    public override LabelSizes Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Enum.Parse<LabelSizes>(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        LabelSizes value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}