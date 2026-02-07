using Bones.Database.DbSets.Items.Fields;
using Bones.Shared.Backend.Enums;

namespace Bones.Api.Models.Project;

/// <summary>
///   API response for the GetProjectItemFieldsAsync endpoint
/// </summary>
public class GetProjectItemFieldsResponse
{
    /// <summary>
    ///   List of item fields associated with the project
    /// </summary>
    public required List<ProjectItemFieldModel> ItemFields { get; init; }

    internal static GetProjectItemFieldsResponse FromInternalList(List<ItemField> fields)
    {
        return new()
        {
            ItemFields = [.. fields.Select(ProjectItemFieldModel.FromInternal)]
        };
    }

    /// <summary>
    ///   Response model for item fields
    /// </summary>
    public class ProjectItemFieldModel
    {
        /// <summary>
        ///   ID of the item field
        /// </summary>
        [JsonRequired]
        public required Guid FieldId { get; init; }

        /// <summary>
        ///   ID of the item field version
        /// </summary>
        [JsonRequired]
        public required Guid FieldVersionId { get; init; }

        /// <summary>
        ///   Version number of the item field
        /// </summary>
        [JsonRequired]
        public required long Version { get; init; }

        /// <summary>
        ///   Name of the item field
        /// </summary>
        [JsonRequired]
        public required string Name { get; init; }

        /// <summary>
        ///   Is the field required?
        /// </summary>
        [JsonRequired]
        public required bool IsRequired { get; init; }

        /// <summary>
        ///   Type of the item field
        /// </summary>
        [JsonRequired]
        public required FieldType Type { get; init; }

        /// <summary>
        ///   If the field is numeric, can it be negative?
        /// </summary>
        public bool? CanBeNegative { get; init; }

        /// <summary>
        ///   If the field is a ValueList, the possible values
        /// </summary>
        public IEnumerable<KeyValuePair<string, StringValueMatchingType>>? PossibleValues { get; init; }

        internal static ProjectItemFieldModel FromInternal(ItemField field)
        {
            if (field.Current is null)
            {
                throw new InvalidOperationException("Field has no latest version");
            }

            return new()
            {
                FieldId = field.Id,
                FieldVersionId = field.Current.Id,
                Version = field.Current.Version,
                Name = field.Current.Name,
                IsRequired = field.Current.IsRequired,
                Type = field.Current.Type,
                CanBeNegative = field.Current.CanBeNegative,
                PossibleValues = field.Current.PossibleValues?.Select(x => new KeyValuePair<string, StringValueMatchingType>(x.Value, x.MatchingType))
            };
        }
    }
}
