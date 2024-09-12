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
    /// <param name="cancellationToken"></param>
    /// <returns>true if valid, false otherwise.</returns>
    public static async Task<bool> IsValidEmailAsync(this string str, CancellationToken cancellationToken = default)
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
            IDnsQueryResponse? result = await lookup.QueryAsync(addr.Host, QueryType.MX, cancellationToken: cancellationToken);
            MxRecord? record = result.Answers.MxRecords().FirstOrDefault();

            return record != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///   Gets a URL safe and base 64 encoded translation of the provided string.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Base64UrlSafeEncode(this string text)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    /// <summary>
    ///   Gets a URL safe and base 64 decoded translation of the provided string.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
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

    private static readonly List<char> SoundexCharsToRemove = ['Y', 'H', 'W'];

    private static readonly Dictionary<char, int> SoundexMappings = new()
    {
        { 'B', 1 }, { 'F', 1 }, { 'P', 1 }, { 'V', 1 },
        { 'C', 2 }, { 'G', 2 }, { 'J', 2 }, { 'K', 2 }, { 'Q', 2 }, { 'S', 2 }, { 'X', 2 }, { 'Z', 2 },
        { 'D', 3 }, { 'T', 3 },
        { 'L', 4 },
        { 'M', 5 }, { 'N', 5 },
        { 'R', 6 }
    };

    /// <summary>
    ///   Do these strings match using soundex?
    /// </summary>
    /// <param name="str"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool SoundexMatch(this string str, string other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other), "Other string cannot be null!");
        }

        string strSoundex = str.ToSoundexString();
        string otherSoundex = other.ToSoundexString();

        return strSoundex.Equals(otherSoundex);
    }

    /// <summary>
    ///   Translates the provided string into its soundex representation.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    /// <remarks>
    ///   For reference: https://en.wikipedia.org/wiki/Soundex
    /// </remarks>
    public static string ToSoundexString(this string str)
    {
        string strUpper = str.ToUpper();
        char firstLetter = strUpper[0];
        StringBuilder soundex = new($"{firstLetter}");

        foreach (char c in SoundexCharsToRemove.Where(c => strUpper.Contains(c)))
        {
            strUpper = strUpper.Replace(c.ToString(), string.Empty);
        }

        int? prevCode = null;
        if (SoundexMappings.TryGetValue(firstLetter, out int mapping))
        {
            prevCode = mapping;
        }

        bool previousLetterVowel = IsVowel(firstLetter);
        foreach (char c in strUpper[1..])
        {
            if (SoundexMappings.TryGetValue(c, out int curCode))
            {
                if (!previousLetterVowel && curCode == prevCode)
                {
                    continue;
                }

                soundex.Append(curCode);
                prevCode = curCode;
            }

            previousLetterVowel = IsVowel(c);
        }

        return soundex.ToString().PadRight(4, '0')[..4];
    }

    private static bool IsVowel(char c)
    {
        const string vowels = "AEIOU";
        return vowels.Contains(char.ToUpper(c));
    }

    /// <summary>
    ///   Converts the string into a stream
    /// </summary>
    /// <param name="str"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<Stream> ToStreamAsync(this string str, CancellationToken cancellationToken = default)
    {
        MemoryStream stream = new();
        StreamWriter writer = new(stream);
        await writer.WriteAsync(str);
        await writer.FlushAsync(cancellationToken);
        stream.Position = 0;
        return stream;
    }
}