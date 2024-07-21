using MediatR;

namespace Bones.Testing.Shared;

/// <summary>
///     Base for Unit Test classes.
/// </summary>
public class TestBase
{
    /// <summary>
    ///     MediatR sender for commands and queries. Unique DB per-test.
    /// </summary>
    protected ISender Sender { get; } = TestFactory.GetTestSender();
}