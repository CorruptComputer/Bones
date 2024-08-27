using Microsoft.AspNetCore.Identity;

namespace Bones.Shared.Backend.Extensions;

public static class IdentityOptionsExtensions
{
    public static IdentityOptions AddBonesIdentityOptions(this IdentityOptions options)
    {
        options.User.RequireUniqueEmail = true;

        options.SignIn.RequireConfirmedEmail = true;

        options.Lockout.MaxFailedAccessAttempts = 3;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
        options.Lockout.AllowedForNewUsers = true;

        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;

        return options;
    }
}