using System.Net;
using Bones.Database.Converters;

namespace Bones.Database.UnitTests.Converters;

/// <summary>
///   Tests for the IpAddressToStringConverter
/// </summary>
public class IpAddressToStringConverterTests : TestBase
{
    private const string _testIpStr = "127.0.0.1";
    private readonly IPAddress _testIp = IPAddress.Parse(_testIpStr);

    /// <summary>
    ///   Checks that the converter converts from IpAddress to string correctly
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithValidIpAddress_ShouldConvertToValidIpString()
    {
        IpAddressToStringConverter converter = new();

        string actual = converter.ConvertToProviderTyped(_testIp);
        actual.ShouldBe(_testIpStr);
    }

    /// <summary>
    ///   Checks that the converter converts from string to IpAddress correctly
    /// </summary>
    [Fact]
    public void ConvertFromProviderTyped_WithValidIpString_ShouldConvertToValidIpAddress()
    {
        IpAddressToStringConverter converter = new();

        IPAddress actual = converter.ConvertFromProviderTyped(_testIpStr);
        actual.ShouldBe(_testIp);
    }

    /// <summary>
    ///   Checks that the converter handles null IpAddress correctly
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithNullIpAddress_ShouldConvertToEmptyIpString()
    {
        IpAddressToStringConverter converter = new();

        string actual = converter.ConvertToProviderTyped(null!);
        actual.ShouldBe(string.Empty);
    }

    /// <summary>
    ///   Checks that the converter handles None IpAddress correctly
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithNoneIpAddress_ShouldConvertToEmptyIpString()
    {
        IpAddressToStringConverter converter = new();

        string actual = converter.ConvertToProviderTyped(IPAddress.None);
        actual.ShouldBe(string.Empty);
    }

    /// <summary>
    ///   Checks that the converter handles null string correctly
    /// </summary>
    [Fact]
    public void ConvertFromProviderTyped_WithNullIpString_ShouldConvertToNoneIpAddress()
    {
        IpAddressToStringConverter converter = new();

        IPAddress actual = converter.ConvertFromProviderTyped(null!);
        actual.ShouldBe(IPAddress.None);
    }

    /// <summary>
    ///   Checks that the converter handles empty string correctly
    /// </summary>
    [Fact]
    public void ConvertFromProviderTyped_WithEmptyIpString_ShouldConvertToNoneIpAddress()
    {
        IpAddressToStringConverter converter = new();

        IPAddress actual = converter.ConvertFromProviderTyped(string.Empty);
        actual.ShouldBe(IPAddress.None);
    }
}