namespace Bones.Backend.Features.ProjectManagement.WorkItemLayouts;

/// <summary>
///     Command for creating a layout.
/// </summary>
/// <param name="ProjectId">Internal IDs of the project this layout should belong to</param>
/// <param name="Name">Name of the layout</param>
public record CreateLayoutCommand(Guid ProjectId, string Name) : IRequest<CommandResponse>;