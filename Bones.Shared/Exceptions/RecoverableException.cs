namespace Bones.Shared.Exceptions;

/// <summary>
///     Some shit went sideways, but we might still be able to flush.
/// </summary>
/// <inheritdoc />
public class RecoverableException(string message) : BonesExceptionBase(message);
