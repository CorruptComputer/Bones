using Bones.Database.Converters;

namespace Bones.Database.UnitTests.Converters;

/// <summary>
///   Tests for the DateTimeOffsetUtcConverter
/// </summary>
public class DateTimeOffsetUtcConverterTests : TestBase
{
    // UTC Time
    private readonly DateTimeOffset _utcStandardTime = new(2025, 11, 24, 23, 15, 30, TimeSpan.Zero);
    private readonly DateTimeOffset _utcDaylightTime = new(2025, 5, 24, 23, 15, 30, TimeSpan.Zero);

    // Central Time
    private readonly DateTimeOffset _cstTime = new(2025, 11, 24, 17, 15, 30, TimeSpan.FromHours(-6));
    private readonly DateTimeOffset _cdtTime = new(2025, 5, 24, 18, 15, 30, TimeSpan.FromHours(-5));

    // Japan Time
    private readonly DateTimeOffset _jstTime = new(2025, 11, 25, 8, 15, 30, TimeSpan.FromHours(9));
    // No Daylight time in Japan

    // Central European Time
    private readonly DateTimeOffset _cetTime = new(2025, 11, 25, 0, 15, 30, TimeSpan.FromHours(1));
    private readonly DateTimeOffset _cestTime = new(2025, 5, 25, 1, 15, 30, TimeSpan.FromHours(2));

    /// <summary>
    ///   Checks that the converter converts to UTC correctly when input is already UTC
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithUtcDateTimeOffset_ShouldConvertToUtc()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertToProviderTyped(_utcStandardTime);
        actual.ShouldBe(_utcStandardTime);
    }

    /// <summary>
    ///   Checks that the converter converts Central Standard Time to UTC
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithCstDateTimeOffset_ShouldConvertToUtc()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertToProviderTyped(_cstTime);
        actual.ShouldBe(_utcStandardTime);
        actual.Offset.ShouldBe(TimeSpan.Zero);
    }

    /// <summary>
    ///   Checks that the converter converts Central Daylight Time to UTC
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithCdtDateTimeOffset_ShouldConvertToUtc()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertToProviderTyped(_cdtTime);

        actual.ShouldBe(_utcDaylightTime);
        actual.Offset.ShouldBe(TimeSpan.Zero);
    }

    /// <summary>
    ///   Checks that the converter converts Japan Standard Time to UTC
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithJstDateTimeOffset_ShouldConvertToUtc()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertToProviderTyped(_jstTime);
        actual.ShouldBe(_utcStandardTime);
        actual.Offset.ShouldBe(TimeSpan.Zero);
    }

    /// <summary>
    ///   Checks that the converter converts Central European Time to UTC
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithCetDateTimeOffset_ShouldConvertToUtc()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertToProviderTyped(_cetTime);
        actual.ShouldBe(_utcStandardTime);
        actual.Offset.ShouldBe(TimeSpan.Zero);
    }

    /// <summary>
    ///   Checks that the converter converts Central European Summer Time to UTC
    /// </summary>
    [Fact]
    public void ConvertToProviderTyped_WithCestDateTimeOffset_ShouldConvertToUtc()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertToProviderTyped(_cestTime);
        actual.ShouldBe(_utcDaylightTime);
        actual.Offset.ShouldBe(TimeSpan.Zero);
    }

    /// <summary>
    ///   Checks that the converter returns the DateTimeOffset unchanged on reverse conversion
    /// </summary>
    [Fact]
    public void ConvertFromProviderTyped_WithAnyDateTimeOffset_ShouldReturnUnchanged()
    {
        DateTimeOffsetUtcConverter converter = new();

        DateTimeOffset actual = converter.ConvertFromProviderTyped(_utcStandardTime);
        actual.ShouldBe(_utcStandardTime);
    }
}