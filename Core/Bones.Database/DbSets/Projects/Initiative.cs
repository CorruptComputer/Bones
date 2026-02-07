using Bones.Database.DbConsts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bones.Database.DbSets.Projects;

/// <summary>
///   Model for the Projects.Initiatives table
/// </summary>
[Table(TableNames.ProjectManagement.Initiatives, Schema = SchemaNames.Projects)]
[PrimaryKey(nameof(Id))]
public class Initiative
{
    /// <summary>
    ///   Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the project this initiative belongs to
    /// </summary>
    public required Guid ProjectId { get; init; }

    /// <summary>
    ///   The name of this initiative
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   Disables access to this Initiative and schedules deletes for everything within,
    ///   when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;

    #region Navigational Properties
    /// <summary>
    ///   Navigational property for the project that owns this initiative, null if not .Include()'d in the query
    /// </summary>
    public Project? Project { get; set; }

    /// <summary>
    ///   Navigational property for the queues that belong to this initiative, empty if not .Include()'d in the query
    /// </summary>
    public List<TaskQueue> Queues { get; set; } = [];
    #endregion

    internal static void BuildTable(EntityTypeBuilder<Initiative> builder)
    {
        // Remove deleted items from being included in default queries
        builder.HasQueryFilter(x => !x.DeleteFlag);

        builder.HasOne(i => i.Project)
            .WithMany(p => p.Initiatives)
            .HasForeignKey(i => i.ProjectId);

        builder.HasMany(i => i.Queues)
            .WithOne(q => q.Initiative)
            .HasForeignKey(q => q.InitiativeId);
    }
}