using System.Text;
using System.Text.Encodings.Web;
using Bones.Database.DbSets.Identity;
using Bones.Shared.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Bones.Api.Features.Identity.SendConfirmationEmail;

public class SendConfirmationEmailHandler(UserManager<BonesUser> userManager, IEmailSender<BonesUser> emailSender, LinkGenerator linkGenerator, IConfiguration configuration) : IRequestHandler<SendConfirmationEmailCommand, CommandResponse>
{
    private readonly string _webUiBaseUrl = configuration["WebUIBaseUrl"] ?? throw new BonesException("Web UI Base Url Not Found");
    private string WebUiConfirmEmailUrl => $"{_webUiBaseUrl}/confirm-email";
    
    public async Task<CommandResponse> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (_webUiBaseUrl is null)
        {
            throw new NotSupportedException("No email confirmation endpoint was registered!");
        }

        string code = request.IsChange
            ? await userManager.GenerateChangeEmailTokenAsync(request.User, request.Email)
            : await userManager.GenerateEmailConfirmationTokenAsync(request.User);
        
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        string userId = await userManager.GetUserIdAsync(request.User);
        RouteValueDictionary routeValues = new()
        {
            ["userId"] = userId,
            ["code"] = code,
        };

        if (request.IsChange)
        {
            // This is validated by the /confirmEmail endpoint on change.
            routeValues.Add("changedEmail", request.Email);
        }

        string confirmEmailUrl = linkGenerator.GetUriByAddress(request.Context, WebUiConfirmEmailUrl, routeValues)
                                 ?? throw new NotSupportedException($"Could not find endpoint named '{WebUiConfirmEmailUrl}'.");

        await emailSender.SendConfirmationLinkAsync(request.User, request.Email, HtmlEncoder.Default.Encode(confirmEmailUrl));

        return new()
        {
            Success = true
        };
    }
}