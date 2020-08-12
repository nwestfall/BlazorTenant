// https://github.com/dotnet/aspnetcore/blob/master/src/Components/Components/src/Routing/RouteTable.cs

using System;

namespace BlazorTenant
{
    internal class MultiTenantRouteTable
    {
        public MultiTenantRouteTable(MultiTenantRouteEntry[] routes)
        {
            Routes = routes;
        }

        public MultiTenantRouteEntry[] Routes { get; }

        internal void Route(MultiTenantRouteContext routeContext)
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

        internal bool MatchesRoute(string tenantIdentifier)
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