using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Serializes an <see cref="Encoding"/> by its <see cref="Encoding.WebName"/> (e.g. <c>"utf-8"</c>).
/// </summary>
internal sealed class CsvEncodingJsonConverter : JsonConverter<Encoding>
{
    // The runtime value is a concrete subclass (UTF8Encoding, UnicodeEncoding, ...), so match all of them.
    public override bool CanConvert(Type typeToConvert) => typeof(Encoding).IsAssignableFrom(typeToConvert);



    public override Encoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString();
        return string.IsNullOrEmpty(name) ? Encoding.UTF8 : Encoding.GetEncoding(name);
    }



    public override void Write(Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.WebName);
}
