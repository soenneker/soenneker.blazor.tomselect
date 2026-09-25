using Soenneker.Blazor.TomSelect.Base;
using Soenneker.Blazor.TomSelect.Configuration;
using Soenneker.Blazor.TomSelect.Dtos;
using System.Collections.Generic;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace Soenneker.Blazor.TomSelect;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, ReadCommentHandling = JsonCommentHandling.Skip, UseStringEnumConverter = true, Converters = new[] { typeof(AddItemTypeMetadataConverter), typeof(TomSelectPluginTypeMetadataConverter) })]
[JsonSerializable(typeof(TomSelectConfiguration))]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(System.Text.Json.JsonElement))]
[JsonSerializable(typeof(System.Collections.Generic.Dictionary<string, object?>))]
[JsonSerializable(typeof(System.Collections.Generic.List<object?>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(object[]))]
internal partial class LibraryJsonContext : JsonSerializerContext
{
    internal static JsonTypeInfo<T> Get<T>() =>
        (JsonTypeInfo<T>)(Default.GetTypeInfo(typeof(T)) ?? throw new NotSupportedException($"No generated JSON metadata for {typeof(T)}."));

    internal static JsonSerializerOptions WithContext(JsonSerializerContext? additionalContext)
    {
        JsonSerializerOptions defaults = Get<object>().Options;
        if (additionalContext is null)
            return defaults;
        var options = new JsonSerializerOptions(defaults)
        {
            TypeInfoResolver = JsonTypeInfoResolver.Combine(defaults.TypeInfoResolver!, additionalContext)
        };
        options.MakeReadOnly();
        return options;
    }
}

internal sealed class AddItemTypeMetadataConverter : JsonConverter<Soenneker.Blazor.TomSelect.Enums.AddItemType>
{
    public override Soenneker.Blazor.TomSelect.Enums.AddItemType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String && Soenneker.Blazor.TomSelect.Enums.AddItemType.TryFromValue(reader.GetString(), out var value) ? value : throw new JsonException("Unknown AddItemType value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Blazor.TomSelect.Enums.AddItemType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}

internal sealed class TomSelectPluginTypeMetadataConverter : JsonConverter<Soenneker.Blazor.TomSelect.Enums.TomSelectPluginType>
{
    public override Soenneker.Blazor.TomSelect.Enums.TomSelectPluginType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.String && Soenneker.Blazor.TomSelect.Enums.TomSelectPluginType.TryFromValue(reader.GetString(), out var value) ? value : throw new JsonException("Unknown TomSelectPluginType value.");

    public override void Write(Utf8JsonWriter writer, Soenneker.Blazor.TomSelect.Enums.TomSelectPluginType value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
