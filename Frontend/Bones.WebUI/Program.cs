using Bones.Api.Client;
using Bones.Shared.Exceptions;
using Bones.WebUI.Infrastructure;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

namespace Bones.WebUI;

public static class Program
{
    public static async Task Main(string[] args)
    {
        WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddMudServices();
        builder.Services.AddSingleton<LocalStorageService>();

        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddSingleton<BonesAuthenticationStateProvider>();
        builder.Services.AddSingleton<AuthenticationStateProvider>(s => s.GetRequiredService<BonesAuthenticationStateProvider>());
        builder.Services.AddScoped<CookieDelegatingHandler>();
        builder.Services.AddScoped<UnauthorizedDelegatingHandler>();

        string apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? throw new BonesException("Missing ApiBaseUrl");
        builder.Services.AddHttpClient(BonesApiClient.API_CLIENT_NAME, client =>
            {
                client.BaseAddress = new(apiBaseUrl);
            })
            .AddHttpMessageHandler<CookieDelegatingHandler>()
            .AddHttpMessageHandler<UnauthorizedDelegatingHandler>();

        builder.Services.AddSingleton<BonesApiClient>();
        await builder.Build().RunAsync();
    }
}