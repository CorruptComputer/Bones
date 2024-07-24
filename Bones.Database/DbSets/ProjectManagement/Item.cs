namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.Items table
/// </summary>
[Table("Items", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Item
{
    /// <summary>
    ///     Internal ID for the Task
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [MaxLength(512)]
    public required string Name { get; set; }
    
    public required Queue Queue { get; set; }

    public List<Tag> Tags { get; set; } = [];
}