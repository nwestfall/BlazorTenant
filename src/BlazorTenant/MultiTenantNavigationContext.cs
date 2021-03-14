// https://github.com/dotnet/aspnetcore/blob/main/src/Components/Components/src/Routing/NavigationContext.cs

using System.Threading;

namespace BlazorTenant
{
    /// <summary>
    /// Provides information about the current asynchronous navigation event
    /// including the target path and the cancellation token.
    /// </summary>
    public class MultiTenantNavigationContext
    {
        internal MultiTenantNavigationContext(string path, CancellationToken cancellationToken)
        {
            Path = path;
            CancellationToken = cancellationToken;
        }
        
        /// <summary>
        /// The target path for the navigation.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// The <see cref="CancellationToken"/> to use to cancel navigation.
        /// </summary>
        public CancellationToken CancellationToken { get; }
    }
}