using Bones.Shared.Extensions;

namespace Bones.Database.DbSets.ProjectManagement.Items;

/// <summary>
///     Model for the ProjectManagement.ItemValues table
/// </summary>
[Table("ItemValues", Schema = "ProjectManagement")]
[PrimaryKey(nameof(Id))]
public class ItemValue
{
    /// <summary>
    ///     Internal ID for the ItemHistory
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required ItemField Field { get; set; }

    public string? Value { get; private set; }

    public bool TrySetValue<T>(T value)
    {
        string? valueStr = value?.ToString();
        if (value == null || valueStr == null)
        {
            if (Field.IsRequired)
            {
                return false;
            }

            Value = null;
            return true;
        }

        switch (Field.Type)
        {
            case ItemField.FieldType.String:
                Value = valueStr;
                return true;
            case ItemField.FieldType.Number:
                if (value.IsNumericType())
                {
                    Value = valueStr;
                    return true;
                }

                return false;
            case ItemField.FieldType.Boolean:
                if (value is bool)
                {
                    Value = valueStr;
                    return true;
                }

                return false;
            case ItemField.FieldType.DateTime:
                if (value is DateTime or DateTimeOffset)
                {
                    Value = valueStr;
                    return true;
                }

                return false;
            case ItemField.FieldType.ValueList:
                if (Field.PossibleValues?.Any(pv => pv.Matches(valueStr)) ?? false)
                {
                    Value = valueStr;
                    return true;
                }

                return false;
            default:
                return false;
        }
    }
}