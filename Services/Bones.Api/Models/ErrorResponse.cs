namespace Bones.Api.Models;

/// <summary>
///   The response body is empty, this is a workaround for the limitations of the API client.
/// </summary>
[Serializable]
[JsonSerializable(typeof(ErrorResponse))]
public sealed record ErrorResponse
{
    /// <summary>
    ///   Creates an error response
    /// </summary>
    /// <param name="errors"></param>
    /// <param name="errorMessage"></param>
    public ErrorResponse(Dictionary<string, List<string>>? errors = null, string? errorMessage = null)
    {
        Errors = errors;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    ///   Creates an error response from a failed CommandResponse
    /// </summary>
    /// <param name="failedCommandResponse"></param>
    /// <returns></returns>
    public static ErrorResponse FromCommandResponse(CommandResponse failedCommandResponse) => new(failedCommandResponse.FailureReasons);

    /// <summary>
    ///   Creates an error response from a failed QueryResponse
    /// </summary>
    /// <param name="failedQueryResponse"></param>
    /// <returns></returns>
    public static ErrorResponse FromQueryResponse<T>(QueryResponse<T> failedQueryResponse) => new(failedQueryResponse.FailureReasons);

    /// <summary>
    ///   The errors that occurred, with the key being either the input that was invalid and the list of reasons it was invalid, or 'server' with the list of server errors.
    /// </summary>
    public Dictionary<string, List<string>>? Errors { get; init; }

    /// <summary>
    ///   The error message, if any.
    /// </summary>
    public string? ErrorMessage { get; init; }
}