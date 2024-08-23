using Bones.Shared.Backend.Enums;

namespace Bones.Database.DbSets.ProjectManagement;

/// <summary>
///     Model for the ProjectManagement.ItemFields table
/// </summary>
[Table("ItemFields", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemField
{
    /// <summary>
    ///     Internal ID for the ItemField
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [MaxLength(512)]
    public required string Name { get; set; }

    public bool IsRequired { get; set; } = false;

    public List<ItemFieldListEntry> PossibleValues { get; set; } = [];

    public required FieldType Type { get; set; }
}