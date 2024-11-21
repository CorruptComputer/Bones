namespace Bones.Database.Operations.GenericItem.ItemLayouts.CreateItemLayoutDb;

/// <summary>
///   DB Command for creating an Item Layout
/// </summary>
/// <param name="Name"></param>
/// <param name="ProjectId"></param>
public sealed record CreateItemLayoutDbCommand(string Name, Guid ProjectId) : IRequest<CommandResponse>;