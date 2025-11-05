using Bones.Database.DbSets.Items;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   Response for the GetProjectSettingsAsync endpoint
/// </summary>
[JsonSerializable(typeof(GetProjectSettingsResponse))]
public record GetProjectSettingsResponse
{
    /// <summary>
    ///   The projects ID
    /// </summary>
    [JsonRequired]
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The name of the project
    /// </summary>
    [JsonRequired]
    public required string ProjectName { get; init; }

    /// <summary>
    ///   The type of owner
    /// </summary>
    [JsonRequired]
    public required OwnershipType OwnerType { get; init; }

    /// <summary>
    ///   The ID of the owner
    /// </summary>
    [JsonRequired]
    public required Guid OwnerId { get; init; }

    /// <summary>
    ///   The DisplayName of the owner
    /// </summary>
    [JsonRequired]
    public required string OwnerDisplayName { get; init; }

    /// <summary>
    ///   The total number of item fields in the project
    /// </summary>
    [JsonRequired]
    public required int ItemFieldCount { get; init; }

    /// <summary>
    ///   A list of the item fields in the project
    /// </summary>
    [JsonRequired]
    public required IEnumerable<ItemFieldModel> ItemFields { get; init; }

    /// <summary>
    ///   The total number of item layouts in the project
    /// </summary>
    [JsonRequired]
    public required int ItemLayoutCount { get; init; }

    /// <summary>
    ///   A list of the item layouts in the project
    /// </summary>
    [JsonRequired]
    public required IEnumerable<ItemLayoutModel> ItemLayouts { get; init; }

    internal static GetProjectSettingsResponse FromInternal(Database.DbSets.ProjectManagement.Project project, List<ItemField> itemFields, List<ItemLayout> itemLayouts)
    {
        // We know they won't be null
        Guid ownerId = project.OwnerType == OwnershipType.User
            ? project.OwningUser!.Id
            : project.OwningOrganization!.Id;

        string ownerDisplayName = project.OwnerType == OwnershipType.User
            // Default it to "Unknown", if someone hasn't set it yet and sees that it'll probably prompt them to add it
            ? project.OwningUser!.DisplayName ?? "Unknown"
            : project.OwningOrganization!.Name;

        return new()
        {
            ProjectId = project.Id,
            ProjectName = project.Name,
            OwnerType = project.OwnerType,
            OwnerId = ownerId,
            OwnerDisplayName = ownerDisplayName,
            ItemFieldCount = itemFields.Count,
            ItemFields = itemFields.Select(i =>
                new ItemFieldModel
                {
                    ItemFieldId = i.Id,
                    ItemFieldLatestVersionId = i.LatestVersion!.Id,
                    Name = i.LatestVersion.Name,
                    IsRequired = i.LatestVersion.IsRequired,
                    Type = i.LatestVersion.Type
                }),
            ItemLayoutCount = itemLayouts.Count,
            ItemLayouts = itemLayouts.Select(i =>
                new ItemLayoutModel
                {
                    ItemLayoutId = i.Id,
                    Name = i.LatestVersion!.Name,
                    LayoutUse = i.LatestVersion!.LayoutUse.ToString()
                })
        };
    }

    /// <summary>
    ///   Model for an item field in a projects settings
    /// </summary>
    [JsonSerializable(typeof(ItemFieldModel))]
    public record ItemFieldModel
    {
        /// <summary>
        ///   Internal ID for the ItemField
        /// </summary>
        [JsonRequired]
        public required Guid ItemFieldId { get; init; }

        /// <summary>
        ///   Internal ID for the current version ItemFieldVersion
        /// </summary>
        [JsonRequired]
        public required Guid ItemFieldLatestVersionId { get; init; }

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

    /// <summary>
    ///   Model for an item layout in a projects settings
    /// </summary>
    [JsonSerializable(typeof(ItemLayoutModel))]
    public record ItemLayoutModel
    {
        /// <summary>
        ///   Internal ID for the ItemLayout
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
        public required string LayoutUse { get; init; }
    }
}
