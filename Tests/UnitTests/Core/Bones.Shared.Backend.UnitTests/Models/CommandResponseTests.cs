using Bones.Shared.Backend.Models;
using Bones.Testing.UnitTests.Shared;

namespace Bones.Shared.Backend.UnitTests.Models;

/// <summary>
///   Tests for the <see cref="CommandResponse"/> class
/// </summary>
public class CommandResponseTests : TestBase
{
    /// <summary>
    ///   Pass should return a successful result
    /// </summary>
    [Fact]
    public void Pass_ShouldReturnSuccessfulResult()
    {
        CommandResponse response = CommandResponse.Pass();
        response.ShouldNotBeNull();
        response.Success.ShouldBeTrue();
    }

    /// <summary>
    ///   Pass with ID should return a successful result with the ID
    /// </summary>
    [Fact]
    public void PassWithId_ShouldReturnSuccessfulResultWithTheId()
    {
        Guid id = Guid.NewGuid();
        CommandResponse response = CommandResponse.Pass("test", id);

        response.ShouldNotBeNull();
        response.Success.ShouldBeTrue();
        response.Ids.ShouldNotBeNull();
        response.Ids.Count.ShouldBe(1);
        response.Ids["test"].ShouldBe(id);
    }
}
