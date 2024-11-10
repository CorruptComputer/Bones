using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Models;

/// <summary>
///     Basic response of any request.
/// </summary>
[Serializable]
[JsonSerializable(typeof(BonesResponseBase))]
public record BonesResponseBase
{
    /// <summary>
    ///   Base constructor for this response base
    /// </summary>
    protected BonesResponseBase() { }

    /// <summary>
    ///     Was the command successful?
    /// </summary>
    [MemberNotNullWhen(returnValue: false, nameof(FailureReasons))]
    public required bool Success { get; init; }

    /// <summary>
    ///   Was this command blocked due to insufficient permissions?
    /// </summary>
    public bool Forbidden { get; init; } = false;

    /// <summary>
    ///     If the command failed, why?
    /// </summary>
    public Dictionary<string, List<string>>? FailureReasons { get; init; }
}