using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.Assets.Actions;

/// <summary>
///   Action to delete a specific version of an asset.
/// </summary>
[JsonSerializable(typeof(DeleteAssetVersionAction))]
public sealed record class DeleteAssetVersionAction : AssetActionBase
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
