using System.Text.Json.Serialization;

namespace Bones.Api.Models;

/// <summary>
///     Result from a query.
/// </summary>
/// <typeparam name="TResult"></typeparam>
[Serializable]
[JsonSerializable(typeof(ApiQueryResponse<>))]
public sealed record ApiQueryResponse<TResult>
{
    /// <summary>
    ///     Was the command successful?
    /// </summary>
    public required bool Success { get; init; } = true;

    /// <summary>
    ///     If the query was successful, this should have some data in it.
    /// </summary>
    public TResult? Result { get; init; }

    /// <summary>
    ///     If the command failed, why?
    /// </summary>
    public string? FailureReason { get; init; } = null;

    /// <summary>
    ///     Translates a QueryResponse&lt;TResult&gt; into a TResult?
    /// </summary>
    /// <param name="response">The QueryResponse&lt;TResult&gt;</param>
    /// <returns>The newly translated TResult?</returns>
    public static implicit operator TResult?(ApiQueryResponse<TResult> response)
    {
        if (response.Success == false || response.Result == null)
        {
            return default;
        }

        return response.Result;
    }

    /// <summary>
    ///     Translates a TResult? into a QueryResponse&lt;TResult&gt;
    /// </summary>
    /// <param name="result">The TResult? you want to translate</param>
    /// <returns>The newly translated QueryResponse&lt;TResult&gt;</returns>
    public static implicit operator ApiQueryResponse<TResult>(TResult? result)
    {
        if (result == null)
        {
            return new()
            {
                Success = false
            };
        }

        return new()
        {
            Success = true,
            Result = result
        };
    }
}