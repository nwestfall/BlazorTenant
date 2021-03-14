using System;
using Xunit;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace BlazorTenant.Tests
{
    public class MultiTenantRouterTests
    {
        private readonly TestNavigationManager _navigationManager;

        public MultiTenantRouterTests()
        {
            _navigationManager = new TestNavigationManager();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void NoTenant(bool preferExactMatches)
        {
            using var ctx = new TestContext();

            SetupContext(ctx);

            _navigationManager.NotifyLocationChanged("https://www.example.com/nocompany", false);

            RenderFragment<RouteData> found = __builder =>
            {
                RenderFragment foundContentrf = __builder =>
                {
                    __builder.AddContent(0, "Found");
                };
                return foundContentrf;
            };

            RenderFragment notFound = __builder =>
            {
                __builder.AddContent(0, "Not Found");
            };

            RenderFragment<string> noTenant = __builder =>
            {
                RenderFragment noTenantrf = __builder =>
                {
                    __builder.AddContent(0, "No Tenant");
                };
                return noTenantrf;
            };

            var cut = ctx.RenderComponent<MultiTenantRouter>(
                ("AppAssembly", typeof(BlazorTenant.Example.Program).Assembly),
                ("PreferExactMatches", preferExactMatches),
                ("Found",  found),
                ("NotFound", notFound),
                ("NoTenant", noTenant)
            );

            cut.MarkupMatches("No Tenant");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ValidTenant(bool preferExactMatches)
        {
            using var ctx = new TestContext();

            SetupContext(ctx);

            _navigationManager.NotifyLocationChanged("https://www.example.com/company1", false);

            RenderFragment<RouteData> found = __builder =>
            {
                RenderFragment foundContentrf = __builder =>
                {
                    __builder.AddContent(0, "Found");
                };
                return foundContentrf;
            };

            RenderFragment notFound = __builder =>
            {
                __builder.AddContent(0, "Not Found");
            };

            RenderFragment<string> noTenant = __builder =>
            {
                RenderFragment noTenantrf = __builder =>
                {
                    __builder.AddContent(0, "No Tenant");
                };
                return noTenantrf;
            };

            var cut = ctx.RenderComponent<MultiTenantRouter>(
                ("AppAssembly", typeof(BlazorTenant.Example.Program).Assembly),
                ("PreferExactMatches", preferExactMatches),
                ("Found",  found),
                ("NotFound", notFound),
                ("NoTenant", noTenant)
            );

            cut.MarkupMatches("Found");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ValidTenantInvalidPage(bool preferExactMatches)
        {
            using var ctx = new TestContext();

            SetupContext(ctx);

            _navigationManager.NotifyLocationChanged("https://www.example.com/company1/nopage", false);

            RenderFragment<RouteData> found = __builder =>
            {
                RenderFragment foundContentrf = __builder =>
                {
                    __builder.AddContent(0, "Found");
                };
                return foundContentrf;
            };

            RenderFragment notFound = __builder =>
            {
                __builder.AddContent(0, "Not Found");
            };

            RenderFragment<string> noTenant = __builder =>
            {
                RenderFragment noTenantrf = __builder =>
                {
                    __builder.AddContent(0, "No Tenant");
                };
                return noTenantrf;
            };

            var cut = ctx.RenderComponent<MultiTenantRouter>(
                ("AppAssembly", typeof(BlazorTenant.Example.Program).Assembly),
                ("PreferExactMatches", preferExactMatches),
                ("Found",  found),
                ("NotFound", notFound),
                ("NoTenant", noTenant)
            );

            cut.MarkupMatches("Not Found");
        }

        void SetupContext(TestContext ctx)
        {
            ctx.Services.AddSingleton<NavigationManager>(_navigationManager);
            var tenantStore = new InMemoryTenantStore();
            tenantStore.TryAdd(new Tenant("company1", null));
            ctx.Services.AddMultiTenantancy(tenantStore);
            ctx.Services.AddSingleton<INavigationInterception, TestNavigationInterception>();

            ctx.Services.AddServiceProviderToMultiTenantRoutes();
        }

        internal class TestNavigationManager : NavigationManager
        {
            public TestNavigationManager() =>
                Initialize("https://www.example.com/", "https://www.example.com/");

            protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotImplementedException();

            public void NotifyLocationChanged(string uri, bool intercepted)
            {
                Uri = uri;
                NotifyLocationChanged(intercepted);
            }
        }

        internal sealed class TestNavigationInterception : INavigationInterception
        {
            public static readonly TestNavigationInterception Instance = new TestNavigationInterception();

            public Task EnableNavigationInterceptionAsync()
            {
                return Task.CompletedTask;
            }
        }
    }
}
