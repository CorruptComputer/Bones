using Bones.Database.DbSets.AccountManagement;

namespace Bones.Api.Models.Assets.Actions;

/// <summary>
///   Action to delete an asset.
/// </summary>
[JsonSerializable(typeof(DeleteAssetAction))]
public sealed record class DeleteAssetAction : AssetActionBase
{
    /// <summary>
    ///   The ID of the asset to perform the action on
    /// </summary>
    public required Guid AssetId { get; init; }

    internal override Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }

    internal override Task<AssetActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender)
    {
        throw new NotImplementedException();
    }
}
