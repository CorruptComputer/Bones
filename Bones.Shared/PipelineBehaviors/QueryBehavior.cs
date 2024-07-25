namespace Bones.Shared.PipelineBehaviors;

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