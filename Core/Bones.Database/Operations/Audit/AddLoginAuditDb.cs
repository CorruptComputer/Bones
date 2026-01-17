using System.Net;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;

namespace Bones.Database.Operations.Audit;

/// <inheritdoc />
public class AddLoginAuditDb(BonesDbContext dbContext) : IRequestHandler<AddLoginAuditDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB Command for creating a new login audit entry
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Successful"></param>
    /// <param name="RequestingIpAddress"></param>
    public record Command(string Email, bool Successful, IPAddress RequestingIpAddress) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.RequestingIpAddress).NotNull();
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        BonesUser? account = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email, cancellationToken);
        bool accountKnown = account is not null;

        LoginAudit audit = new()
        {
            BonesUserId = account?.Id,
            UnknownEmail = accountKnown ? null : request.Email,
            LoginDateTime = DateTimeOffset.Now,
            Successful = request.Successful,
            RequestingIpAddress = request.RequestingIpAddress,
        };

        dbContext.LoginAudits.Add(audit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
