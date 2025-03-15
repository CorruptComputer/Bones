using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class GetSystemAdminMaskUserDb(BonesDbContext dbContext) : IRequestHandler<GetSystemAdminMaskUserDb.Query, QueryResponse<BonesUser?>>
{
    /// <summary>
    ///   DB query to get the SMTP configuration
    /// </summary>
    public record Query : IRequest<QueryResponse<BonesUser?>>;

    /// <inheritdoc />
    public async Task<QueryResponse<BonesUser?>> Handle(Query request, CancellationToken cancellationToken)
    {
        SystemSetting? setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.SystemAdminMaskUserId, cancellationToken);

        Guid? systemAdminMaskUserId;
        if (setting?.Value == null)
        {
            systemAdminMaskUserId = null;
        }
        else
        {
            bool success = Guid.TryParse(setting.Value, out Guid id);
            if (success)
            {
                systemAdminMaskUserId = id;
            }
            else
            {
                systemAdminMaskUserId = null;
            }
        }

        if (systemAdminMaskUserId == null)
        {
            return QueryResponse<BonesUser?>.Pass(null);
        }

        BonesUser? backgroundServiceUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == systemAdminMaskUserId, cancellationToken);
        return QueryResponse<BonesUser?>.Pass(backgroundServiceUser);
    }
}