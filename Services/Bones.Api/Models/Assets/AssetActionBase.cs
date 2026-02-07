using Bones.Api.Models.Assets.Actions;
using Bones.Database.DbSets.Accounts;

namespace Bones.Api.Models.Assets;

/// <summary>
///   An action that can be performed on an asset
/// </summary>
[JsonPolymorphic] // This shit doesn't work with NSwag, need to find an alternative
[JsonDerivedType(typeof(CreateAssetAction), nameof(CreateAssetAction))]
[JsonDerivedType(typeof(CreateAssetVersionAction), nameof(CreateAssetVersionAction))]
[JsonDerivedType(typeof(DeleteAssetAction), nameof(DeleteAssetAction))]
[JsonDerivedType(typeof(DeleteAssetVersionAction), nameof(DeleteAssetVersionAction))]
public abstract record AssetActionBase
{
    /// <summary>
    ///   The timestamp of when this action was performed
    /// </summary>
    public required DateTimeOffset ActionDateTime { get; init; }

    internal abstract Task<IRequest<CommandResponse>> ToInternalAsync(BonesUser user, ISender sender);

    internal abstract Task<AssetActionResponse> FromInternalAsync(CommandResponse result, BonesUser user, ISender sender);
}
