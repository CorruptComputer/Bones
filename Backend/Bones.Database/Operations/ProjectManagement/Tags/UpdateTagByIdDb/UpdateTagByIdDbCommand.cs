namespace Bones.Database.Operations.ProjectManagement.Tags.UpdateTagByIdDb;

/// <summary>
///     DB Command for updating a Tag.
/// </summary>
/// <param name="TagId">Internal ID of the tag</param>
/// <param name="Name">The new name of the tag</param>
public record UpdateTagByIdDbCommand(Guid TagId, string Name) : IRequest<CommandResponse>;