namespace Bones.Database.Operations.ProjectManagement.WorkItemLayouts.CreateWorkItemLayoutDb;

/// <summary>
///     DB Command for creating a layout.
/// </summary>
/// <param name="ProjectId">ID of the Project</param>
/// <param name="Name">Name of the layout</param>
public sealed record CreateWorkItemLayoutDbCommand(Guid ProjectId, string Name) : IRequest<CommandResponse>;