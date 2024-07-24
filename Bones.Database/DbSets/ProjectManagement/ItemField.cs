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

    public required string FieldTitle { get; set; }

    public bool IsRequired { get; set; } = false;

    public List<ItemFieldListEntry> PossibleValues { get; set; } = [];
    
    public required FieldType Type { get; set; }

    public enum FieldType
    {
        String = 0,
        Number = 1,
        Boolean = 2,
        DateTime = 3,
        ValueList = 4
    }
}