using Bones.Database.DbSets.Identity;
using Bones.Shared.Exceptions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Identity;

public sealed class CreateEmailVerificationDb(BonesDbContext dbContext) : IRequestHandler<CreateEmailVerificationDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for generating an email verification token and queuing an email to be sent.
    /// </summary>
    /// <param name="UserId">User ID that must drink the verification can.</param>
    public record Command(Guid UserId) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (UserId == Guid.Empty)
            {
                return false;
            }

            return true;
        }
    }

    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser? user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return new()
            {
                Success = false,
                FailureReason = $"Unable to find user by ID: {request.UserId}"
            };
        }

        EntityEntry<UserEmailVerification> created = await dbContext.UserEmailVerifications.AddAsync(new()
        {
            Token = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            ValidUntilDateTime = DateTimeOffset.UtcNow.AddDays(1),
            User = user
        }, cancellationToken);

        // TODO: Might want to actually queue an email to be sent at some point

        await dbContext.SaveChangesAsync(cancellationToken);

        return new()
        {
            Success = true,
            Id = created.Entity.Id
        };
    }
}