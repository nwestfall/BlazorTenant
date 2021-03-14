// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/Routing/LegacyRouteMatching/LegacyRouteTable.cs

using System;

namespace BlazorTenant
{
    internal class LegacyMultiTenantRouteTable : IRouteTable
    {
        public LegacyMultiTenantRouteTable(LegacyMultiTenantRouteEntry[] routes)
        {
            Routes = routes;
        }

        public LegacyMultiTenantRouteEntry[] Routes { get; }

        public void Route(MultiTenantRouteContext routeContext)
        {
            for (var i = 0; i < Routes.Length; i++)
            {
                Routes[i].Match(routeContext);
                if (routeContext.Handler != null)
                {
                    return;
                }
            }
        }

        public bool MatchesRoute(string tenantIdentifier)
        {
            for(var i = 0; i < Routes.Length; i++)
            {
                if(Routes[i].Match(tenantIdentifier))
                {
                    return true;
                }
            }

            return false;
        }
    }
}