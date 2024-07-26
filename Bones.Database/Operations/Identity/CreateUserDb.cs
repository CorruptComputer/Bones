using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Bones.Database.Operations.Identity;

public sealed class CreateUserDb(BonesDbContext dbContext, ISender sender) : IRequestHandler<CreateUserDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for creating a user.
    /// </summary>
    /// <param name="Email">Email address to use for the User.</param>
    public record Command(string Email) : IValidatableRequest<CommandResponse>
    {
        /// <inheritdoc />
        public bool IsRequestValid()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                return false;
            }

            return true;
        }
    }



    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        CommandResponse emailAvailable =
            await sender.Send(new EmailAvailableForUseDb.Command(request.Email), cancellationToken);
        if (!emailAvailable.Success)
        {
            return emailAvailable;
        }

        EntityEntry<BonesUser> created = await dbContext.Users.AddAsync(new()
        {
            CreateDateTime = DateTimeOffset.UtcNow,
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