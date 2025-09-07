using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Models;

/// <summary>
///   Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(CommandResponse))]
public sealed record CommandResponse : BonesResponseBase
{
    private CommandResponse() { }

    /// <summary>
    ///   If an ID was generated for something by the command, it can optionally be returned here.
    /// </summary>
    public Dictionary<string, Guid> Ids { get; init; } = [];

    /// <summary>
    ///   Creates a successful response, optionally with an ID.
    /// </summary>
    /// <param name="idFor"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static CommandResponse Pass([NotNullIfNotNull(nameof(id))] string? idFor = null, Guid? id = null) => new()
    {
        Success = true,
        Ids = id is not null && idFor is not null
            ? new Dictionary<string, Guid> { { idFor, id.Value } }
            : []
    };

    /// <summary>
    ///   Creates a successful response, with multiple IDs
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public static CommandResponse Pass(Dictionary<string, Guid> ids) => new()
    {
        Success = true,
        Ids = ids
    };

    /// <summary>
    ///   Creates a failure response, optionally with the reasons why it failed.
    /// </summary>
    /// <param name="failureReasons"></param>
    /// <returns></returns>
    public static CommandResponse Fail(Dictionary<string, List<string>> failureReasons) => new()
    {
        Success = false,
        FailureReasons = failureReasons
    };

    /// <summary>
    ///   Creates a failure response, optionally with the reason why it failed.
    /// </summary>
    /// <param name="failureReason"></param>
    /// <returns></returns>
    public static CommandResponse Fail(string? failureReason = null) => new()
    {
        Success = false,
        FailureReasons = new()
        {
            { SERVER_ERROR_KEY, [ failureReason ?? UNKNOWN_ERROR_VALUE ] }
        }
    };

    /// <summary>
    ///   Creates a forbidden response.
    /// </summary>
    /// <returns></returns>
    public static CommandResponse Forbid() => new()
    {
        Success = false,
        FailureReasons = new()
        {
            { SERVER_ERROR_KEY, [ FORBIDDEN_ERROR_VALUE ] }
        },
        Forbidden = true
    };
}