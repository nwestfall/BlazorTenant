using System;

namespace BlazorTenant
{
    /// <summary>
    /// Tenant Exception
    /// </summary>
    internal class TenantException : Exception
    {
        /// <summary>
        /// Tenant Exception Ctor
        /// </summary>
        /// <param name="message"></param>
        public TenantException(string message)
            : base(message) { }
    }
}