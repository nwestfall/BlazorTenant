using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorTenant
{
    /// <summary>
    /// MultiTenant RemoteAuthenticationPaths
    /// </summary>
    public static class MultiTenantRemoteAuthenticationPaths
    {
        /// <summary>
        /// Auto assign auth settings from tenant
        /// Add tenant to callback paths
        /// </summary>
        /// <param name="tenant">The tenant</param>
        /// <param name="navigationManager">The navigation manage</param>
        /// <param name="options">The options</param>
        public static void AssignPathsOptionsForTenantOrDefault(Tenant tenant, NavigationManager navigationManager, RemoteAuthenticationOptions<OidcProviderOptions> options)
        {
            if(tenant != null)
            {
                options.AuthenticationPaths.LogInCallbackPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LoginCallbackPath}";
                options.AuthenticationPaths.LogInFailedPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LoginFailedPath}";
                options.AuthenticationPaths.LogInPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LoginPath}";
                options.AuthenticationPaths.LogOutCallbackPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LogoutCallbackPath}";
                options.AuthenticationPaths.LogOutFailedPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LogoutFailedPath}";
                options.AuthenticationPaths.LogOutPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LogoutPath}";
                options.AuthenticationPaths.LogOutSucceededPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.LogoutSucceededPath}";
                options.AuthenticationPaths.ProfilePath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.ProfilePath}";
                options.AuthenticationPaths.RegisterPath = $"{tenant.Identifier}/{RemoteAuthenticationDefaults.RegisterPath}";

                options.ProviderOptions.PostLogoutRedirectUri = $"{navigationManager.BaseUri}{options.AuthenticationPaths.LogOutCallbackPath}";
                options.ProviderOptions.RedirectUri = $"{navigationManager.BaseUri}{options.AuthenticationPaths.LogInCallbackPath}";
            }
        }
    }
}