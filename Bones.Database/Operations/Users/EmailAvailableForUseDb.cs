using Bones.Database.DbSets;
using Bones.Database.DbSets.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bones.Database.Operations.Users;

public class EmailAvailableForUseDb(BonesDbContext dbContext) : IRequestHandler<EmailAvailableForUseDb.Command, CommandResponse>
{
    /// <summary>
    ///     DB Command for checking if an email is available to use.
    /// </summary>
    /// <param name="Email">Email address to check</param>
    public record Command(string Email) : IRequest<CommandResponse>;
    
    public async Task<CommandResponse> Handle(Command request,
        CancellationToken cancellationToken)
    {
        BonesUser? User =
            await dbContext.Users.FirstOrDefaultAsync(a => a.Email == request.Email, cancellationToken);

        if (User != null)
        {
            return new()
            {
                Success = false,
                FailureReason = "Email already in use on an existing User."
            };
        }

        return new()
        {
            Success = true
        };
    }
}