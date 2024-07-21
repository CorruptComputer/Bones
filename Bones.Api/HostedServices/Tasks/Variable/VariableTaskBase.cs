namespace Bones.Api.HostedServices.Tasks.Variable;

internal abstract class VariableTaskBase(ISender sender) : TaskBase(sender)
{
    internal sealed override TaskFrequency Frequency => TaskFrequency.Variable;
}