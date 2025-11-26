using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Bones.Shared.Exceptions;

namespace Bones.Testing.Shared.Exceptions;

/// <summary>
///   The "test blew up" exception.
/// </summary>
/// <param name="message">What went wrong.</param>
public class BonesTestException(string message) : BonesException(message)
{
    /// <summary>Throws an <see cref="ArgumentNullException"/> if <paramref name="argument"/> is null.</summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument"/> corresponds.</param>
    public static new void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        if (argument is null)
        {
            throw new BonesTestException($"Variable '{paramName}' cannot be null.");
        }
    }
}