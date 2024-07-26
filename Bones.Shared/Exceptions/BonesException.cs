namespace Bones.Shared.Exceptions;

/// <summary>
///     Exceptions from the Bones application.
/// </summary>
/// <param name="message">What went wrong.</param>
public class BonesException(string message) : ApplicationException(message);