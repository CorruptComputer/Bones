using System.Net.Mail;

namespace Bones.Shared.Extensions;

/// <summary>
///   Extension methods for strings
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///   Checks if the string is a valid email.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>true if valid, false otherwise.</returns>
    public static bool IsValidEmail(this string str)
    {
        str = str.Trim();

        try
        {
            MailAddress addr = new(str);

            // Need to have a TLD
            if (!addr.Host.Contains('.') || addr.Host.EndsWith('.'))
            {
                return false;
            }

            return addr.Address == str;
        }
        catch
        {
            return false;
        }
    }
}