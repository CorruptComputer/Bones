using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.GenericItems.GenericItemFields;

/// <summary>
///     Model for the GenericItems.GenericItemFields table
/// </summary>
[Table("GenericItemFields", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class GenericItemField
{
    /// <summary>
    ///     Internal ID for the ItemField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    /// <summary>
    ///   The ID of the project this ItemField belongs to
    /// </summary>
    [ForeignKey(nameof(Project))]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The most recent version of this field
    /// </summary>
    [NotMapped]
    public GenericItemFieldVersion CurrentVersion => Versions.OrderByDescending(v => v.Version).First();

    /// <summary>
    ///   The versions for this Item field
    /// </summary>
    public List<GenericItemFieldVersion> Versions { get; set; } = [];

    /// <summary>
    ///   Disables creating of new layouts with this field,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}