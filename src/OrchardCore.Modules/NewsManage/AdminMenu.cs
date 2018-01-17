using System;
using System.Linq;
using Microsoft.Extensions.Localization;
using NewsManage.Models;
using OrchardCore.ContentManagement;
using OrchardCore.Environment.Navigation;
using OrchardCore.Menu.Models;
using OrchardCore.Title.Model;
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
            if (!String.Equals(name, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var AdminMenu = _session.Query<ContentItem>().ListAsync().Result.Where(x => x.ContentType == "Menu" && x.Latest && x.As<TitlePart>().Title == "AdminMenu");
            if (AdminMenu.Count() == 1)
            {

                ContentItem AdminMenuItem = AdminMenu.First();
                var Menu = AdminMenuItem.As<MenuItemsListPart>().MenuItems;
                foreach (var MenuItem in Menu)
                {
                    string AAA = MenuItem.Content.ToString();
                    var Name = MenuItem.As<LinkMenuItemPart>().Name;
                    var Url = MenuItem.As<LinkMenuItemPart>().Url;
                    builder
                    .Add(T[Name], "2", contentParts => contentParts
                    .Action("List", "Admin", new { area = "NewsManage", ModelName = Url })
                    .Permission(Permissions.NewManage)
                    .LocalNav()
                    );
                    if (AAA.Contains("MenuItemsListPart") && MenuItem.As<MenuItemsListPart>().MenuItems.Count != 0)
                    {
                        var MenuChildren = MenuItem.As<MenuItemsListPart>().MenuItems;
                        foreach (var MenuItem1 in MenuChildren)
                        {
                            string AAA1 = MenuItem1.Content.ToString();
                            var Name2 = MenuItem1.As<LinkMenuItemPart>().Name;
                            var Url2 = MenuItem1.As<LinkMenuItemPart>().Url;
                            builder
                                .Add(T[Name], "2", contentParts => contentParts
                                .Action("List", "Admin", new { area = "NewsManage", ModelName = Url })
                                .Permission(Permissions.NewManage)
                                .Add(T[Name2], "2", conParts => conParts
                                .Action("List", "Admin", new { area = "NewsManage", ModelName = Url2 })
                                .Permission(Permissions.NewManage)
                                ));
                            if (AAA1.Contains("MenuItemsListPart") && MenuItem1.As<MenuItemsListPart>().MenuItems.Count != 0)
                            {
                                var MenuChildren1 = MenuItem.As<MenuItemsListPart>().MenuItems;
                                foreach (var MenuItem2 in MenuChildren1)
                                {
                                    string AAA2 = MenuItem2.Content.ToString();
                                    var Name3 = MenuItem2.As<LinkMenuItemPart>().Name;
                                    var Url3 = MenuItem2.As<LinkMenuItemPart>().Url;
                                    builder
                                        .Add(T[Name], "2", contentParts => contentParts
                                        .Action("List", "Admin", new { area = "NewsManage", ModelName = Url })
                                        .Permission(Permissions.NewManage)
                                        .Add(T[Name2], "2", conParts => conParts
                                        .Action("List", "Admin", new { area = "NewsManage", ModelName = Url2 })
                                        .Permission(Permissions.NewManage)
                                        .Add(T[Name3], "2", conarts => conarts
                                        .Action("List", "Admin", new { area = "NewsManage", ModelName = Url3 })
                                        .Permission(Permissions.NewManage)
                                        )));
                                    if (AAA2.Contains("MenuItemsListPart") && MenuItem2.As<MenuItemsListPart>().MenuItems.Count != 0)
                                    {
                                        builder
                                        .Add(T["导航添加出错"], "5", contentParts => contentParts
                                        .Action("AdminMenuError", "Admin", new { area = "NewsManage" })
                                        .Permission(Permissions.NewManage)
                                        .LocalNav()
                                    );
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                var contentItem = _session.Query<ContentItem>().ListAsync().Result.Where(x => x.ContentType == "新闻组管理" && x.Latest);
                if (contentItem.Count() != 0)
                {
                    foreach (var item in contentItem)
                    {
                        var NewDisplayName = item.As<NewPart>().NewDisplayName;
                        var Name = item.As<NewPart>().Name;
                        builder.Add(T["新闻组管理"], index => index
                                .Add(T[NewDisplayName], "3", contentParts => contentParts
                                    .Permission(Permissions.NewManage)
                                    .Action("List", "Admin", new { area = "NewsManage", ModelName = Name }))
                       );
                        builder
                             .Add(T["编辑管理"], index => index
                                .Add(T[NewDisplayName], "3", contentParts => contentParts
                                    .Permission(Permissions.NewEditor)
                                    .Action("List", "Admin", new { area = "NewsManage", ModelName = Name }))
                       );
                    }
                }
            }

            builder
                .Add(T["新闻组管理"], index => index
                    .Add(T["添加新闻类型"], "1", Top => Top
                        .Permission(Permissions.NewManage)
                        .Action("NewManage", "Admin", new { area = "NewsManage" }))
             );
            builder
                 .Add(T["查看发布"], index => index
                    .Permission(Permissions.NewManage)
                    .Action("ReleaseManage", "Admin", new { area = "NewsManage" })
            );
            builder.
                Add(T["新闻模块初始化"], initialSetup => initialSetup
                    .Permission(Permissions.NewManage)
                    .Action("NewSetup", "InitialSetup", new { area = "NewsManage" })
            );
        }

        
    }
}
