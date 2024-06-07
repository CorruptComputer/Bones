using MediatR;

namespace Bones.Backend.HostedServices.Tasks.Variable;

internal abstract class VariableTaskBase(ISender sender) : TaskBase(sender)
{
    internal sealed override TaskFrequency Frequency => TaskFrequency.Variable;
}