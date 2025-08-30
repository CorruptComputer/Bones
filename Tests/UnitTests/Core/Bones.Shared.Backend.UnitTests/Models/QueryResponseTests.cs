using Bones.Shared.Backend.Models;
using Bones.Testing.UnitTests.Shared;

namespace Bones.Shared.Backend.UnitTests.Models;

/// <summary>
///   Tests for the QueryResponse class
/// </summary>
public class QueryResponseTests : TestBase
{
    /// <summary>
    ///   Pass should return a successful result with the value
    /// </summary>
    [Fact]
    public void Pass_ShouldReturnSuccessfulResultWithTheValue()
    {
        QueryResponse<bool> response = QueryResponse<bool>.Pass(true);
        response.ShouldNotBeNull();
        response.Success.ShouldBeTrue();
        response.Result.ShouldBeTrue();
    }


}
