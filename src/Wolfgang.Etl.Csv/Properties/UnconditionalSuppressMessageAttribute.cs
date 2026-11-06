#if !NET5_0_OR_GREATER

using System.ComponentModel;

// ReSharper disable once CheckNamespace
// Polyfills MUST declare the .NET runtime namespace so consumer code binds
// [UnconditionalSuppressMessage] to our shim on old TFMs and to the real BCL
// attribute on new ones. Folder location doesn't apply to polyfills.
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Polyfill of <see cref="UnconditionalSuppressMessageAttribute"/> for target
/// frameworks where the type is internal.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
internal sealed class UnconditionalSuppressMessageAttribute : Attribute
{
    public UnconditionalSuppressMessageAttribute(string category, string checkId)
    {
        Category = category;
        CheckId = checkId;
    }

    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // The following accessors mirror the BCL attribute API — read reflectively
    // by trimmer / analyzer tooling on newer TFMs. Our own code never reads
    // them, but removing them breaks the polyfill contract.
    public string Category { get; }

    public string CheckId { get; }

    public string? Scope { get; set; }

    public string? Target { get; set; }

    public string? MessageId { get; set; }

    public string? Justification { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
}

#endif
