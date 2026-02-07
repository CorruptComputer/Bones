namespace Bones.Shared.Backend.Enums;

/// <summary>
///   The type of field
/// </summary>
public enum FieldType
{
    /// <summary>
    ///   Short text, should just be a <see cref="string" />
    /// </summary>
    TextField = 0,

    /// <summary>
    ///   Long text, should just be a <see cref="string" />
    /// </summary>
    TextBox = 1,

    /// <summary>
    ///   Should just be a <see cref="int" />
    /// </summary>
    Integer = 2,

    /// <summary>
    ///   Should just be a <see cref="double" />
    /// </summary>
    Decimal = 3,

    /// <summary>
    ///   Should just be a <see cref="bool" />
    /// </summary>
    Boolean = 4,

    /// <summary>
    ///   Should just be a <see cref="DateTimeOffset"/>
    /// </summary>
    DateTime = 5,

    /// <summary>
    ///   The value will just be a <see cref="string" />, however it will be validated against a list of allowed values
    /// </summary>
    ValueList = 6,
}