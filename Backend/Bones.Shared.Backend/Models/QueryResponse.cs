using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Bones.Shared.Backend.Models;

/// <summary>
///     Result from a query.
/// </summary>
/// <typeparam name="TResult"></typeparam>
[Serializable]
[JsonSerializable(typeof(QueryResponse<>))]
public sealed record QueryResponse<TResult> : BonesResponseBase
{
    private QueryResponse() { }

    /// <summary>
    ///     If the query was successful, this should have some data in it.
    /// </summary>
    public TResult? Result { get; init; }

    /// <summary>
    ///   Creates a successful response, with the required result
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static QueryResponse<TResult> Pass(TResult result) => new()
    {
        Success = true,
        Result = result
    };

    /// <summary>
    ///   Creates a failure response, optionally with the reasons why it failed.
    /// </summary>
    /// <param name="failureReasons"></param>
    /// <returns></returns>
    public static QueryResponse<TResult> Fail(Dictionary<string, string[]>? failureReasons = null) => new()
    {
        Success = false,
        FailureReasons = failureReasons
    };
    
    /// <summary>
    ///   Creates a failure response, optionally with the reason why it failed.
    /// </summary>
    /// <param name="failureReason"></param>
    /// <returns></returns>
    public static QueryResponse<TResult> Fail(string? failureReason = null) => new()
    {
        Success = false,
        FailureReasons = string.IsNullOrEmpty(failureReason) ? null : new()
        {
            { "Failure", [ failureReason ] }
        }
    };

    /// <summary>
    ///   Creates a forbidden response.
    /// </summary>
    /// <returns></returns>
    public static QueryResponse<TResult> Forbid() => new()
    {
        Success = false,
        FailureReasons = new()
        {
            {"Forbidden", [ "Forbidden." ] }
        },
        Forbidden = true
    };

    /// <summary>
    ///     Translates a QueryResponse&lt;TResult&gt; into a TResult?
    /// </summary>
    /// <param name="response">The QueryResponse&lt;TResult&gt;</param>
    /// <returns>The newly translated TResult?</returns>
    public static implicit operator TResult?(QueryResponse<TResult> response)
    {
        if (!response.Success || response.Result == null)
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
    public static implicit operator QueryResponse<TResult>(TResult? result)
    {
        if (result == null)
        {
            return new()
            {
                Success = false
            };
        }

        if (result is IdentityResult identityResult)
        {
            return new()
            {
                Success = identityResult.Succeeded,
                Result = result
            };
        }

        if (result is SignInResult signInResult)
        {
            return new()
            {
                Success = signInResult.Succeeded,
                Result = result
            };
        }

        return new()
        {
            Success = true,
            Result = result
        };
    }
}