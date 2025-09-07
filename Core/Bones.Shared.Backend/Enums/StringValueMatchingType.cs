namespace Bones.Shared.Backend.Enums;

/// <summary>
///   Type of string matching to use for this
/// </summary>
public enum StringValueMatchingType
{
    /// <summary>
    ///   Exact match, case matters
    /// </summary>
    Exact,

    /// <summary>
    ///   Case-insensitive
    /// </summary>
    CaseInvariant,

    /// <summary>
    ///   Case-insensitive soundex matching
    /// </summary>
    Soundex
}