using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NewsManage.Drivers;
using NewsManage.Models;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Navigation;
using OrchardCore.Modules;
using OrchardCore.Security.Permissions;

namespace NewsManage
{
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPermissionProvider, Permissions>();
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IDataMigration, Migrations>();
            services.AddScoped<IContentPartDisplayDriver, TypeNewClassifyPartDriver>();
            services.AddSingleton<ContentPart, TypeNewClassifyPart>();


        }
    }
}
