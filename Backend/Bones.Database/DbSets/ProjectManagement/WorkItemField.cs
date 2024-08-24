using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.WorkItemFields table
/// </summary>
[Table("WorkItemFields", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class WorkItemField
{
    /// <summary>
    ///     Internal ID for the IItemField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(512)]
    public required string Name { get; set; }

    public bool IsRequired { get; set; } = false;

    public List<WorkItemFieldListEntry> PossibleValues { get; set; } = [];

    public required FieldType Type { get; set; }
}