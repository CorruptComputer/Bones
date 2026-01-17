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
    ///   ID of the project this Asset belongs to
    /// </summary>
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The ID of the item this Asset represents
    /// </summary>
    public required Guid ItemId { get; set; }

    /// <summary>
    ///   Disables access to this Asset and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the project this Asset belongs to, null if not .Include()'d in the query
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    ///   Navigational property for the item this Asset represents, null if not .Include()'d in the query
    /// </summary>
    public Item? Item { get; set; }
    #endregion

    internal static void BuildTable(EntityTypeBuilder<Asset> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(x => x.Project)
               .WithMany()
               .HasForeignKey(x => x.ProjectId);

        builder.HasOne(x => x.Item)
               .WithMany()
               .HasForeignKey(x => x.ItemId);
    }
}