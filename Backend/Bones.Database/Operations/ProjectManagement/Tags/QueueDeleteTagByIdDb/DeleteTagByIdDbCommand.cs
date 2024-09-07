namespace Bones.Database.Operations.ProjectManagement.Tags.QueueDeleteTagByIdDb;

/// <summary>
///     DB Command for deleting a Tag.
/// </summary>
/// <param name="TagId">Internal ID of the tag</param>
public sealed record DeleteTagByIdDbCommand(Guid TagId) : IRequest<CommandResponse>;