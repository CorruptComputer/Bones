using Bones.Database.DbSets;
using Bones.Database.Models;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Accounts;

/// <summary>
///     DB Command for generating an email verification token and queuing an email to be sent.
/// </summary>
/// <param name="AccountId">Account ID that must drink the verification can.</param>
public record CreateEmailVerificationDbCommand(long AccountId) : IRequest<DbCommandResponse>;

internal class CreateEmailVerificationDbHandler(BonesDbContext dbContext)
    : IRequestHandler<CreateEmailVerificationDbCommand, DbCommandResponse>
{
    public async Task<DbCommandResponse> Handle(CreateEmailVerificationDbCommand request,
        CancellationToken cancellationToken)
    {
        EntityEntry<AccountEmailVerification> created = await dbContext.AccountEmailVerifications.AddAsync(new()
        {
            Token = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            ValidUntilDateTime = DateTimeOffset.UtcNow.AddDays(1),
            AccountId = request.AccountId
        }, cancellationToken);

        // TODO: Might want to actually queue an email to be sent at some point

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.EmailVerificationId
        };
    }
}