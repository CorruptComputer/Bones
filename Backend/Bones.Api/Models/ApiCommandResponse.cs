using System.Text.Json.Serialization;

namespace Bones.Api.Models;

/// <summary>
///     Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(ApiCommandResponse))]
public sealed record ApiCommandResponse
{
    /// <summary>
    ///     Was the command successful?
    /// </summary>
    public required bool Success { get; init; } = true;

    /// <summary>
    ///     If an ID was generated for something by the command, it can optionally be returned here.
    /// </summary>
    public Guid? Id { get; init; }

    /// <summary>
    ///     If the command failed, why?
    /// </summary>
    public string? FailureReason { get; init; }
}