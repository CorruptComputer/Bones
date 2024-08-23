namespace Bones.Shared.Exceptions;

public class AuthenticationFailedException(bool isForbidden, string message) : BonesException(message)
{
    public bool IsForbidden { get; set; } = isForbidden;
}