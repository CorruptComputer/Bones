using Bones.Database.DbSets.GenericItems.GenericItemFields;
using Bones.Database.DbSets.GenericItems.GenericItemLayouts;
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

    internal static GetProjectSettingsResponse FromInternal(Database.DbSets.ProjectManagement.Project project, List<GenericItemField> itemFields, List<GenericItemLayout> itemLayouts)
    {
        return new()
        {
            ProjectId = project.Id,
            ProjectName = project.Name,
            ItemFieldCount = itemFields.Count,
            ItemFields = itemFields.Select(i =>
                new ItemFieldModel
                {
                    ItemFieldId = i.Id,
                    ItemFieldCurrentVersionId = i.CurrentVersion.Id,
                    Name = i.CurrentVersion.Name,
                    IsRequired = i.CurrentVersion.IsRequired,
                    Type = i.CurrentVersion.Type
                }),
            ItemLayoutCount = itemLayouts.Count,
            ItemLayouts = itemLayouts.Select(i =>
                new ItemLayoutModel
                {
                    ItemLayoutId = i.Id,
                    Name = i.Name,
                    EnabledFor = i.EnabledFor,
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
        ///     Internal ID for the ItemField
        /// </summary>
        [JsonRequired]
        public required Guid ItemFieldId { get; init; }

        /// <summary>
        ///     Internal ID for the current version ItemFieldVersion
        /// </summary>
        [JsonRequired]
        public required Guid ItemFieldCurrentVersionId { get; init; }

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
        ///   The uses this layout is applicable to
        /// </summary>
        [JsonRequired]
        public required List<ItemLayoutUse> EnabledFor { get; init; }
    }
}
