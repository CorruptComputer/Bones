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
        SystemSetting? setting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.Smtp, cancellationToken);
        
        if (setting == null)
        {
            setting = new SystemSetting
            {
                Setting = SystemSetting.SettingType.Smtp,
                Value = JsonSerializer.Serialize(request.SmtpConfig)
            };
            dbContext.SystemSettings.Add(setting);
        }
        else
        {
            setting.Value = JsonSerializer.Serialize(request.SmtpConfig);
            dbContext.SystemSettings.Update(setting);
        }

        SystemAudit audit = new()
        {
            ActionDateTime = DateTimeOffset.Now,
            ActionTaken = SystemAudit.Actions.SystemSettingUpdate,
            SettingChanged = SystemSetting.SettingType.Smtp,
            ActionTakenBy = request.ActionTakenBy,
            Reason = request.Reason
        };
        dbContext.SystemAudits.Add(audit);

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return CommandResponse.Pass();
    }
}
