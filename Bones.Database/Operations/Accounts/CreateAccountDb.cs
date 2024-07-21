using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Command for creating an account.
/// </summary>
/// <param name="Email">Email address to use for the account.</param>
public record CreateAccountDbCommand(string Email) : IRequest<CommandResponse>;

internal class CreateAccountDbHandler(BonesDbContext dbContext, ISender sender)
    : IRequestHandler<CreateAccountDbCommand, CommandResponse>
{
    public async Task<CommandResponse> Handle(CreateAccountDbCommand request, CancellationToken cancellationToken)
    {
        CommandResponse emailAvailable =
            await sender.Send(new EmailAvailableForUseDbCommand(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        EntityEntry<Account> created = await dbContext.Accounts.AddAsync(new()
        {
            CreateDateTime = DateTime.UtcNow,
            Email = request.Email
        }, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}