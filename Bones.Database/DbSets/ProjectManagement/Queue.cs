namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.Queues table
/// </summary>
[Table("Queues", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Queue
{
    /// <summary>
    ///     Internal ID for the Slot
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public required Initiative Initiative { get; set; }
    
    [MaxLength(512)]
    public required string Name { get; set; }
}