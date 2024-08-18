namespace Bones.WebUI.Exceptions;

/// <summary>
///     Exceptions from the Bones application.
/// </summary>
/// <param name="message">What went wrong.</param>
public class BonesUiException(string message) : ApplicationException(message);