using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Localization;
using OrchardCore.Environment.Navigation;
using YesSql;

namespace NewsManage
{
    public class AdminMenu : INavigationProvider
    {
        public AdminMenu(ISession session, IStringLocalizer<AdminMenu> localizer)
        {
            _session = session;
            T = localizer;
        }

        private readonly ISession _session;
        public IStringLocalizer T { get; set; }
        public void BuildNavigation(string name, NavigationBuilder builder)
        {
            builder
                 .Add(T["测试1"], index => index
                    .Action("Test1", "MyTest", new { area = "MyTest" })
            );
        }
    }
}
