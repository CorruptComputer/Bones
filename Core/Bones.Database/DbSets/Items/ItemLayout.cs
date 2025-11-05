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
    ///   The project this ItemLayout belongs to
    /// </summary>
    public required Project Project { get; set; }

    /// <summary>
    ///   The latest version of this layout
    /// </summary>
    [NotMapped]
    public ItemLayoutVersion? LatestVersion => Versions.OrderByDescending(v => v.Version).FirstOrDefault();

    /// <summary>
    ///   The versions for this Item layout
    /// </summary>
    public List<ItemLayoutVersion> Versions { get; set; } = [];

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

    internal static void BuildTable(EntityTypeBuilder<ItemLayout> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);
    }
}