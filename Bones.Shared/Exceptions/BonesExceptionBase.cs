namespace Bones.Shared.Exceptions;

/// <summary>
///     Exceptions from the Bones application.
/// </summary>
/// <param name="reason">What went wrong.</param>
public abstract class BonesExceptionBase(string reason) : ApplicationException(reason);