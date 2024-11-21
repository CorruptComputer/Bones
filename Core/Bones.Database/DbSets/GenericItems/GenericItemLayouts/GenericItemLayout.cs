using Bones.Database.DbSets.ProjectManagement;
using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.GenericItems.GenericItemLayouts;

/// <summary>
///     Model for the GenericItems.GenericItemLayouts table
/// </summary>
[Table("GenericItemLayouts", Schema = "GenericItems")]
[PrimaryKey(nameof(Id))]
public class GenericItemLayout
{
    /// <summary>
    ///     Internal ID for the ItemLayout
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    /// <summary>
    ///   The name for this Item layout
    /// </summary>
    [MaxLength(512)]
    public required string Name { get; set; }

    /// <summary>
    ///   The ID of the project this ItemLayout belongs to
    /// </summary>
    [ForeignKey(nameof(Project))]
    public required Guid ProjectId { get; set; }

    /// <summary>
    ///   The versions for this Item layout
    /// </summary>
    public List<GenericItemLayoutVersion> Versions { get; set; } = [];

    /// <summary>
    ///   The uses this layout is applicable to
    /// </summary>
    public required List<ItemLayoutUse> EnabledFor { get; set; }

    /// <summary>
    ///   Disables creating of new items using this layout,
    ///   and when all items using it are deleted it will be removed.
    /// </summary>
    public bool DeleteFlag { get; set; } = false;
}