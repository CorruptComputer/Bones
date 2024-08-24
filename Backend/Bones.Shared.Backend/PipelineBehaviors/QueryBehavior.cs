using Bones.Shared.Backend.Models;

namespace Bones.Shared.Backend.PipelineBehaviors;

public class QueryBehavior<TRequest, TValue> : PipelineBehaviorBase<TRequest, QueryResponse<TValue>> where TRequest : notnull
{
    protected override (bool success, string? failReason) GetResult(QueryResponse<TValue> response)
    {
        return (response.Success, response.FailureReason);
    }

    protected override QueryResponse<TValue> GetFailedResponse(string failReason)
    {
        return new()
        {
            Success = false,
            FailureReason = failReason
        };
    }
}