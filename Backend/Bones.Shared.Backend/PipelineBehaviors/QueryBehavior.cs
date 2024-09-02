using Bones.Shared.Backend.Models;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <inheritdoc />
public class QueryBehavior<TRequest, TValue> : PipelineBehaviorBase<TRequest, QueryResponse<TValue>> where TRequest : notnull
{
    /// <inheritdoc />
    protected override (bool success, string? failReason) GetResult(QueryResponse<TValue> response)
    {
        return (response.Success, response.FailureReason);
    }

    /// <inheritdoc />
    protected override QueryResponse<TValue> GetFailedResponse(string failReason)
    {
        return new()
        {
            Success = false,
            FailureReason = failReason
        };
    }
}