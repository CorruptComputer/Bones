using Bones.Shared.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   Response for the GetItemLayoutAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetProjectItemLayoutResponse))]
public record GetProjectItemLayoutResponse
{
    /// <summary>
    ///     Internal ID for the ItemLayout
    /// </summary>
    [JsonRequired]
    public required Guid ItemLayoutId { get; init; }

    /// <summary>
    ///   The name for this Item layout
    /// </summary>
    [JsonRequired]
    public required string Name { get; init; }

    /// <summary>
    ///   The use this layout is applicable to
    /// </summary>
    [JsonRequired]
    public required ItemLayoutUse LayoutUse { get; init; }

    /// <summary>
    ///   The current version of this layout
    /// </summary>
    [JsonRequired]
    public required ItemLayoutVersionModel CurrentVersion { get; init; }

    /// <summary>
    ///   Model for an item layout version
    /// </summary>
    [JsonSerializable(typeof(ItemLayoutVersionModel))]
    public record ItemLayoutVersionModel
    {
        /// <summary>
        ///     Internal ID for the ItemLayoutVersion
        /// </summary>
        [JsonRequired]
        public required Guid ItemLayoutVersionId { get; init; }

        /// <summary>
        ///   The version number for this
        /// </summary>
        [JsonRequired]
        public required long Version { get; init; }

        /// <summary>
        ///   The time which this was created
        /// </summary>
        [JsonRequired]
        public required DateTimeOffset CreateDateTime { get; init; }

        /// <summary>
        ///   The fields associated with this layout version
        /// </summary>
        [JsonRequired]
        public required List<ItemFieldModel> Fields { get; init; }

        /// <summary>
        ///   Disables creating of new items using this layout version,
        ///   and when all items using it are deleted it will be removed.
        /// </summary>
        [JsonRequired]
        public required bool DeleteFlag { get; init; }
    }

    /// <summary>
    ///   Model for an item field in a projects item layout settings
    /// </summary>
    [JsonSerializable(typeof(ItemFieldModel))]
    public record ItemFieldModel
    {
        /// <summary>
        ///     Internal ID for the ItemField
        /// </summary>
        [JsonRequired]
        public required Guid ItemFieldId { get; init; }

        /// <summary>
        ///   The name of this ItemField
        /// </summary>
        [JsonRequired]
        public required string Name { get; init; }

        /// <summary>
        ///   Is this field required to have a value?
        /// </summary>
        [JsonRequired]
        public required bool IsRequired { get; init; }

        /// <summary>
        ///   The FieldType for this field
        /// </summary>
        [JsonRequired]
        public required FieldType Type { get; init; }
    }
}
