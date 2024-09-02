using Bones.Shared.Backend.Enums;

namespace Bones.Database.Operations.AssetManagement.AssetLayouts.CreateAssetLayoutDb;

/// <summary>
///   DB Command for creating an Asset Layout
/// </summary>
/// <param name="OwnershipType"></param>
/// <param name="OwnerId"></param>
/// <param name="Name"></param>
public sealed record CreateAssetLayoutDbCommand(OwnershipType OwnershipType, Guid OwnerId, string Name) : IRequest<CommandResponse>;