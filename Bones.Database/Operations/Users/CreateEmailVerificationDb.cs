using Bones.Database.DbSets;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Users;

public class CreateEmailVerificationDb(BonesDbContext dbContext) : IRequestHandler<CreateEmailVerificationDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for generating an email verification token and queuing an email to be sent.
    /// </summary>
    /// <param name="UserId">User ID that must drink the verification can.</param>
    public record Command(Guid UserId) : IRequest<CommandResponse>;
    
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        EntityEntry<UserEmailVerification> created = await dbContext.UserEmailVerifications.AddAsync(new()
        {
            Token = Guid.NewGuid(),
            CreateDateTime = DateTimeOffset.UtcNow,
            ValidUntilDateTime = DateTimeOffset.UtcNow.AddDays(1),
            UserId = request.UserId
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