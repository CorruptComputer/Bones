using Bones.Database.DbSets.ProjectManagement.Queues;

namespace Bones.Database.DbSets.ProjectManagement.Items;

/// <summary>
///     Model for the ProjectManagement.Items table
/// </summary>
[Table("Items", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Item
{
    /// <summary>
    ///     Internal ID for the Item
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(512)]
    public required string Name { get; set; }

    public required Queue Queue { get; set; }

    public required DateTimeOffset AddedToQueueDateTime { get; set; }

    public List<Tag> Tags { get; set; } = [];

    public List<ItemVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables viewing this item, and when safe to do so it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}