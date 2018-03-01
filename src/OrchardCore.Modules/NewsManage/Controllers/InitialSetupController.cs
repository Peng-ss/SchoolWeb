using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using OrchardCore.Admin;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Roles.ViewModels;
using OrchardCore.Security;
using OrchardCore.Security.Permissions;
using OrchardCore.Templates.Controllers;
using OrchardCore.Templates.Models;
using OrchardCore.Templates.Services;
using OrchardCore.Templates.ViewModels;
using YesSql;

namespace NewsManage.Controllers
{

    //初始化设置控制器
    public class InitialSetupController : Controller, IUpdateModel
    {
        private readonly TemplatesManager _templatesManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly RoleManager<IRole> _roleManager;
        private readonly ISession _session;
        private readonly IHtmlLocalizer<AdminController> TH;
        private readonly INotifier _notifier;

        public IStringLocalizer T { get; set; }

        public InitialSetupController(IAuthorizationService authorizationService, 
            TemplatesManager templatesManager,
            RoleManager<IRole> roleManager,
            IHtmlLocalizer<AdminController> htmlLocalizer,
            ISession session,
            INotifier notifier,
            IStringLocalizer<TemplateController> stringLocalizer)
        {
            _notifier = notifier;
            TH = htmlLocalizer;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _templatesManager = templatesManager;
            T = stringLocalizer;
            _session = session;
        }



        [Admin]
        public ActionResult NewSetup()
        {
            //新建Page模版
            CreateIndexTemplate();
            //string[] Role1 = new string[16] {"NewManage","AccessAdminPanel","EditOwnContent","EditContent","ViewOwnContent","PreviewOwnContent","PreviewContent",
            //"ViewContentTypes","EditContentTypes","PublishOwnContent","PublishContent","DeleteOwnContent","DeleteContent","ViewContent","SetHomepage","ContentPreview"};
            //string[] Role2 = new string[7] {"NewEditor","AccessAdminPanel","EditOwnContent","EditContent","ViewOwnContent","PreviewOwnContent","PreviewContent" };
            //List<RoleClaim> rolePermissions1 = new List<RoleClaim>();
            //List<RoleClaim> rolePermissions2 = new List<RoleClaim>();
            //for (var i = 0; i < Role1.Length; i++)
            //{
            //    rolePermissions1.Add(new RoleClaim { ClaimType = "Permission", ClaimValue = Role1[i] });
            //}
            //for (var i = 0; i < Role2.Length; i++)
            //{
            //    rolePermissions2.Add(new RoleClaim { ClaimType = "Permission", ClaimValue = Role2[i] });
            //}
            ////添加角色：新闻管理者
            //CreateRole(new CreateRoleViewModel { RoleName = "新闻管理者" }, rolePermissions1);
            ////添加角色：新闻编辑者
            //CreateRole(new CreateRoleViewModel { RoleName = "新闻编辑者" }, rolePermissions2);
            return View();
        }

        //新建模版函数
        public async void CreateIndexTemplate()
        {
            var MyTemplate = new TemplateViewModel { };
            MyTemplate.Name = "Content__Page";
            MyTemplate.Description = "A template for the  Page content type";
            MyTemplate.Content = "<style>" +
"    .button{border:2px solid #C5C5C5;background:#EEEEEE;margin:0 4px;}" +
"    .search{border:2px solid #C5c5c5;}" +
"</style>" +
"<center>" +
"    <div class=\"container\" width=\"960\">" +
"            " +
"            " +
"    </div>" +
"</center>";
            if (ModelState.IsValid)
            {
                if (String.IsNullOrWhiteSpace(MyTemplate.Name))
                {
                    ModelState.AddModelError(nameof(TemplateViewModel.Name), T["The name is mandatory."]);
                }
            }

            if (ModelState.IsValid)
            {
                var template = new Template { Content = MyTemplate.Content, Description = MyTemplate.Description };

                await _templatesManager.UpdateTemplateAsync(MyTemplate.Name, template);

            }

        }

        //添加角色函数
        public async void CreateRole(CreateRoleViewModel model, List<RoleClaim> rolePermissions)
        {
            if (ModelState.IsValid)
            {
                model.RoleName = model.RoleName.Trim();
                if (await _roleManager.FindByNameAsync(_roleManager.NormalizeKey(model.RoleName)) != null)
                {
                    ModelState.AddModelError(string.Empty, T["The role is already used."]);
                }
            }
            if (ModelState.IsValid)
            {
            
                var Newrole = new Role { RoleName = model.RoleName };
                var result = await _roleManager.CreateAsync(Newrole);

                _session.Cancel();//session.Cancel()可以使用调用来阻止当前更改被提交。
                                  //session.CommitAsync()，即使事务处理完毕后实际上将提交事务

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var role = (Role)await _roleManager.FindByNameAsync(_roleManager.NormalizeKey(model.RoleName));
            role.RoleClaims.RemoveAll(c => c.ClaimType == Permission.ClaimType);
            role.RoleClaims.AddRange(rolePermissions);

            await _roleManager.UpdateAsync(role);
        }

    }
}
