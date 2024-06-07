using System.Text.Json.Serialization;

namespace Bones.Database.Models;

/// <summary>
///   Result from a query.
/// </summary>
/// <typeparam name="TResult"></typeparam>
[Serializable]
[JsonSerializable(typeof(DbQueryResponse<>))]
public sealed record DbQueryResponse<TResult>
{
    /// <summary>
    ///   Was the command successful?
    /// </summary>
    public required bool Success { get; init; } = true;

    /// <summary>
    ///   If the query was successful, this should have some data in it.
    /// </summary>
    public TResult? Result { get; init; } = default;

    /// <summary>
    ///   If the command failed, why?
    /// </summary>
    public string? FailureReason { get; init; } = null;

    /// <summary>
    ///   Translates a DbQueryResponse&lt;TResult&gt; into a TResult?
    /// </summary>
    /// <param name="response">The DbQueryResponse&lt;TResult&gt;</param>
    /// <returns>The newly translated TResult?</returns>
    public static implicit operator TResult?(DbQueryResponse<TResult> response)
    {
        if (response.Success == false || response.Result == null)
        {
            return default;
        }

        return response.Result;
    }

    /// <summary>
    ///   Translates a TResult? into a DbQueryResponse&lt;TResult&gt;
    /// </summary>
    /// <param name="result">The TResult? you want to translate</param>
    /// <returns>The newly translated DbQueryResponse&lt;TResult&gt;</returns>
    public static implicit operator DbQueryResponse<TResult>(TResult? result)
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