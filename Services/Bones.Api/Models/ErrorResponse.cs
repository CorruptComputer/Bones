using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Models;

/// <summary>
///   The response body is empty, this is a workaround for the limitations of the API client.
/// </summary>
[Serializable]
[JsonSerializable(typeof(ErrorResponse))]
public sealed record ErrorResponse
{
    /// <summary>
    ///   Creates an error response with an optional message
    /// </summary>
    /// <param name="errorMessage"></param>
    public ErrorResponse(string? errorMessage = null)
    {
        ErrorMessage = errorMessage;
    }

    /// <summary>
    ///   Creates an error response from a dictionary of errors
    /// </summary>
    /// <param name="errors"></param>
    public ErrorResponse(Dictionary<string, List<string>>? errors)
    {
        Errors = errors?.ToList();
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
    ///   Gets the errors from an IdentityResult in a format that we can return.
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static ErrorResponse FromIdentityResult(IdentityResult result)
    {
        Dictionary<string, List<string>> errors = [];

        foreach (IdentityError error in result.Errors)
        {
            if (errors.ContainsKey(error.Code))
            {
                errors[error.Code].Add(error.Description);
            }
            else
            {
                errors[error.Code] = [error.Description];
            }
        }

        return new(errors);
    }

    /// <summary>
    ///   The errors that occurred, with the key being either the input that was invalid and the list of reasons it was invalid, or 'server' with the list of server errors.
    /// </summary>
    public List<KeyValuePair<string, List<string>>>? Errors { get; init; }

    /// <summary>
    ///   The error message, if any.
    /// </summary>
    public string? ErrorMessage { get; init; }
}