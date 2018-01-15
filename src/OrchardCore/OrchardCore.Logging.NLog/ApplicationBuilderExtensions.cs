using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.LayoutRenderers;
using NLog.Web;

namespace OrchardCore.Logging
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNLogWeb(this IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            LayoutRenderer.Register<TenantLayoutRenderer>(TenantLayoutRenderer.LayoutRendererName);
            loggerFactory.AddNLog();
            app.AddNLogWeb();
            env.ConfigureNLog($"{env.ContentRootPath}{Path.DirectorySeparatorChar}NLog.config");
            LogManager.Configuration.Variables["configDir"] = env.ContentRootPath;

            return app;
        }
    }
}
