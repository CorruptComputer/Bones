using System.Net.Mail;

namespace Bones.Backend.Extensions;

internal static class StringExtensions
{
    internal static bool IsValidEmail(this string str)
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