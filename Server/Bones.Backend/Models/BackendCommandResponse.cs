using System.Text.Json.Serialization;
using Bones.Database.Models;

namespace Bones.Backend.Models;

/// <summary>
///     Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(BackendCommandResponse))]
public sealed record BackendCommandResponse
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

    /// <summary>
    ///     Translates a DbCommandResponse into a CommandResponse.
    /// </summary>
    /// <param name="dbResponse">The DbCommandResponse.</param>
    /// <returns>The newly translated CommandResponse.</returns>
    public static implicit operator BackendCommandResponse(DbCommandResponse dbResponse)
    {
        return new()
        {
            Success = dbResponse.Success,
            Id = dbResponse.Id,
            FailureReason = dbResponse.FailureReason
        };
    }
}