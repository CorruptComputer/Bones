using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Models;

/// <summary>
///   Basic response of any request.
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
    ///   Was the command successful?
    /// </summary>
    [MemberNotNullWhen(returnValue: false, nameof(FailureReasons))]
    public required bool Success { get; init; }

    /// <summary>
    ///   Was this command blocked due to insufficient permissions?
    /// </summary>
    public bool Forbidden { get; init; } = false;

    /// <summary>
    ///   If the command failed, why?
    /// </summary>
    public Dictionary<string, List<string>>? FailureReasons { get; init; }

    /// <summary>
    ///   It's from the server, not an issue with the request
    /// </summary>
    public const string SERVER_ERROR_KEY = "server";

    /// <summary>
    ///   It's from the request, not a server issue
    /// </summary>
    public const string REQUEST_ERROR_KEY = "request";

    /// <summary>
    ///   Forbidden access was detected
    /// </summary>
    public const string FORBIDDEN_ERROR_VALUE = "Forbidden.";

    /// <summary>
    ///   Unauthenticated access was detected
    /// </summary>
    public const string UNAUTHENTICATED_ERROR_VALUE = "Unauthenticated.";

    /// <summary>
    ///   Something went wrong, and we don't know what, oops!
    /// </summary>
    public const string UNKNOWN_ERROR_VALUE = "Unknown failure reason";
}