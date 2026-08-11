using System;
using System.Collections.Generic;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// The fixed ladder of CLR types <see cref="CsvSchema.InferAsync"/> classifies columns into, plus the
/// stable string tokens used to persist a <see cref="Type"/> in JSON (a raw <see cref="Type"/> is not
/// JSON-serializable). <c>string</c> is the implicit fallback and is not part of the ladder.
/// </summary>
internal static class CsvInferredTypes
{
    // Ordered most-restrictive first: a column takes the first ladder type every sampled value parses as.
    public static readonly IReadOnlyList<Type> Ladder = new[]
    {
        typeof(bool),
        typeof(int),
        typeof(long),
        typeof(decimal),
        typeof(DateTime),
        typeof(Guid),
    };



    public static string ToToken(Type type)
    {
        if (type == typeof(bool)) { return "bool"; }
        if (type == typeof(int)) { return "int"; }
        if (type == typeof(long)) { return "long"; }
        if (type == typeof(decimal)) { return "decimal"; }
        if (type == typeof(DateTime)) { return "datetime"; }
        if (type == typeof(Guid)) { return "guid"; }

        return "string";
    }



    public static Type FromToken(string? token)
    {
        switch (token)
        {
            case "bool": return typeof(bool);
            case "int": return typeof(int);
            case "long": return typeof(long);
            case "decimal": return typeof(decimal);
            case "datetime": return typeof(DateTime);
            case "guid": return typeof(Guid);
            default: return typeof(string);
        }
    }
}
