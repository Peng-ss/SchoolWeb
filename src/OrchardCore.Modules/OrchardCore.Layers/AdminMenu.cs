using System;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Navigation;
using OrchardCore.Layers.Drivers;

namespace OrchardCore.Layers
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(IStringLocalizer<AdminMenu> localizer)
        {
            T = localizer;
        }

        public IStringLocalizer T { get; set; }
        
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            builder
                .Add(T["Design"], design => design
                    .Add(T["Settings"], settings => settings
                        .Add(T["Layers"], T["Layers"], layers => layers
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = LayerSiteSettingsDisplayDriver.GroupId })
                            .LocalNav()
                        )))
                .Add(T["Content"], design => design
                    .Add(T["Layers"], "5", layers => layers
                        .Permission(Permissions.ManageLayers)
                        .Action("Index", "Admin", new { area = "OrchardCore.Layers" })
                        .LocalNav()
                    ));
        }
    }
}
