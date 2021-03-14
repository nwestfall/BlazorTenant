// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/Routing/IRouteTable.cs

namespace BlazorTenant
{
    /// <summary>
    /// Provides an abstraction over <see cref="RouteTable"/> and <see cref="LegacyRouteMatching.LegacyRouteTable"/>.
    /// This is only an internal implementation detail of <see cref="Router"/> and can be removed once
    /// the legacy route matching logic is removed.
    /// </summary>
    internal interface IRouteTable
    {
        void Route(MultiTenantRouteContext routeContext);

        bool MatchesRoute(string tenantIdentifier);
    }
}