using System.Text.Json.Serialization;

namespace Bones.Database.Models;

/// <summary>
///   Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(DbCommandResponse))]
public sealed record DbCommandResponse
{
    /// <summary>
    ///   Was the command successful?
    /// </summary>
    public required bool Success { get; init; } = true;

    /// <summary>
    ///   If an ID was generated for something by the command, it can optionally be returned here.
    /// </summary>
    public long? Id { get; init; } = null;

    /// <summary>
    ///   If the command failed, why?
    /// </summary>
    public string? FailureReason { get; init; } = null;
}
