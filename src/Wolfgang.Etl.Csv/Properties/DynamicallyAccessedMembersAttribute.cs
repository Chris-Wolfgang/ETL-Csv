#if !NET5_0_OR_GREATER

using System.ComponentModel;

// ReSharper disable once CheckNamespace
// Polyfills MUST declare the .NET runtime namespace so consumer code binds
// [DynamicallyAccessedMembers] to our shim on old TFMs and to the real BCL
// attribute on new ones. Folder location doesn't apply to polyfills.
namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Polyfill of <see cref="DynamicallyAccessedMembersAttribute"/> for target
/// frameworks where the type is internal (net462, netstandard2.0, netstandard2.1).
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
[ExcludeFromCodeCoverage]
[AttributeUsage
(
    AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Method
        | AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Interface
        | AttributeTargets.Parameter
        | AttributeTargets.GenericParameter
        | AttributeTargets.ReturnValue,
    Inherited = false
)]
internal sealed class DynamicallyAccessedMembersAttribute : Attribute
{
    public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
    {
        MemberTypes = memberTypes;
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // Mirrors the BCL attribute API — read reflectively by trimmer / analyzer
    // tooling on newer TFMs. Our own code never reads it, but removing the
    // accessor breaks the polyfill contract.
    public DynamicallyAccessedMemberTypes MemberTypes { get; }
}

#endif
