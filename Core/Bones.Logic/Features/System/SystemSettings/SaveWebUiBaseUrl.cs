using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class SaveWebUiBaseUrl(ISender sender) : IRequestHandler<SaveWebUiBaseUrl.Command, CommandResponse>
{
    /// <summary>
    ///   Command to save the base URL for the web UI
    /// </summary>
    /// <param name="Url"></param>
    /// <param name="Reason"></param>
    /// <param name="ActionTakenBy"></param>
    public sealed record Command(string Url, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public sealed class Validator : AbstractValidator<Command>
    {
        /// <inheritdoc />
        public Validator()
        {
            RuleFor(request => request.Url).NotEmpty().Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("Url is not a valid URL");
        }
    }

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        await sender.Send(new SaveWebUiBaseUrlDb.Command(request.Url, request.Reason, request.ActionTakenBy), cancellationToken);

        return CommandResponse.Pass();
    }
}
