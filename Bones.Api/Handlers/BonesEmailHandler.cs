using Bones.Database.DbSets.AccountManagement;
using Microsoft.AspNetCore.Identity;

namespace Bones.Api.Handlers;

public class BonesEmailHandler : IEmailSender<BonesUser>
{
    public Task SendConfirmationLinkAsync(BonesUser user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(BonesUser user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetCodeAsync(BonesUser user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }
}