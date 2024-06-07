using MediatR;

namespace Bones.Backend.HostedServices.Tasks;

internal abstract class TaskBase(ISender sender)
{
    internal abstract string TaskName { get; }

    internal abstract TaskFrequency Frequency { get; }

    internal bool Running { get; private set; } = false;

    protected ISender Sender { get; } = sender;

    protected readonly Random Random = new();

    internal abstract Task<bool> ShouldRunAsync(CancellationToken cancellationToken);

    internal abstract Task RunAsync(CancellationToken cancellationToken);
    internal abstract DateTimeOffset? GetNextRunTime();
}







