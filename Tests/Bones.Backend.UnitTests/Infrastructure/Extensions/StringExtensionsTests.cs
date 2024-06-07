using Bones.Backend.Infrastructure.Extensions;

namespace Bones.Backend.UnitTests.Infrastructure.Extensions;

/// <summary>
///   Tests for the StringExtensions class
/// </summary>
public class StringExtensionsTests : TestBase
{
    /// <summary>
    ///   Valid emails should return true.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("unit+test@example.com")]
    [InlineData("🍕@example.com")]
    [InlineData("_test@example.com")]
    public void ValidEmail_ShouldReturnTrue(string email)
    {
        email.IsValidEmail().Should().BeTrue();
    }

    /// <summary>
    ///   Invalid emails should return false.
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
    public void InvalidEmail_ShouldFail(string email)
    {
        email.IsValidEmail().Should().BeFalse();
    }
}