using Bones.Database.Operations.System.SystemSettings;

namespace Bones.Logic.Features.System.SystemSettings;

/// <inheritdoc />
public class GetWebUiBaseUrl(ISender sender) : IRequestHandler<GetWebUiBaseUrl.Query, QueryResponse<string>>
{
    /// <summary>
    ///   Query to get the base URL for the web UI
    /// </summary>
    public sealed record Query : IRequest<QueryResponse<string>>;


    /// <inheritdoc />
    public async Task<QueryResponse<string>> Handle(Query request, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetWebUiBaseUrlDb.Query(), cancellationToken) ?? string.Empty;
    }
}
