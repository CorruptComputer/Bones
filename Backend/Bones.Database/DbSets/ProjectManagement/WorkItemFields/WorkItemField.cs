using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.ProjectManagement.WorkItemFields;

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

    /// <summary>
    ///   The name of the work item field
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///   If this field is a ValueList type, these are the possible values it can have
    /// </summary>
    public List<WorkItemFieldListEntry>? PossibleValues { get; set; }

    /// <summary>
    ///   The type for this field
    /// </summary>
    public required FieldType Type { get; set; }
}