using Bones.Database.Operations.System.SystemSettings;
using Bones.Database.Operations.System.SystemSettings.Models;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class GetSmtpConfig(ISender sender) : IRequestHandler<GetSmtpConfig.Query, QueryResponse<SmtpConfig?>>
{
    /// <summary>
    ///   Query to get the SMTP configuration, or null if SMTP is disabled
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<SmtpConfig?>>;


    /// <inheritdoc />
    public async Task<QueryResponse<SmtpConfig?>> Handle(Query request, CancellationToken cancellationToken)
    {
        bool enabled = await sender.Send(new GetSmtpEnabledDb.Query(), cancellationToken);
        if (!enabled)
        {
            return QueryResponse<SmtpConfig?>.Pass(null);
        }

        return await sender.Send(new GetSmtpConfigDb.Query(), cancellationToken);
    }
}