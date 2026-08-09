using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Wolfgang.Etl.Csv;

/// <summary>
/// Fluent, type-safe, code-first builder for a CSV column mapping — an alternative to
/// the <see cref="CsvColumnAttribute"/> for record types you cannot decorate (third-party
/// or generated POCOs) or layouts assembled at runtime.
/// </summary>
/// <remarks>
/// <para>
/// The builder produces the same <see cref="CsvColumnMap"/> leaf type the attribute path
/// resolves to, so a code-built schema is behaviorally identical to the equivalent
/// <c>[CsvColumn]</c>-decorated layout. Assign the result to
/// <see cref="CsvExtractor{TRecord}.ColumnMaps"/> or <see cref="CsvLoader{TRecord}.ColumnMaps"/>;
/// a non-empty <c>ColumnMaps</c> overrides attribute resolution, and leaving it unset keeps
/// today's attribute behavior (backward compatible).
/// </para>
/// <para>
/// Properties are selected with an <see cref="Expression{TDelegate}"/> (<c>x =&gt; x.Name</c>),
/// so the mapping is compile-time checked and survives a rename/refactor.
/// </para>
/// <example>
/// <code>
/// var maps = new CsvSchemaBuilder&lt;Order&gt;()
///     .Column(o =&gt; o.Id,     name: "order_id")
///     .Column(o =&gt; o.Placed, name: "placed_at", format: "yyyy-MM-dd")
///     .Column(o =&gt; o.Total,  name: "amount",    format: "0.00")
///     .Build();
///
/// var extractor = new CsvExtractor&lt;Order&gt;(reader) { ColumnMaps = maps };
/// </code>
/// </example>
/// </remarks>
/// <typeparam name="T">The record type the schema maps.</typeparam>
public sealed class CsvSchemaBuilder<T>
{
    private readonly List<CsvColumnMap> _columns = new();


    /// <summary>
    /// Maps a property of <typeparamref name="T"/> to a CSV column.
    /// </summary>
    /// <typeparam name="TProperty">The selected property's type.</typeparam>
    /// <param name="selector">
    /// A property selector, e.g. <c>x =&gt; x.Name</c>. Must be a direct access of a public
    /// property of <typeparamref name="T"/> (an inherited property is fine) — not a field,
    /// method, nested/chained member, or computed expression.
    /// </param>
    /// <param name="name">
    /// The CSV column name to bind to. Ignored when <paramref name="index"/> is non-negative.
    /// </param>
    /// <param name="index">The 0-based column index; <c>-1</c> (default) binds by name only.</param>
    /// <param name="format">Parse/format string (e.g. <c>"yyyy-MM-dd"</c>).</param>
    /// <param name="optional">When <c>true</c>, a missing column does not fail the read.</param>
    /// <param name="default">Default value used when the column is absent or empty during reading.</param>
    /// <returns>The same builder instance, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="selector"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="selector"/> does not reference a public property of <typeparamref name="T"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than <c>-1</c>.</exception>
    public CsvSchemaBuilder<T> Column<TProperty>
    (
        Expression<Func<T, TProperty>> selector,
        string? name = null,
        int index = -1,
        string? format = null,
        bool optional = false,
        string? @default = null
    )
    {
        var propertyName = ResolvePropertyName(selector);

        if (index < -1)
        {
            throw new ArgumentOutOfRangeException
            (
                nameof(index),
                index,
                "Index must be -1 (bind by name) or a non-negative column index."
            );
        }

        _columns.Add
        (
            new CsvColumnMap(propertyName)
            {
                Name = name,
                Index = index,
                Optional = optional,
                Format = format,
                Default = @default,
            }
        );

        return this;
    }


    /// <summary>
    /// Builds the immutable column-map list. Assign it to the extractor's or loader's
    /// <c>ColumnMaps</c> property.
    /// </summary>
    /// <returns>The mapped columns, in the order they were added.</returns>
    public IReadOnlyList<CsvColumnMap> Build() => Array.AsReadOnly(_columns.ToArray());


    // The one genuinely shared primitive across the ETL family (lambda -> PropertyInfo).
    // Kept local for now; a future move to Wolfgang.Etl.Abstractions (already a shared
    // dependency) would let the sibling schema builders share exactly this and nothing else.
    private static string ResolvePropertyName<TProperty>(Expression<Func<T, TProperty>> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var body = selector.Body;

        // A value-type property selected as Func<T, object> is wrapped in a boxing Convert.
        if (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
        {
            body = unary.Operand;
        }

        if (body is not MemberExpression member
            || member.Member is not PropertyInfo property
            || member.Expression is not ParameterExpression)
        {
            throw new ArgumentException
            (
                $"Selector must be a direct access of a public property of {typeof(T).Name} (e.g. x => x.Name).",
                nameof(selector)
            );
        }

        return property.Name;
    }
}
