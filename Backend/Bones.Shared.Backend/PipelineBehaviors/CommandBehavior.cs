using Bones.Shared.Backend.Models;
using FluentValidation;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <inheritdoc />
public class CommandBehavior<TRequest>(IEnumerable<IValidator<TRequest>> requestValidators)
    : PipelineBehaviorBase<TRequest, CommandResponse>(requestValidators) where TRequest : notnull
{
    /// <inheritdoc />
    protected override (bool success, Dictionary<string, string[]>? failReason, bool forbidden) GetResult(CommandResponse response)
    {
        return (response.Success, response.FailureReasons, response.Forbidden);
    }

    /// <inheritdoc />
    protected override CommandResponse GetFailedResponse(Dictionary<string, string[]> failReason)
    {
        return CommandResponse.Fail(failReason);
    }
}