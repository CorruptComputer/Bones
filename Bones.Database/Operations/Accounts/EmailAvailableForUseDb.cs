using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Command for checking if an email is available to use.
/// </summary>
/// <param name="Email">Email address to check</param>
public record EmailAvailableForUseDbCommand(string Email) : IRequest<CommandResponse>;

internal class EmailAvailableForUseDbHandler(BonesDbContext dbContext)
    : IRequestHandler<EmailAvailableForUseDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(EmailAvailableForUseDbCommand request,
        CancellationToken cancellationToken)
    {
        Account? account =
            await dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email, cancellationToken);

        if (account != null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Email already in use on an existing account."
            };
        }

        return new()
        {
            Success = true
        };
    }
}