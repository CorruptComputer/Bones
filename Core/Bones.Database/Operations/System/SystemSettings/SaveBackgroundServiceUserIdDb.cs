using Bones.Database.DbSets.Accounts;
using Bones.Database.DbSets.Audits;
using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class SaveBackgroundServiceUserIdDb(BonesDbContext dbContext) : IRequestHandler<SaveBackgroundServiceUserIdDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB command to save the Web UI's base URL
    /// </summary>
    /// <param name="BackgroundServiceUserId">The ID of the user</param>
    /// <param name="Reason">The reason this is being changed</param>
    /// <param name="ActionTakenBy">Who changed it</param>
    public record Command(Guid BackgroundServiceUserId, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        SystemSetting? setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.BackgroundServiceUserId, cancellationToken);
        if (setting == null)
        {
            setting = new SystemSetting
            {
                Setting = SystemSetting.SettingType.BackgroundServiceUserId,
                Value = request.BackgroundServiceUserId.ToString()
            };
            dbContext.SystemSettings.Add(setting);
        }
        else
        {
            setting.Value = request.BackgroundServiceUserId.ToString();
            dbContext.SystemSettings.Update(setting);
        }

        SystemAudit audit = new()
        {
            ActionDateTime = DateTimeOffset.Now,
            ActionTaken = SystemAudit.Actions.SystemSettingUpdate,
            SettingChanged = SystemSetting.SettingType.BackgroundServiceUserId,
            ActionTakenByUserId = request.ActionTakenBy.Id,
            Reason = request.Reason
        };
        dbContext.SystemAudits.Add(audit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
