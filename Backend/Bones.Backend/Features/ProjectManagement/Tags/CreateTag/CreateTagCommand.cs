namespace Bones.Backend.Features.ProjectManagement.Tags.CreateTag;

/// <summary>
///     Command for creating a tag.
/// </summary>
/// <param name="Name">Name of the tag</param>
public sealed record CreateTagCommand(string Name) : IRequest<CommandResponse>;