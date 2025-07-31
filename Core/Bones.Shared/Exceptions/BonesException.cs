namespace Bones.Shared.Exceptions;

/// <summary>
///   Exceptions from the Bones application.
/// </summary>
/// <param name="message">What went wrong.</param>
/// <param name="likelyCause">What likely caused the exception.</param>
public class BonesException(string message, LikelyCause likelyCause = LikelyCause.CoderError) : Exception(message)
{
    /// <summary>
    ///   The likely cause of the exception.
    /// </summary>
    public LikelyCause LikelyCause { get; init; } = likelyCause;
}

/// <summary>
///   Likely cause of the exception.
/// </summary>
public enum LikelyCause
{
    /// <summary>
    ///   The user did something wrong, like providing invalid input.
    /// </summary>
    UserError,

    /// <summary>
    ///   The developer did something wrong, like not handling a case properly.
    /// </summary>
    CoderError
}