using System.Text.RegularExpressions;

namespace Bones.Shared;

/// <summary>
///   Standard Regexes to use within the app
/// </summary>
public static partial class StandardRegexes
{
    /// <summary>
    ///   Check if a string is a valid password, must contain at least one uppercase, one lowercase, one number, one special character, and be at least 8 characters long
    /// </summary>
    public const string VALID_PASSWORD = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{8,}$";

    /// <summary>
    ///   Check if a string contains a number
    /// </summary>
    public const string CONTAINS_NUMBER = "[0-9]";

    /// <summary>
    ///   Check if a string contains a number, generated at compile time
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(CONTAINS_NUMBER)]
    public static partial Regex ContainsNumber();

    /// <summary>
    ///   Check if a string contains a lowercase character
    /// </summary>
    public const string CONTAINS_LOWER = "[a-z]";

    /// <summary>
    ///   Check if a string contains a lowercase character, generated at compile time
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(CONTAINS_LOWER)]
    public static partial Regex ContainsLower();

    /// <summary>
    ///   Check if a string contains an uppercase character
    /// </summary>
    public const string CONTAINS_UPPER = "[A-Z]";

    /// <summary>
    ///   Check if a string contains an uppercase character, generated at compile time
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(CONTAINS_UPPER)]
    public static partial Regex ContainsUpper();

    /// <summary>
    ///   Check if a string contains a special character (non-alphanumeric)
    /// </summary>
    public const string CONTAINS_SPECIAL = "[^a-zA-Z0-9]";

    /// <summary>
    ///   Check if a string contains a special character (non-alphanumeric), generated at compile time
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(CONTAINS_SPECIAL)]
    public static partial Regex ContainsSpecial();
}