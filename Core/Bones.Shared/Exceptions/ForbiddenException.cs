namespace Bones.Shared.Exceptions;

/// <summary>
///     The user was authenticated, but they are not authorized to perform the action.
/// </summary>
public class ForbiddenException : Exception;