using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class GetBackgroundServiceUserDb(BonesDbContext dbContext) : IRequestHandler<GetBackgroundServiceUserDb.Query, QueryResponse<BonesUser?>>
{
    /// <summary>
    ///   DB query to get the SMTP configuration
    /// </summary>
    public record Query : IRequest<QueryResponse<BonesUser?>>;

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser?>> Handle(Query request, CancellationToken cancellationToken)
    {
        SystemSetting? setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.BackgroundServiceUserId, cancellationToken);

        Guid? backgroundServiceUserId;
        if (setting?.Value == null)
        {
            backgroundServiceUserId = null;
        }
        else
        {
            bool success = Guid.TryParse(setting.Value, out Guid id);
            if (success)
            {
                backgroundServiceUserId = id;
            }
            else
            {
                backgroundServiceUserId = null;
            }
        }

        if (backgroundServiceUserId == null)
        {
            return QueryResponse<BonesUser?>.Pass(null);
        }

        BonesUser? backgroundServiceUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == backgroundServiceUserId, cancellationToken);
        return QueryResponse<BonesUser?>.Pass(backgroundServiceUser);
    }
}