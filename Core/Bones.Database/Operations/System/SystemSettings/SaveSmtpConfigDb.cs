using System.Text.Json;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.Audit;
using Bones.Database.DbSets.System;
using Bones.Database.Operations.System.SystemSettings.Models;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class SaveSmtpConfigDb(BonesDbContext dbContext) : IRequestHandler<SaveSmtpConfigDb.Command, CommandResponse>
{
    /// <summary>
    ///   DB command to save the SMTP configuration
    /// </summary>
    /// <param name="SmtpConfig"></param>
    /// <param name="Reason"></param>
    /// <param name="ActionTakenBy"></param>
    public record Command(SmtpConfig SmtpConfig, string Reason, BonesUser ActionTakenBy) : IRequest<CommandResponse>;

    /// <inheritdoc />
    public async Task<CommandResponse> Handle(Command request, CancellationToken cancellationToken)
    {
        SystemSetting? configSetting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.SmtpConfig, cancellationToken);

        if (configSetting == null)
        {
            configSetting = new SystemSetting
            {
                Setting = SystemSetting.SettingType.SmtpConfig,
                Value = JsonSerializer.Serialize(request.SmtpConfig)
            };
            dbContext.SystemSettings.Add(configSetting);
        }
        else
        {
            configSetting.Value = JsonSerializer.Serialize(request.SmtpConfig);
            dbContext.SystemSettings.Update(configSetting);
        }

        SystemAudit configAudit = new()
        {
            ActionDateTime = DateTimeOffset.Now,
            ActionTaken = SystemAudit.Actions.SystemSettingUpdate,
            SettingChanged = SystemSetting.SettingType.SmtpConfig,
            ActionTakenBy = request.ActionTakenBy,
            Reason = request.Reason
        };
        dbContext.SystemAudits.Add(configAudit);

        await dbContext.SaveChangesAsync(cancellationToken);

        return CommandResponse.Pass();
    }
}
