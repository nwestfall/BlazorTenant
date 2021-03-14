using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace BlazorTenant
{
    /// <summary>
    /// In Memory Tenant Store
    /// </summary>
    public class InMemoryTenantStore : ITenantStore
    {
        readonly ConcurrentDictionary<string, Tenant> _tenants = new ConcurrentDictionary<string, Tenant>();

        /// <summary>
        /// Try and add tenant to store
        /// </summary>
        /// <param name="tenant">Tenant</param>
        /// <returns>True if added</returns>
        public virtual bool TryAdd(Tenant tenant)
        {
            if(tenant.Identifier != null)
                return _tenants.TryAdd(tenant.Identifier.ToLower(), tenant);

            return false;
        }

        /// <summary>
        /// Try and get the tenant
        /// </summary>
        /// <param name="identifier">Tenant identifier</param>
        /// <returns>The tenant or null</returns>
        public virtual Tenant? TryGet(string? identifier)
        {
            if(identifier != null && _tenants.TryGetValue(identifier, out Tenant? tenant))
                return tenant;

            return null;
        }

        /// <summary>
        /// Try and remove tenant by identifier
        /// </summary>
        /// <param name="identifier">The tenant identifier</param>
        /// <returns>True if removed</returns>
        public virtual bool TryRemove(string identifier)
            => _tenants.TryRemove(identifier, out Tenant _);

        /// <summary>
        /// Try and update the tenant
        /// </summary>
        /// <param name="tenant">The tenant</param>
        /// <returns>True if updated</returns>
        public virtual bool TryUpdate(Tenant tenant)
        {
            if(tenant.Identifier != null)
            {
                var oldTenant = TryGet(tenant.Identifier);
                if(oldTenant != null)
                    return _tenants.TryUpdate(tenant.Identifier.ToLower(), tenant, oldTenant);
            }

            return false;
        }
    }
}