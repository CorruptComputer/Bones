using System.Text.Json;
using Bones.Database.DbSets.System;
using Bones.Database.Operations.System.SystemSettings.Models;

namespace Bones.Database.Operations.System.SystemSettings;

/// <inheritdoc />
public class GetSmtpConfigDb(BonesDbContext dbContext) : IRequestHandler<GetSmtpConfigDb.Query, QueryResponse<SmtpConfig?>>
{
    /// <summary>
    ///   DB query to get the SMTP configuration
    /// </summary>
    public record Query : IRequest<QueryResponse<SmtpConfig?>>;

    /// <inheritdoc />
    public async Task<QueryResponse<SmtpConfig?>> Handle(Query request, CancellationToken cancellationToken)
    {
        SystemSetting? configSetting = await dbContext.SystemSettings.FirstOrDefaultAsync(s => s.Setting == SystemSetting.SettingType.SmtpConfig, cancellationToken);

        SmtpConfig? smtpConfig;
        if (configSetting?.Value == null)
        {
            smtpConfig = null;
        }
        else
        {
            // If parsing fails, oh well! Time to reconfigure it you sad administator (probably me).
            // I should really make every effort to ensure that these are valid and backwards compatible though.
            smtpConfig = JsonSerializer.Deserialize<SmtpConfig>(configSetting.Value);
        }

        return QueryResponse<SmtpConfig?>.Pass(smtpConfig);
    }
}

