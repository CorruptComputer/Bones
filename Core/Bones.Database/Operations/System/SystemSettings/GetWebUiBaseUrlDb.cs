using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class GetWebUiBaseUrlDb(BonesDbContext dbContext) : IRequestHandler<GetWebUiBaseUrlDb.Query, QueryResponse<string>>
{
    /// <summary>
    ///   DB query to get the Web UI's base URL, generally used for generating links in emails
    /// </summary>
    public record Query : IRequest<QueryResponse<string>>;

    /// <inheritdoc />
    public async Task<QueryResponse<string>> Handle(Query request, CancellationToken cancellationToken)
    {
        SystemSetting? setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.WebUIBaseUrl, cancellationToken);
        return QueryResponse<string>.Pass(setting?.Value ?? string.Empty);
    }
}
