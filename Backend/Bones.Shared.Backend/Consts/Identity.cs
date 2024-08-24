using Microsoft.AspNetCore.Identity;

namespace Bones.Shared.Backend.Consts;

public static class Identity
{
    public static IdentityOptions IdentityOptions => new()
    {
        User = new()
        {
            RequireUniqueEmail = true
        },
        SignIn = new()
        {
            RequireConfirmedEmail = true
        },
        Lockout = new()
        {
            MaxFailedAccessAttempts = 3,
            DefaultLockoutTimeSpan = TimeSpan.FromHours(1),
            AllowedForNewUsers = true
        },
        Password = new()
        {
            RequireDigit = true,
            RequireLowercase = true,
            RequireUppercase = true,
            RequireNonAlphanumeric = true,
            RequiredLength = 8
        }
    };
}