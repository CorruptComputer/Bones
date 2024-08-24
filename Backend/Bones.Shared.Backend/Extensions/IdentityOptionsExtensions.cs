using Microsoft.AspNetCore.Identity;

namespace Bones.Shared.Backend.Extensions;

public static class IdentityOptionsExtensions
{
    public static IdentityOptions OverwriteWith(this IdentityOptions thisOptions, IdentityOptions newOptions)
    {
        // There are more, but we aren't overriding the defaults on those
        // See: Bones.Shared.Backend.Consts.Identity.IdentityOptions

        thisOptions.User = newOptions.User;
        thisOptions.SignIn = newOptions.SignIn;
        thisOptions.Lockout = newOptions.Lockout;
        thisOptions.Password = newOptions.Password;

        return thisOptions;
    }
}