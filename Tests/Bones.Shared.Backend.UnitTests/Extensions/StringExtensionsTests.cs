using Bones.Shared.Extensions;

namespace Bones.Shared.Backend.UnitTests.Extensions;

/// <summary>
///     Tests for the StringExtensions class
/// </summary>
public class StringExtensionsTests : TestBase
{
    /// <summary>
    ///     Valid emails should return true.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("unit+test@example.com")]
    [InlineData("🍕@example.com")]
    [InlineData("_test@example.com")]
    public async Task ValidEmail_ShouldReturnTrue(string email)
    {
        (await email.IsValidEmailAsync()).Should().BeTrue();
    }

    /// <summary>
    ///     Invalid emails should return false.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(" @ ")]
    [InlineData("test@example,com")]
    [InlineData("test@example.")]
    [InlineData("test@example")]
    [InlineData("test@")]
    [InlineData("test")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        (await email.IsValidEmailAsync()).Should().BeFalse();
    }
}