namespace BlazorTenant
{
    /// <summary>
    /// Tenant Store Interface
    /// </summary>
    public interface ITenantStore
    {
        /// <summary>
        /// Try to add the Tenant to the store.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool TryAdd(Tenant tenant);

        /// <summary>
        /// Try to update the Tenant in the store.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool TryUpdate(Tenant tenant);

        /// <summary>
        /// Try to remove the Tenant from the store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool TryRemove(string identifier);

        /// <summary>
        /// Retrieve the Tenant for a given tenant Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Tenant? TryGet(string? identifier);
    }
}