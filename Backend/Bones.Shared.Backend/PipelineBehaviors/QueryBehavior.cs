using Bones.Shared.Backend.Models;
using FluentValidation;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <inheritdoc />
public class QueryBehavior<TRequest, TValue>(IEnumerable<IValidator<TRequest>> requestValidators)
    : PipelineBehaviorBase<TRequest, QueryResponse<TValue>>(requestValidators) where TRequest : notnull
{
    /// <inheritdoc />
    protected override (bool success, Dictionary<string, string[]>? failReason, bool forbidden) GetResult(QueryResponse<TValue> response)
    {
        return (response.Success, response.FailureReasons, response.Forbidden);
    }

    /// <inheritdoc />
    protected override QueryResponse<TValue> GetFailedResponse(Dictionary<string, string[]> failReason)
    {
        return QueryResponse<TValue>.Fail(failReason);
    }
}