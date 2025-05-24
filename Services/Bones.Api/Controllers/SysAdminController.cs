using Bones.Api.Controllers.Base;
using Bones.Api.Models.SysAdmin;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.Operations.System.SystemSettings.Models;
using Bones.Logic.Features.System.SystemSettings;
using Bones.Logic.Features.SystemAdmin;
using Bones.Shared.Consts;
using Microsoft.AspNetCore.Authorization;

namespace Bones.Api.Controllers;

/// <summary>
///   Handles SysAdmin stuffs
/// </summary>
[Authorize(Roles = SystemRoles.SYSTEM_ADMINISTRATORS)]
public class SysAdminController(ISender sender) : AuthenticatedControllerBase(sender)
{
    #region GET
    /// <summary>
    ///     Gets the system admin dashboard
    /// </summary>
    /// <returns>Ok with the results if successful, otherwise BadRequest with a message of what went wrong.</returns>
    [HttpGet("dashboard", Name = "GetSystemAdminDashboardAsync")]
    [ProducesResponseType<GetSystemAdminDashboardResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetSystemAdminDashboardResponse>> GetSystemAdminDashboardAsync()
    {
        GetSystemAdminDashboardData.Response? a = await Sender.Send(new GetSystemAdminDashboardData.Query());

        if (a is null)
        {
            return BadRequest(EmptyResponse.Value);
        }

        GetSystemAdminDashboardResponse response = new()
        {
            UserCount = a.UserCount,
            OrganizationCount = a.OrganizationCount,
            ProjectCount = a.PtojectCount,
            ItemCount = a.ItemCount
        };

        return Ok(response);
    }

    /// <summary>
    ///   Gets the background service user config
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings/background-service-user-config", Name = "GetBackgroundServiceUserConfigAsync")]
    [ProducesResponseType<GetBackgroundServiceUserConfigResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetBackgroundServiceUserConfigResponse>> GetBackgroundServiceUserConfigAsync()
    {
        BonesUser? user = await Sender.Send(new GetBackgroundServiceUser.Query());

        if (user is null)
        {
            return BadRequest(EmptyResponse.Value);
        }

        return Ok(GetBackgroundServiceUserConfigResponse.FromInternal(user));
    }

    /// <summary>
    ///   Gets the system admin mask user config
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings/system-admin-mask-user-config", Name = "GetSystemAdminMaskUserConfigAsync")]
    [ProducesResponseType<GetSystemAdminMaskUserConfigResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetSystemAdminMaskUserConfigResponse>> GetSystemAdminMaskUserConfigAsync()
    {
        bool isEnabled = await Sender.Send(new GetSystemAdminMaskUserEnabled.Query());
        BonesUser? user = await Sender.Send(new GetSystemAdminMaskUser.Query());

        return Ok(GetSystemAdminMaskUserConfigResponse.FromInternal(isEnabled, user));
    }

    /// <summary>
    ///   Gets the Web UI's base URL configured
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings/web-ui-base-url", Name = "GetWebUiBaseUrlAsync")]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<string>> GetWebUiBaseUrlAsync()
    {
        string? baseUrl = await Sender.Send(new GetWebUiBaseUrl.Query());

        return Ok(baseUrl ?? string.Empty);
    }

    /// <summary>
    ///   Gets the SMTP configuration
    /// </summary>
    /// <returns></returns>
    [HttpGet("settings/smtp-config", Name = "GetSmtpConfigAsync")]
    [ProducesResponseType<GetSmtpConfigResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<GetSmtpConfigResponse>> GetSmtpConfigAsync()
    {
        bool isEnabled = await Sender.Send(new GetSmtpEnabled.Query());
        SmtpConfig? config = await Sender.Send(new GetSmtpConfig.Query());

        return Ok(GetSmtpConfigResponse.FromInternal(isEnabled, config));
    }
    #endregion

    #region POST
    /// <summary>
    ///   Saves the background service user config
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("settings/background-service-user-config", Name = "SaveBackgroundServiceUserConfigAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<EmptyResponse>> SaveBackgroundServiceUserConfigAsync([FromBody] SaveBackgroundServiceUserConfigRequest request)
    {
        CommandResponse result = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));

        if (!result.Success)
        {
            return BadRequest(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Saves the system admin mask user config
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("settings/system-admin-mask-user-config", Name = "SaveSystemAdminMaskUserConfigAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<EmptyResponse>> SaveSystemAdminMaskUserConfigAsync([FromBody] SaveSystemAdminMaskUserConfigRequest request)
    {
        CommandResponse result = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));

        if (!result.Success)
        {
            return BadRequest(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Saves the Web UI's base URL
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("settings/web-ui-base-url", Name = "SaveWebUiBaseUrlAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<EmptyResponse>> SaveWebUiBaseUrlAsync([FromBody] SaveWebUiBaseUrlRequest request)
    {
        CommandResponse result = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));

        if (!result.Success)
        {
            return BadRequest(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }

    /// <summary>
    ///   Saves the SMTP configuration
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("settings/smtp-config", Name = "SaveSmtpConfigAsync")]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<EmptyResponse>(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<EmptyResponse>> SaveSmtpConfigAsync([FromBody] SaveSmtpConfigRequest request)
    {
        CommandResponse result = await Sender.Send(request.ToInternal(await GetCurrentBonesUserAsync()));

        if (!result.Success)
        {
            return BadRequest(EmptyResponse.Value);
        }

        return Ok(EmptyResponse.Value);
    }
    #endregion
}