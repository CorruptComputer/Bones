using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class SaveSystemAdminMaskUserEnabledDb(BonesDbContext dbContext) : IRequestHandler<SaveSystemAdminMaskUserEnabledDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB command to save the SMTP enabled setting
    /// </summary>
    /// <param name="Enabled"></param>
    /// <param name="Reason"></param>
    /// <param name="ActionTakenBy"></param>
    public record Command(bool Enabled, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        SystemSetting? enabledSetting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.SystemAdminMaskUserEnabled, cancellationToken);
        if (enabledSetting == null)
        {
            enabledSetting = new SystemSetting
            {
                Setting = SystemSetting.SettingType.SystemAdminMaskUserEnabled,
                Value = request.Enabled.ToString()
            };
            dbContext.SystemSettings.Add(enabledSetting);
        }
        else
        {
            enabledSetting.Value = request.Enabled.ToString();
            dbContext.SystemSettings.Update(enabledSetting);
        }

        SystemAudit enabledAudit = new()
        {
            ActionDateTime = DateTimeOffset.Now,
            ActionTaken = SystemAudit.Actions.SystemSettingUpdate,
            SettingChanged = SystemSetting.SettingType.SystemAdminMaskUserEnabled,
            ActionTakenByUserId = request.ActionTakenBy.Id,
            Reason = request.Reason
        };
        dbContext.SystemAudits.Add(enabledAudit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}