using Bones.Database.DbConsts;
using Bones.Database.DbSets.Projects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Items.Fields;

/// <summary>
///   Model for the Items.Fields.ItemFields table
/// </summary>
[Table(TableNames.Item.Fields.ItemFields, Schema = SchemaNames.Items_Fields)]
[PrimaryKey(nameof(Id))]
public class ItemField
{
    /// <summary>
    ///   Internal ID for the ItemField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The date and time this field was created
    /// </summary>
    public DateTimeOffset CreateDateTime { get; init; } = DateTimeOffset.Now;

    /// <summary>
    ///   The ID of the project this ItemField belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   Disables creating of new layouts with this field,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational Property for the project this ItemField belongs to, null if not .Include()'d in the query
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    ///   Navigational Property for the versions of this ItemField, empty if not .Include()'d in the query
    /// </summary>
    public List<ItemFieldVersion> Versions { get; set; } = [];
    #endregion

    #region Non-Mapped Properties
    /// <summary>
    ///   The latest version of this field
    /// </summary>
    [NotMapped]
    public ItemFieldVersion? Current => Versions.OrderByDescending(v => v.Version).FirstOrDefault();
    #endregion

    internal static void BuildTable(EntityTypeBuilder<ItemField> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(itf => !itf.DeleteFlag);

        builder.HasOne(itf => itf.Project)
            .WithMany()
            .HasForeignKey(itf => itf.ProjectId);

        builder.HasMany(itf => itf.Versions)
            .WithOne(ifv => ifv.ItemField)
            .HasForeignKey(ifv => ifv.ItemFieldId);
    }
}