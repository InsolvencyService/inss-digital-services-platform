using System.Text.Json;
using System.Text.Json.Serialization;
using GovUk.Forms.Domain.Enums;

namespace GovUk.Forms.Domain.Serialization.Converters;

internal sealed class SubmitTypeJsonConverter : JsonConverter<SubmitTypes>
{
    public override SubmitTypes Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
        Enum.Parse<SubmitTypes>(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        SubmitTypes value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}