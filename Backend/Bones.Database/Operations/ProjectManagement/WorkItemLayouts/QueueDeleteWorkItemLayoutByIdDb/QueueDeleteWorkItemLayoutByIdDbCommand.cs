namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.QueueDeleteWorkItemLayoutByIdDb;

/// <summary>
///     DB Command for deleting a Layout.
/// </summary>
/// <param name="LayoutId">Internal ID of the layout</param>
public sealed record QueueDeleteWorkItemLayoutByIdDbCommand(Guid LayoutId) : IRequest<CommandResponse>;