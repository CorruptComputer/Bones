using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.GenericItems.GenericItemFields;

/// <summary>
///     Model for the GenericItems.GenericItemFields table
/// </summary>
[Table("GenericItemFieldVersions", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class GenericItemFieldVersion
{
    /// <summary>
    ///     Internal ID for the ItemFieldVersion
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the layout this version belongs to
    /// </summary>
    [ForeignKey(nameof(GenericItemField))]
    public required Guid GenericItemFieldId { get; init; }

    /// <summary>
    ///   The version number for this
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required long Version { get; init; }

    /// <summary>
    ///   The name of this ItemFieldVersion
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   Is this field required to have a value?
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    ///   If the Type of this field is a ValueList, the possible values this can have
    /// </summary>
    public List<GenericItemFieldListEntry>? PossibleValues { get; set; }

    /// <summary>
    ///   The FieldType for this field
    /// </summary>
    public required FieldType Type { get; set; }

    /// <summary>
    ///   Disables creating of new layouts with this field,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}