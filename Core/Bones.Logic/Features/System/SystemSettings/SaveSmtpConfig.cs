using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System.SystemSettings;
using Bones.Database.Operations.System.SystemSettings.Models;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class SaveSmtpConfig(ISender sender) : IRequestHandler<SaveSmtpConfig.Command, CommandResponse>
{
    /// <summary>
    ///   Save the system SMTP configuration
    /// </summary>
    /// <param name="IsEnabled"></param>
    /// <param name="Config"></param>
    /// <param name="Reason"></param>
    /// <param name="ActionTakenBy"></param>
    public sealed record Command(bool IsEnabled, SmtpConfig Config, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {

        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        await sender.Send(new SaveSmtpEnabledDb.Command(request.IsEnabled, request.Reason, request.ActionTakenBy), cancellationToken);
        await sender.Send(new SaveSmtpConfigDb.Command(request.Config, request.Reason, request.ActionTakenBy), cancellationToken);

        return CommandResponse.Pass();
    }
}
