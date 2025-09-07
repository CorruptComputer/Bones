using System.Net;
using Bones.Database.Operations.Audit;

namespace Bones.Logic.Features.Audits;

/// <inheritdoc />
public class AddLoginAudit(ISender sender) : IRequestHandler<AddLoginAudit.Command, CommandResponse>
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
        return await sender.Send(new AddLoginAuditDb.Command(request.Email, request.Successful, request.RequestingIpAddress), cancellationToken);
    }
}