using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using OrchardCore.Environment.Extensions;
using OrchardCore.Modules;

namespace Microsoft.AspNetCore.Builder
{
    public static class ModularApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseModules(this IApplicationBuilder app, Action<ModularApplicationBuilder> modules = null)
        {
            // Ensure the shell tenants are loaded when a request comes in
            // and replaces the current service provider for the tenant's one.
            app.UseMiddleware<ModularTenantContainerMiddleware>();

            app.ConfigureModules(modules);

            app.UseMiddleware<ModularTenantRouterMiddleware>();

            return app;
        }

        public static IApplicationBuilder ConfigureModules(this IApplicationBuilder app, Action<ModularApplicationBuilder> modules)
        {
            var modularApplicationBuilder = new ModularApplicationBuilder(app);
            modules?.Invoke(modularApplicationBuilder);

            return app;
        }

        public static ModularApplicationBuilder UseStaticFilesModules(this ModularApplicationBuilder modularApp)
        {
            modularApp.Configure(app =>
            {
                var extensionManager = app.ApplicationServices.GetRequiredService<IExtensionManager>();
                var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();

                // TODO: configure the location and parameters (max-age) per module.
                var availableExtensions = extensionManager.GetExtensions();
                foreach (var extension in availableExtensions)
                {
                    var contentPath = Path.Combine(extension.ExtensionFileInfo.PhysicalPath, "Content");
                    var contentSubPath = Path.Combine(extension.SubPath, "Content");

                    if (Directory.Exists(contentPath))
                    {
                        IFileProvider fileProvider;
                        if (env.IsDevelopment())
                        {
                            fileProvider = new CompositeFileProvider(
                                new ModuleProjectContentFileProvider(env.ContentRootPath, contentSubPath),
                                new PhysicalFileProvider(contentPath));
                        }
                        else
                        {
                            fileProvider = new PhysicalFileProvider(contentPath);
                        }

                        app.UseStaticFiles(new StaticFileOptions
                        {
                            RequestPath = "/" + extension.Id,
                            FileProvider = fileProvider
                        });
                    }
                }
            });

            return modularApp;
        }
    }
}