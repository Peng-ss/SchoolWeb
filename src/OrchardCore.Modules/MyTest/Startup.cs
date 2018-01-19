using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Data.Migration;
using OrchardCore.Environment.Navigation;
using OrchardCore.Modules;

namespace NewsManage
{
    public class Startup :StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<INavigationProvider, AdminMenu>();
            services.AddScoped<IDataMigration, Migrations>();
        }
    }
}
