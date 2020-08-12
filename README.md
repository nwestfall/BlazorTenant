[![NuGet version](https://badge.fury.io/nu/BlazorTenant.svg)](https://badge.fury.io/nu/BlazorTenant)


# BlazorTenant

Support multi-tenantancy in your Blazor application.  You don't need to deploy multiple versions, just load a different config based on the url.

Tested and supported
 - Tenant identification from url (/TENANT/counter)
 - Change authentication based on tenant
 - Custom properties on the tenant to do... anything! (custom endpoints, custom tokens, feature flags, be creative!)

## How does it work?

Setup is pretty simple, especially if starting with a new project.  There are 3 main areas to look at.  They instructures below are for WebAssembly.

#### Program.cs
Wire up the library to do it's thing.  You will be creating a `TenantStore` (library comes included with an `InMemoryTenantStore`, but you can create your own) and adding that to the `ServiceCollection`.

```c#
using BlazorTenant;

...

public static async Task Main(string[] args)
{
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("app");

    // Create a TenantStore
    var store = new InMemoryTenantStore();
    // Add manually
    store.TryAdd(new Tenant("mycompany1", new Dictionary<string, string>()
    {
        "Property1", "value1"
    }));
    // Add from appsettings.json file
    var tenantSection = builder.Configuration.GetSection("Tenants");
    foreach(var tenant in tenantSection.GetChildren())
    {
        store.TryAdd(new Tenant(tenant.GetValue<string>("Identifier"), new Dictionary<string, string>()
        {
            "Property1", tenant.GetValue<string>("Property1")
        }));
    }
    // Add MultiTenantancy
    builder.Services.AddMultiTenantancy(store);

    var build = builder.Build();
    // We need the service provider for some fancy action
    build.Services.AddServiceProviderToMultiTenantRoutes();
    await build.RunAsync();
}
```

#### App.razor
Replace your `Router` with our new `MultiTenantRouter`.  There is also a new `RenderFragment` called `NoTenant` to let you display custom messages if the tenant does not exist.

```html
<MultiTenantRouter AppAssembly="@typeof(Program).Assembly">
    <Found Context="routeData">
        <RouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)" />
    </Found>
    <NotFound>
        <LayoutView Layout="@typeof(MainLayout)">
            <p>Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
    <NoTenant Context="message">
        <LayoutView Layout="@typeof(MainLayout)">
            <p>@message</p>
        </LayoutView>
    </NoTenant>
</MultiTenantRouter>
```

#### Update your NavigationManager
This one can be a little annoying and I plan to fix in the future.  When you use `NavigationManager`, you will need to include the current `Tenant.Identifier` so that the router works.  Next step would be to replace the `NavigationManager` with a `TenantNavigationManager` that does this for you.  But for now, you will have to do the following on your Razor pages

```html
@inject Tenant tenant
@inject NavigationManager navigationManager

<button @onclick="GoToPage">Go!</button>

@code {
    public void GoToPage()
    {
        navigationManager.NavigateTo($"{tenant.Identifier}/mypage");
    }
}
```

Feel free to look at the sample project as well.

## Why was this built?
Where I worked, I wanted to build a blazor app that sat on top of a few different systems.  Based on your tenant, you could have a different OIDC provider, maybe a different endpoint for your data, and even some features weren't available based on where you lived.  The goal of this application was to just build a new front end on existing APIs, using Blazor WebAssembly.  Oh, and it had to be a PWA.

Few problems with having different tenants in Blazor
 - How do you configure different OIDC paths, client ID, etc
 - How do you handle different endpoints for you data?
 - Yes, a lot of this data can be stored in `appsettings.json`, but this can have integrity checks so I can't make it dynamic easily
 - I want to run this in S3 without ANY servers... so I'm relying on just my files to be correct, no fancy tricks

A lot of inspiration was taken from [Finbuckle.MultiTenant](https://www.finbuckle.com/MultiTenant).  Recently, they appear to have started supporting Blazor, but not WebAssembly (from what I can tell, please correct me).  My goal was to have a completely offline solution, so I didn't want to use Blazor Server or something else.

I was learning Blazor for the first time while building this, but it helped me learn the project **very** quickly.  This library will probably evolve over time as I learn more, but I'm very happy with the outcome in our application.

## Future Plans
 [ ] Build a `TenantNavigationManager`

 [ ] Try to make `ServiceProvider` not included
 
 [ ] Make an API based `TenantStore`
