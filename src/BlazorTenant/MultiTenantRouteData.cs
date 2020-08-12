// https://github.com/dotnet/aspnetcore/blob/master/src/Components/Components/src/Routing/RouteData.cs

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace BlazorTenant
{
    /// <summary>
    /// Describes information determined during routing that specifies
    /// the page to be displayed.
    /// </summary>
    public sealed class MultiTenantRouteData
    {
        /// <summary>
        /// Constructs an instance of <see cref="MultiTenantRouteData"/>.
        /// </summary>
        /// <param name="pageType">The type of the page matching the route, which must implement <see cref="IComponent"/>.</param>
        /// <param name="routeValues">The route parameter values extracted from the matched route.</param>
        public MultiTenantRouteData(Type pageType, IReadOnlyDictionary<string, object> routeValues)
        {
            if (pageType == null)
            {
                throw new ArgumentNullException(nameof(pageType));
            }

            if (!typeof(IComponent).IsAssignableFrom(pageType))
            {
                throw new ArgumentException($"The value must implement {nameof(IComponent)}.", nameof(pageType));
            }

            PageType = pageType;
            RouteValues = routeValues ?? throw new ArgumentNullException(nameof(routeValues));
        }

        /// <summary>
        /// Gets the type of the page matching the route.
        /// </summary>
        public Type PageType { get; }

        /// <summary>
        /// Gets route parameter values extracted from the matched route.
        /// </summary>
        public IReadOnlyDictionary<string, object> RouteValues { get; }
    }
}