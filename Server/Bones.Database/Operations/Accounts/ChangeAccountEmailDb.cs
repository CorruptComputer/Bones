using Bones.Database.DbSets.Identity;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Command for updating the email address on an account.
/// </summary>
/// <param name="AccountId">The ID of the account to update</param>
/// <param name="Email">The new email address</param>
public record ChangeAccountEmailDbCommand(Guid AccountId, string Email) : IRequest<DbCommandResponse>;

internal class ChangeAccountEmailDbHandler(BonesDbContext dbContext, ISender sender)
    : IRequestHandler<ChangeAccountEmailDbCommand, DbCommandResponse>
{
    public async Task<DbCommandResponse> Handle(ChangeAccountEmailDbCommand request,
        CancellationToken cancellationToken)
    {
        DbCommandResponse emailAvailable =
            await sender.Send(new EmailAvailableForUseDbCommand(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        Account? acct =
            await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == request.AccountId, cancellationToken);

        if (acct == null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Failed to find the specified account."
            };
        }

        acct.Email = request.Email;
        acct.EmailConfirmed = false;
        acct.EmailConfirmedDateTime = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true
        };
    }
}