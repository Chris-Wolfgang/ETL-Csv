#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP3_0_OR_GREATER

using System.ComponentModel;

// ReSharper disable once CheckNamespace
// Polyfills MUST declare the .NET runtime namespace so the compiler's nullable flow analysis
// recognises [NotNullWhen] on old TFMs and binds to the real BCL attribute on new ones.
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Polyfill of <see cref="NotNullWhenAttribute"/> for target frameworks where the type does not
/// exist (it ships in netstandard2.1 / netcoreapp3.0).
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
internal sealed class NotNullWhenAttribute : Attribute
{
    public NotNullWhenAttribute(bool returnValue)
    {
        ReturnValue = returnValue;
    }

    // Mirrors the BCL attribute API; read by the compiler, never by our own code.
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public bool ReturnValue { get; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
}

#endif
