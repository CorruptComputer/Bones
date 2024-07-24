namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.ItemLayouts table
/// </summary>
[Table("ItemLayouts", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemLayout
{
    /// <summary>
    ///     Internal ID for the ItemLayout
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [MaxLength(512)]
    public required string Name { get; set; }

    public List<ItemField> ItemFields { get; set; } = [];
}