using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Models;

/// <summary>
///     Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(CommandResponse))]
public sealed record CommandResponse
{
    private CommandResponse() { }

    /// <summary>
    ///     Was the command successful?
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    ///     If an ID was generated for something by the command, it can optionally be returned here.
    /// </summary>
    public Guid? Id { get; init; }

    /// <summary>
    ///     If the command failed, why?
    /// </summary>
    public string? FailureReason { get; init; }

    /// <summary>
    ///   Creates a successful response, optionally with an ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static CommandResponse Pass(Guid? id = null) => new()
    {
        Success = false,
        Id = id
    };

    /// <summary>
    ///   Creates a failure response, optionally with the reason why it failed.
    /// </summary>
    /// <param name="failureReason"></param>
    /// <returns></returns>
    public static CommandResponse Fail(string? failureReason = null) => new()
    {
        Success = false,
        FailureReason = failureReason
    };
}