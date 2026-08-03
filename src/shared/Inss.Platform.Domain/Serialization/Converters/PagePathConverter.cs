using System.Text.Json;
using System.Text.Json.Serialization;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Serialization.Converters;

internal sealed class PagePathConverter : JsonConverter<PagePath>
{
    public override PagePath Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options) =>
        new(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        PagePath value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

internal sealed class ComponentIdConverter : JsonConverter<ComponentId>
{
    public override ComponentId Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options) =>
        new(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        ComponentId value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

internal sealed class ContentConverter : JsonConverter<Content>
{
    public override Content Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options) =>
        new(reader.GetString()!);

    public override void Write(
        Utf8JsonWriter writer,
        Content value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());
}

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

internal sealed class TypeConverter : JsonConverter<Type>
{
    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? typeName = reader.GetString();
        if (string.IsNullOrWhiteSpace(typeName))
            return null!; // or throw if null not allowed

        // Try to get the type from the string
        Type? type = Type.GetType(typeName, throwOnError: false);
        if (type == null)
            throw new JsonException($"Unable to resolve type '{typeName}'.");

        return type;
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Serialize as AssemblyQualifiedName for full resolution
        writer.WriteStringValue(value.AssemblyQualifiedName);
    }
}