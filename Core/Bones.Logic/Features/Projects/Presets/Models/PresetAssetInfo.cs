using System.Diagnostics.CodeAnalysis;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetAssetInfo
{
    internal required string Title { get; init; }

    internal required PresetLayoutInfo Layout { get; init; }

    internal required Dictionary<PresetFieldInfo, object?> Fields { get; init; }

    [MemberNotNullWhen(true, nameof(AssetId))]
    [MemberNotNullWhen(true, nameof(AssetVersionId))]
    internal bool Created => AssetId.HasValue && AssetVersionId.HasValue;

    /// <summary>
    ///   After the asset is created, this will be set to the ID of the asset
    /// </summary>
    internal Guid? AssetId { get; set; }

    /// <summary>
    ///   After the asset is created, this will be set to the ID of the asset version
    /// </summary>
    internal Guid? AssetVersionId { get; set; }
}
