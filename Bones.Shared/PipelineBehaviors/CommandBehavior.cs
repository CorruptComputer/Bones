namespace Bones.Shared.PipelineBehaviors;

public class CommandBehavior<TRequest> : PipelineBehaviorBase<TRequest, CommandResponse> where TRequest : notnull
{
    protected override (bool success, string? failReason) GetResult(CommandResponse response)
    {
        return (response.Success, response.FailureReason);
    }

    protected override CommandResponse GetFailedResponse(string failReason)
    {
        return new()
        {
            Success = false,
            FailureReason = failReason
        };
    }
}