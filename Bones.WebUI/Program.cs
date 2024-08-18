using Bones.ApiClients;
using Bones.WebUI.Exceptions;
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
        builder.Services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new(builder.HostEnvironment.BaseAddress)
        });
        builder.Services.AddSingleton(_ =>
        {
            string apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? throw new BonesUiException("Missing ApiBaseUrl");
            return new Bones_Api_v0Client(apiBaseUrl, new()
            {
                BaseAddress = new(apiBaseUrl)
            });
        });

        await builder.Build().RunAsync();
    }
}