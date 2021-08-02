using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Simucraft.Client.Common;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Blazor.DragDrop.Core;

namespace Simucraft.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services
                .AddHttpClient(
                    "Simucraft.Client.ServerAPI",
                    o => o.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
            builder.Services
                .AddTransient(o => o.GetRequiredService<IHttpClientFactory>()
                .CreateClient("Simucraft.Client.ServerAPI"));
            builder.Services.AddOptions();
            builder.Services.AddMsalAuthentication(o =>
            {
                builder.Configuration.Bind("AzureAdB2C", o.ProviderOptions.Authentication);
                o.ProviderOptions.DefaultAccessTokenScopes.Add("https://dunnjingames.onmicrosoft.com/fba67b83-ab64-4873-944e-141f97f764ff/API.Access");
            });

            builder.Services.AddBlazorDragDrop();
            builder.Services.AddSimucraft();

            await builder.Build().RunAsync();
        }
    }
}
