namespace Bones.Database.Operations.AssetManagement.AssetLayouts.UpdateAssetLayoutByIdDb;

/// <summary>
///   DB Command for updating an Asset Layout
/// </summary>
/// <param name="AssetLayoutId"></param>
/// <param name="NewName"></param>
public sealed record UpdateAssetLayoutByIdDbCommand(Guid AssetLayoutId, string NewName) : IRequest<CommandResponse>;