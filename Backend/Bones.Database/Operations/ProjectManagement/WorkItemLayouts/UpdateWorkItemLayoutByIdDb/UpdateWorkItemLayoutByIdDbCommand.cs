namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.UpdateWorkItemLayoutByIdDb;

/// <summary>
///     DB Command for updating a Layout.
/// </summary>
/// <param name="LayoutId">Internal ID of the layout</param>
/// <param name="NewName">The new name of the layout</param>
public sealed record UpdateWorkItemLayoutByIdDbCommand(Guid LayoutId, string NewName) : IRequest<CommandResponse>;