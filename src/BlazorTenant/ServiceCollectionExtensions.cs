using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorTenant
{
    /// <summary>
    /// Service Collection Extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add multi-tenancy to the service collection
        /// </summary>
        /// <param name="services">The service colleciton</param>
        /// <param name="tenantStore">The tenant store</param>
        /// <returns></returns>
        public static IServiceCollection AddMultiTenantancy(this IServiceCollection services, ITenantStore tenantStore)
        {
            services.TryAddScoped<Tenant>();
            services.TryAddSingleton<ITenantStore>(tenantStore);

            return services;
        }

        /// <summary>
        /// Add the service provider to the route context (required)
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static void AddServiceProviderToMultiTenantRoutes(this IServiceProvider serviceProvider)
        {
            MultiTenantRouteContext.Services = serviceProvider;
        }
    }
}