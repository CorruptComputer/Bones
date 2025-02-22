using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class GetSmtpEnabled(ISender sender) : IRequestHandler<GetSmtpEnabled.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<bool>>;


    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetSmtpEnabledDb.Query(), cancellationToken);
    }
}
