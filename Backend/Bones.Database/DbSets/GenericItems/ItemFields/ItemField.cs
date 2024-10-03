using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.GenericItems.ItemFields;

/// <summary>
///     Model for the GenericItems.ItemFields table
/// </summary>
[Table("ItemFields", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class ItemField
{
    /// <summary>
    ///     Internal ID for the ItemField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The name of this ItemField
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The ID of the project this ItemField belongs to
    /// </summary>
    [ForeignKey(nameof(Project))]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///   If the Type of this field is a ValueList, the possible values this can have
    /// </summary>
    public List<ItemFieldListEntry>? PossibleValues { get; set; }

    /// <summary>
    ///   The FieldType for this field
    /// </summary>
    public required FieldType Type { get; set; }

}