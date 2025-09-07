namespace Bones.Shared.Exceptions;

/// <summary>
///   The user is not authenticated, they need to log in or the token is invalid.
/// </summary>
public class UnauthenticatedException : Exception;