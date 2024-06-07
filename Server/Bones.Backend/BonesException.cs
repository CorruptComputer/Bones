namespace Bones.Backend;

/// <summary>
///     Exceptions from the Bones application.
/// </summary>
/// <param name="reason">What went wrong.</param>
public class BonesException(string reason) : ApplicationException(reason);