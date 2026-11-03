#if !NET7_0_OR_GREATER

using System.ComponentModel;

// ReSharper disable once CheckNamespace
// Polyfills MUST declare the .NET runtime namespace so consumer code binds
// [RequiresDynamicCode] to our shim on old TFMs and to the real BCL
// attribute on new ones. Folder location doesn't apply to polyfills.
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Polyfill of <see cref="RequiresDynamicCodeAttribute"/> for target
/// frameworks where the type does not exist (it ships in net7.0).
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
internal sealed class RequiresDynamicCodeAttribute : Attribute
{
    public RequiresDynamicCodeAttribute(string message)
    {
        Message = message;
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // Mirrors the BCL attribute API — read reflectively by trimmer / analyzer
    // tooling on newer TFMs. Our own code never reads it, but removing the
    // accessor breaks the polyfill contract.
    public string Message { get; }

    public string? Url { get; set; }
}

#endif
