// https://github.com/dotnet/aspnetcore/blob/master/src/Components/Components/src/Routing/RouteContext.cs
// Added support for tenant

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorTenant
{
    internal class MultiTenantRouteContext
    {
        internal static IServiceProvider? Services { get; set; }

        private static char[] Separator = new[] { '/' };
        ILogger<MultiTenantRouteContext> _logger;

        public MultiTenantRouteContext(string path, IRouteTable routeTable, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MultiTenantRouteContext>();

            // This is a simplification. We are assuming there are no paths like /a//b/. A proper routing
            // implementation would be more sophisticated.
            var segments = path.Trim('/').Split(Separator, StringSplitOptions.RemoveEmptyEntries);
            // Individual segments are URL-decoded in order to support arbitrary characters, assuming UTF-8 encoding.
            // MultiTenant is in first path
            if(segments.Length != 0)
            {
                var tenantIdentifier = segments[0];
                if(routeTable.MatchesRoute(tenantIdentifier))
                {
                    _logger.LogWarning($"Tenant Name matches part of a route.  This is not allowed ({tenantIdentifier})");
                    if(Services == null)
                        throw new ArgumentNullException("Services not setup");
                    Services.GetRequiredService<Tenant>().Identifier = null;
                    Services.GetRequiredService<Tenant>().Parameters = new Dictionary<string, string>();
                    throw new TenantException("Invalid tenant name (part of another path)");
                }
                else
                {
                    var tenant = ValidTenant(tenantIdentifier);
                    if(tenant == null)
                    {
                        _logger.LogWarning($"Tenant not found in store ({tenantIdentifier})");
                        if(Services == null)
                            throw new ArgumentNullException("Services not setup");
                        Services.GetRequiredService<Tenant>().Identifier = null;
                        Services.GetRequiredService<Tenant>().Parameters = new Dictionary<string, string>();
                        throw new TenantException("Tenant not in store");
                    }
                    else
                    {
                        _logger.LogDebug($"Tenant found");
                        Segments = new string[segments.Length - 1];
                        if(Services == null)
                            throw new ArgumentNullException("Services not setup");
                        Services.GetRequiredService<Tenant>().Identifier = tenantIdentifier;
                        Services.GetRequiredService<Tenant>().Parameters = tenant.Parameters;
                        // Change config
                        for (int i = 1; i < segments.Length; i++)
                        {
                            Segments[i - 1] = Uri.UnescapeDataString(segments[i]);
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("No tenant was specified");
                if(Services == null)
                        throw new ArgumentNullException("Services not setup");
                Services.GetRequiredService<Tenant>().Identifier = null;
                Services.GetRequiredService<Tenant>().Parameters = new Dictionary<string, string>();
                throw new TenantException("No tenant specified");
            }
        }

        Tenant? ValidTenant(string identifier)
        {
            if(Services == null)
                        throw new ArgumentNullException("Services not setup");
            var store = Services.GetRequiredService<ITenantStore>();
            return store.TryGet(identifier);
        }

        public string[] Segments { get; }

        public Type? Handler { get; set; }

        public IReadOnlyDictionary<string, object?>? Parameters { get; set; }
    }
}