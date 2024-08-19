namespace Bones.Api.Controllers;

public class AuthController_Info
{
    accountGroup.MapGet("/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
    (ClaimsPrincipal claimsPrincipal, [FromServices] IServiceProvider sp) =>
    {
        UserManager<TUser> userManager = sp.GetRequiredService<UserManager<TUser>>();
        if (await userManager.GetUserAsync(claimsPrincipal) is not { } user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
    });

    [HttpGet("info", Name = "GetInfoAsync")]
    [ProducesResponseType<Ok<GetUserInfoResponse>>(StatusCodes.Status200OK, Type = typeof(Ok<GetUserInfoResponse>))]
    [ProducesResponseType<ValidationProblem>(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblem))]
    [ProducesResponseType<NotFound>(StatusCodes.Status404NotFound, Type = typeof(NotFound))]
    public async Task<Results<Ok<GetUserInfoResponse>, ValidationProblem, NotFound>> GetInfoAsync(ClaimsPrincipal claimsPrincipal)
    {
        QueryResponse<IdentityResult> result = await Sender.Send(new ConfirmEmailQuery()
        {
            UserId = userId,
            Code = code,
            ChangedEmail = changedEmail
        });

        if (result is { Success: true, Result: not null })
        {
            return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager);
        }

        return TypedResults.NotFound();
    }
}