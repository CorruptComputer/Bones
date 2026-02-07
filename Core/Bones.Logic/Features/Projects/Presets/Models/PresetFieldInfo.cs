using System.Diagnostics.CodeAnalysis;
using Bones.Shared.Backend.Enums;

namespace Bones.Logic.Features.Projects.Presets.Models;

internal record PresetFieldInfo
{
    internal required string Name { get; init; }
    internal required bool IsRequired { get; init; }
    internal required FieldType Type { get; init; }
    internal bool? CanBeNegative { get; init; }
    internal Dictionary<string, StringValueMatchingType>? PossibleValues { get; init; }

    [MemberNotNullWhen(true, nameof(FieldId))]
    [MemberNotNullWhen(true, nameof(FieldVersionId))]
    internal bool Created => FieldId.HasValue && FieldVersionId.HasValue;

    /// <summary>
    ///   After the field is created, this will be set to the ID of the field
    /// </summary>
    internal Guid? FieldId { get; set; }

    /// <summary>
    ///   After the field is created, this will be set to the ID of the field version
    /// </summary>
    internal Guid? FieldVersionId { get; set; }
}