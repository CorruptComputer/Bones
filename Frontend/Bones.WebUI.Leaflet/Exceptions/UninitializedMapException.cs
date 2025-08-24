using System.Runtime.CompilerServices;
using Bones.Shared.Exceptions;

namespace Bones.WebUI.Leaflet.Exceptions;

/// <summary>
///   Exception thrown when the user tried to manipulate the map before it has been initialized.
/// </summary>
public class UninitializedMapException : BonesException
{
    /// <summary>
    ///   ctor for UninitializedMapException
    /// </summary>
    /// <param name="message"></param>
    /// <param name="offendingMethod"></param>
    public UninitializedMapException(string message = "", [CallerMemberName] string offendingMethod = "(unknown)") :
        base(message)
    {
        Console.WriteLine($"{offendingMethod}: {message}");
    }
}