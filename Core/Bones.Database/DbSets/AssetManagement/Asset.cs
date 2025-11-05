using System.Runtime.Serialization;
using Bones.Database.DbConsts;
using Bones.Database.DbSets.Items;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.AssetManagement;

/// <summary>
///   Model for the AssetManagement.Assets table
/// </summary>
[Table(TableNames.AssetManagement.Assets, Schema = SchemaNames.AssetManagement)]
[PrimaryKey(nameof(Id))]
public class Asset
{
    /// <summary>
    ///   Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The project this Asset belongs to
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The  item for this asset
    /// </summary>
    public required Item Item { get; set; }

    /// <summary>
    ///   The current version of the  item this work item is using
    /// </summary>
    [IgnoreDataMember]
    public ItemVersion? CurrentVersion
        => Item.Versions.FirstOrDefault(x => x.Version == Item.CurrentVersion);

    /// <summary>
    ///   Disables access to this Asset and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    internal static void BuildTable(EntityTypeBuilder<Asset> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}