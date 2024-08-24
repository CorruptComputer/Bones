using System.Text.RegularExpressions;

namespace Bones.Shared;

public static partial class StandardRegexes
{
    [GeneratedRegex(@"[0-9]")]
    public static partial Regex PasswordContainsNumber();

    [GeneratedRegex(@"[a-z]")]
    public static partial Regex PasswordContainsLower();

    [GeneratedRegex(@"[A-Z]")]
    public static partial Regex PasswordContainsUpper();

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    public static partial Regex PasswordContainsSpecial();
}