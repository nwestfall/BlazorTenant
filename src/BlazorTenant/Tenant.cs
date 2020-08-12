using System.Collections.Generic;

namespace BlazorTenant
{
    /// <summary>
    /// Tenant
    /// </summary>
    public class Tenant
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public Tenant() { }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="identifier">Identifier</param>
        /// <param name="parameters">Parameters (optional)</param>
        public Tenant(string identifier, IDictionary<string, string> parameters)
        {
            Identifier = identifier;
            Parameters = parameters;
        }

        /// <summary>
        /// Tenant URL Identifier
        /// </summary>
        /// <value></value>
        public string Identifier { get; internal set; }

        /// <summary>
        /// Tenant Parameters
        /// </summary>
        /// <typeparam name="string"></typeparam>
        /// <typeparam name="string"></typeparam>
        /// <returns></returns>
        public IDictionary<string, string> Parameters { get; internal set; } = new Dictionary<string, string>();
    }
}