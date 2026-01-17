using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.DbSets.System;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class SaveWebUiBaseUrlDb(BonesDbContext dbContext) : IRequestHandler<SaveWebUiBaseUrlDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB command to save the Web UI's base URL
    /// </summary>
    /// <param name="WebUiBaseUrl">What the new value should be</param>
    /// <param name="Reason">The reason this is being changed</param>
    /// <param name="ActionTakenBy">Who changed it</param>
    public record Command(string WebUiBaseUrl, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        SystemSetting? setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.WebUIBaseUrl, cancellationToken);
        if (setting == null)
        {
            setting = new SystemSetting
            {
                Setting = SystemSetting.SettingType.WebUIBaseUrl,
                Value = request.WebUiBaseUrl
            };
            dbContext.SystemSettings.Add(setting);
        }
        else
        {
            setting.Value = request.WebUiBaseUrl;
            dbContext.SystemSettings.Update(setting);
        }

        SystemAudit audit = new()
        {
            ActionDateTime = DateTimeOffset.Now,
            ActionTaken = SystemAudit.Actions.SystemSettingUpdate,
            SettingChanged = SystemSetting.SettingType.WebUIBaseUrl,
            ActionTakenByUserId = request.ActionTakenBy.Id,
            Reason = request.Reason
        };
        dbContext.SystemAudits.Add(audit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}