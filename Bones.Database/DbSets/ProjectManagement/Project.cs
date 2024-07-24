namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.Projects table
/// </summary>
[Table("Projects", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class Project
{
    /// <summary>
    ///     Internal ID for the Tag
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [MaxLength(512)]
    public required string Name { get; set; }
}