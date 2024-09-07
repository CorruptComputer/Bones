using System.Text.Json.Serialization;

namespace Bones.Shared.Backend.Models;

/// <summary>
///     Basic response of any command.
/// </summary>
[Serializable]
[JsonSerializable(typeof(CommandResponse))]
public sealed record CommandResponse : BonesResponseBase
{
    private CommandResponse() { }
    
    /// <summary>
    ///     If an ID was generated for something by the command, it can optionally be returned here.
    /// </summary>
    public Guid? Id { get; init; }
    
    /// <summary>
    ///   Creates a successful response, optionally with an ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static CommandResponse Pass(Guid? id = null) => new()
    {
        Success = true,
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
    
    /// <summary>
    ///   Creates a forbidden response.
    /// </summary>
    /// <returns></returns>
    public static CommandResponse Forbid() => new()
    {
        Success = false,
        FailureReason = "Forbidden.",
        Forbidden = true
    };
}