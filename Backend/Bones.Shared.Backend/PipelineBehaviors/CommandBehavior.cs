using Bones.Shared.Backend.Models;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <inheritdoc />
public class CommandBehavior<TRequest> : PipelineBehaviorBase<TRequest, CommandResponse> where TRequest : notnull
{
    /// <inheritdoc />
    protected override (bool success, string? failReason) GetResult(CommandResponse response)
    {
        return (response.Success, response.FailureReason);
    }

    /// <inheritdoc />
    protected override CommandResponse GetFailedResponse(string failReason)
    {
        return CommandResponse.Fail(failReason);
    }
}