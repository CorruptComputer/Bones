namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.ItemHistories table
/// </summary>
[Table("ItemHistories", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemHistory
{
    /// <summary>
    ///     Internal ID for the ItemHistory
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    
}