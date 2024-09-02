using System.Text.RegularExpressions;

namespace Bones.Shared;

/// <summary>
///   Standard Regexes to use within the app
/// </summary>
public static partial class StandardRegexes
{
    /// <summary>
    ///   Check for if a password contains a number
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"[0-9]")]
    public static partial Regex PasswordContainsNumber();

    /// <summary>
    ///   Check for if the password contains a lowercase character
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"[a-z]")]
    public static partial Regex PasswordContainsLower();

    /// <summary>
    ///   Check for if the password contains an uppercase character
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"[A-Z]")]
    public static partial Regex PasswordContainsUpper();

    /// <summary>
    ///   Check for if the password contains a special character
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    public static partial Regex PasswordContainsSpecial();
}