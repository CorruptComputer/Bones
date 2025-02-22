using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class GetSystemAdminMaskUserEnabledDb(BonesDbContext dbContext) : IRequestHandler<GetSystemAdminMaskUserEnabledDb.Query, QueryResponse<bool>>
{
    /// <summary>
    ///   DB query to get the system mask user enabled setting
    /// </summary>
    public record Query : IRequest<QueryResponse<bool>>;

    /// <inheritdoc />
    public async Task<QueryResponse<bool>> Handle(Query request, CancellationToken cancellationToken)
    {
        SystemSetting? enabledSetting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.SystemAdminMaskUserEnabled, cancellationToken);

        if (enabledSetting?.Value != null && bool.TryParse(enabledSetting.Value, out bool enabled))
        {
            return QueryResponse<bool>.Pass(enabled);
        }

        return QueryResponse<bool>.Pass(false);
    }
}