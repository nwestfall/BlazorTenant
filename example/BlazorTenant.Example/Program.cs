using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

namespace BlazorTenant.Example
{
    public class Program
    {
        internal static IServiceProvider _sp;

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var store = new InMemoryTenantStore();
            var tenantSection = builder.Configuration.GetSection("Tenants");
            foreach(var tenant in tenantSection.GetChildren())
            {
                var authSection = tenant.GetSection("Auth");
                store.TryAdd(new Tenant(tenant.GetValue<string>("Identifier"), new Dictionary<string, string>()
                {
                    { "ApiUri", tenant.GetValue<string>("ApiUri") },
                    { "Auth.Authority", authSection.GetValue<string>("Authority") },
                    { "Auth.ClientId", authSection.GetValue<string>("ClientId") },
                    { "Auth.ResponseType", authSection.GetValue<string>("ResponseType") }
                }));
            }
            builder.Services.AddMultiTenantancy(store);
            builder.Services.Configure<RemoteAuthenticationOptions<OidcProviderOptions>>(options =>
            {
                var tenant = _sp.GetRequiredService<Tenant>();
                var nav = _sp.GetRequiredService<NavigationManager>();
                if(!string.IsNullOrEmpty(tenant?.Identifier))
                {
                    options.ProviderOptions.ClientId = tenant.Parameters["Auth.ClientId"];
                    options.ProviderOptions.Authority = tenant.Parameters["Auth.Authority"];
                    options.ProviderOptions.ResponseType = tenant.Parameters["Auth.ResponseType"];
                    MultiTenantRemoteAuthenticationPaths.AssignPathsOptionsForTenantOrDefault(tenant, nav, options);
                }
            });

            builder.Services.AddOidcAuthentication(options =>
            {
                options.ProviderOptions.DefaultScopes.Clear();
                options.ProviderOptions.DefaultScopes.Add("openid profile offline_access api");
            });

            var build = builder.Build();
            _sp = build.Services;
            build.Services.AddServiceProviderToMultiTenantRoutes();
            await build.RunAsync();
        }
    }
}
