using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Models;

/// <summary>
///     Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(CommandResponse))]
public sealed record CommandResponse
{
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
}