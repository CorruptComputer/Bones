using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///   DB Command for checking if an email is available to use.
/// </summary>
/// <param name="Email">Email address to check</param>
public record EmailAvailableForUseDbCommand(string Email) : IRequest<DbCommandResponse>;

internal class EmailAvailableForUseDbHandler(BonesDbContext dbContext) : IRequestHandler<EmailAvailableForUseDbCommand, DbCommandResponse>
{
    public async Task<DbCommandResponse> Handle(EmailAvailableForUseDbCommand request, CancellationToken cancellationToken)
    {
        Account? account = await dbContext.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email, cancellationToken);

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