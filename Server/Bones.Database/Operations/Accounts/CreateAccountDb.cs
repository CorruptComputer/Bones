using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///   DB Command for creating an account.
/// </summary>
/// <param name="Email">Email address to use for the account.</param>
public record CreateAccountDbCommand(string Email) : IRequest<DbCommandResponse>;

internal class CreateAccountDbHandler(BonesDbContext dbContext, ISender sender) : IRequestHandler<CreateAccountDbCommand, DbCommandResponse>
{
    public async Task<DbCommandResponse> Handle(CreateAccountDbCommand request, CancellationToken cancellationToken)
    {
        DbCommandResponse emailAvailable = await sender.Send(new EmailAvailableForUseDbCommand(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        EntityEntry<Account> created = await dbContext.Accounts.AddAsync(new()
        {
            CreateDateTime = DateTime.UtcNow,
            Email = request.Email,
            EmailVerified = false
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.AccountId
        };
    }
}