namespace Bones.Shared.Exceptions;

/// <summary>
///   An exception that cannot be recovered from, just stop whatever you're doing and have good logs about what happened.
/// </summary>
/// <inheritdoc />
public class UnrecoverableException(string message) : BonesExceptionBase(message);