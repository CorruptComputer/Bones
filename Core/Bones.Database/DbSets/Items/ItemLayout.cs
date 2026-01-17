using Bones.Database.DbConsts;
using Bones.Database.DbSets.ProjectManagement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items;

/// <summary>
///   Model for the Item.ItemLayouts table
/// </summary>
[Table(TableNames.Item.ItemLayouts, Schema = SchemaNames.Item)]
[PrimaryKey(nameof(Id))]
[Index(nameof(FriendlyIdPrefix))]
public class ItemLayout
{
    /// <summary>
    ///   Internal ID for the ItemLayout
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The date and time this layout was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the project this ItemLayout belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The prefix at the start of a Friendly ID for items using this layout,
    ///   e.g. "BUG" for bugs or "FEAT" for feature additions. Up to 6 letters.
    ///
    ///   Cannot be changed after creation.
    /// </summary>
    [MaxLength(6)]
    public required string FriendlyIdPrefix { get; init; }

    /// <summary>
    ///   The next available friendly ID number for items using this layout.
    ///   Most of the time this will be the highest friendly ID number in use + 1.
    /// </summary>
    public long FriendlyIdNonce { get; set; } = 1;

    /// <summary>
    ///   Disables creating of new items using this layout,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational Property for the project this ItemLayout belongs to, null if not .Include()'d in the query
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    ///   Navigational Property for the versions of this ItemLayout, empty if not .Include()'d in the query
    /// </summary>
    public List<ItemLayoutVersion> Versions { get; set; } = [];
    #endregion

    #region Non-Mapped Properties
    /// <summary>
    ///   The latest version of this layout
    /// </summary>
    [NotMapped]
    public ItemLayoutVersion? Current => Versions.OrderByDescending(v => v.Version).FirstOrDefault();

    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemLayout> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(il => il.Project)
               .WithMany()
               .HasForeignKey(il => il.ProjectId);

        builder.HasMany(il => il.Versions)
               .WithOne(ilv => ilv.ItemLayout)
               .HasForeignKey(ilv => ilv.ItemLayoutId);
    }
}