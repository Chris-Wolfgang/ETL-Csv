using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Serializes a <see cref="Type"/> from the inference ladder as a short stable token (e.g. <c>"int"</c>).
/// Manual read/write — no reflection, so it stays trim/AOT-safe.
/// </summary>
internal sealed class CsvClrTypeJsonConverter : JsonConverter<Type>
{
    // Runtime Type instances are a derived RuntimeType, so match the whole hierarchy.
    public override bool CanConvert(Type typeToConvert) => typeof(Type).IsAssignableFrom(typeToConvert);



    public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        CsvInferredTypes.FromToken(reader.GetString());



    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options) =>
        writer.WriteStringValue(CsvInferredTypes.ToToken(value));
}
