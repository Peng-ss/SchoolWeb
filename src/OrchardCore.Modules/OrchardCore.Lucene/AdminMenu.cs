using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Navigation;
using System;

namespace OrchardCore.Lucene
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
                .Add(T["Design"], "10", design => design
                    .AddClass("menu-design").Id("design")
                    .Add(T["Site"], "10", import => import
                        .Add(T["Lucene Indices"], "7", indexes => indexes
                            .Action("Index", "Admin", new { area = "OrchardCore.Lucene" })
                            .Permission(Permissions.ManageIndexes)
                            .LocalNav())
                        .Add(T["Lucene Queries"], "8", queries => queries
                            .Action("Query", "Admin", new { area = "OrchardCore.Lucene" })
                            .Permission(Permissions.ManageIndexes)
                            .LocalNav())))
                .Add(T["Design"], design => design
                    .Add(T["Settings"], settings => settings
                        .Add(T["Search"], T["Search"], entry => entry
                            .Action("Index", "Admin", new { area = "OrchardCore.Settings", groupId = "search" })
                            .LocalNav()
                        )));

        }
    }
}
