using Microsoft.AspNetCore.Authorization;
using OrchardCore.Modules;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.CustomSettings.Services;
using OrchardCore.DisplayManagement.Handlers;
using OrchardCore.Environment.Navigation;
using OrchardCore.Layers.Drivers;
using OrchardCore.Security.Permissions;
using OrchardCore.Settings;

namespace OrchardCore.CustomSettings
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IDisplayDriver<ISite>, CustomSettingsDisplayDriver>();
            services.AddScoped<CustomSettingsService>();

            // Permissions
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<IAuthorizationHandler, CustomSettingsAuthorizationHandler>();
        }
    }
}
