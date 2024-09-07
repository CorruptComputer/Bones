using Bones.Shared.Backend.Models;

namespace Bones.Shared.Backend.PipelineBehaviors;

/// <inheritdoc />
public class CommandBehavior<TRequest> : PipelineBehaviorBase<TRequest, CommandResponse> where TRequest : notnull
{
    /// <inheritdoc />
    protected override (bool success, string? failReason, bool forbidden) GetResult(CommandResponse response)
    {
        return (response.Success, response.FailureReason, response.Forbidden);
    }

    /// <inheritdoc />
    protected override CommandResponse GetFailedResponse(string failReason)
    {
        return CommandResponse.Fail(failReason);
    }
}