using System.Net.Mail;
using System.Text;
using DnsClient;
using DnsClient.Protocol;

namespace Bones.Shared.Extensions;

/// <summary>
///   Extension methods for strings
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///   Checks if the string is a valid email, including checking for an MX record for the domain.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>true if valid, false otherwise.</returns>
    public static async Task<bool> IsValidEmailAsync(this string str)
    {
        str = str.Trim();

        try
        {
            MailAddress addr = new(str);

            // If the parsed address doesn't match the input then something in it wasn't valid
            if (addr.Address != str)
            {
                return false;
            }

            // Need to have a TLD
            if (!addr.Host.Contains('.') || addr.Host.EndsWith('.'))
            {
                return false;
            }

            // Check that the host has an MX record
            LookupClient lookup = new();
            IDnsQueryResponse? result = await lookup.QueryAsync(addr.Host, QueryType.MX);
            MxRecord? record = result.Answers.MxRecords().FirstOrDefault();

            return record != null;
        }
        catch
        {
            return false;
        }
    }

    public static string Base64UrlSafeEncode(this string text)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }


    public static string Base64UrlSafeDecode(this string text)
    {
        text = text.Replace('_', '/').Replace('-', '+');
        switch (text.Length % 4)
        {
            case 2:
                text += "==";
                break;
            case 3:
                text += "=";
                break;
        }
        return Encoding.UTF8.GetString(Convert.FromBase64String(text));
    }
}