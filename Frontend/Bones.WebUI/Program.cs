using System.Net.Http.Headers;
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

        builder.Services.AddTransient<CookieDelegatingHandler>();
        builder.Services.AddTransient<UnauthorizedDelegatingHandler>();

        string thisHost = new Uri(builder.HostEnvironment.BaseAddress).Authority;
        string? apiBaseUrl = builder.Configuration["ApiBaseUrl"]
            ?.Replace("{{THIS_HOST}}", thisHost);

        if (string.IsNullOrWhiteSpace(apiBaseUrl))
        {
            throw new BonesException("Missing ApiBaseUrl");
        }

        builder.Services.AddHttpClient(BonesApiClient.HTTP_CLIENT_NAME, client =>
            {
                client.BaseAddress = new(apiBaseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
            })
            .AddHttpMessageHandler<CookieDelegatingHandler>()
            .AddHttpMessageHandler<UnauthorizedDelegatingHandler>();

        builder.Services.AddTransient<BonesApiClient>();

        await builder.Build().RunAsync();
    }
}