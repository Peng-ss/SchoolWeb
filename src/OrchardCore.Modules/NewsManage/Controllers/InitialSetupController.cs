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
            string[] Role1 = new string[16] {"NewManage","AccessAdminPanel","EditOwnContent","EditContent","ViewOwnContent","PreviewOwnContent","PreviewContent",
            "ViewContentTypes","EditContentTypes","PublishOwnContent","PublishContent","DeleteOwnContent","DeleteContent","ViewContent","SetHomepage","ContentPreview"};
            string[] Role2 = new string[7] {"NewEditor","AccessAdminPanel","EditOwnContent","EditContent","ViewOwnContent","PreviewOwnContent","PreviewContent" };
            List<RoleClaim> rolePermissions1 = new List<RoleClaim>();
            List<RoleClaim> rolePermissions2 = new List<RoleClaim>();
            for (var i = 0; i < Role1.Length; i++)
            {
                rolePermissions1.Add(new RoleClaim { ClaimType = "Permission", ClaimValue = Role1[i] });
            }
            for (var i = 0; i < Role2.Length; i++)
            {
                rolePermissions2.Add(new RoleClaim { ClaimType = "Permission", ClaimValue = Role2[i] });
            }
            //添加角色：新闻管理者
            CreateRole(new CreateRoleViewModel { RoleName = "新闻管理者" }, rolePermissions1);
            //添加角色：新闻编辑者
            CreateRole(new CreateRoleViewModel { RoleName = "新闻编辑者" }, rolePermissions2);
            return View();
        }

        //新建模版函数
        public async void CreateIndexTemplate()
        {
            var MyTemplate = new TemplateViewModel { };
            MyTemplate.Name = "Content__Page";
            MyTemplate.Description = "A template for the  Page content type";
            MyTemplate.Content = "<script>   " +
"  var NewTypeData;  " +
"  $.ajax({" +
"  async: false," +
"  type:\'get\'," +
"  url:\"/New/NewsManage/Display/ReadTypeContents\"," +
"  success:function(Content){" +
"  NewTypeData = Content;" +
"  }" +
"  });" +
"  function createShowingNewType(data) {" +
"  var Str = \"<a href =\'/New\'><img src = \'/New/media/index_02.gif\' style =\'width:10%; height: 40px;float:left\'></a>\";" +
"  for (var i = 0; i < data.length; i++) {" +
"    Str = Str + \"<a href =\'/New/NewsManage/Display/TypeDisplayIndex?ContentType=\"+ data[i].NewPart.Name+" +
"            \"\'><img src = \'/New/media/\" +data[i].NewPart.Name+ \".gif\' style =\'width:10%; height: 40px;float:left\'></a>\";" +
"    }" +
"  $(\"#NewType\").html(Str);" +
" }" +
"                                  $(function() {" +
"                                  createShowingNewType(NewTypeData);" +
"                                  var Type0ImgStr =\" <img src= \'/New/media/\"+NewTypeData[0].NewPart.NewDescription+" +
"                                  \".gif\' style = \'float:left;margin-left:1px;\' >\";" +
"                                  $(\"#Type0Img\").html(Type0ImgStr);" +
"                                  });                   " +
"                                  </script>" +
"  <center> " +
"<div  style =\"width:70%;\">" +
"    <img src = \"/New/media/index_01.gif\" style =\"width:100%; height: 131px;\">" +
"    <div id =\"NewType\">  </div>     " +
"    <div style=\"width:100%;\">" +
"      <div style=\"width:100%;height:40px;\">" +
"      </div>      " +
"      <div style=\"width:100%;height:40px;\">" +
"      </div>        " +
"      <div style=\"width:34%;height:450px;float:left;border:1px solid #cdf5ed;\">" +
"        <div id=\"Type4Img\" style=\"width:100%;height:40px;\">" +
"        </div >         " +
"      </div>    	      " +
"      <div style=\"width:66%;height:650px;float:left;border:1px solid #cdf5ed;\">" +
"        <script>              " +
"          $.ajax({" +
"          type:\'get\'," +
"          url:\"/New/NewsManage/Display/TypeDisplay?ContentType=\"+NewTypeData[0].NewPart.Name," +
"          success:function(Content){" +
"          ShowingNewTypeContent(Content);" +
"          }      					" +
"          });" +
"          function ShowingNewTypeContent(data) {" +
"          var Str = \"\";  				" +
"          for (var i =(data.length-1); i>-1;i--) {" +
"          Str = Str +\" <li ><a href=\'/New/NewsManage/Display/NewPartDisplay?ContentType=\"+data[i].ContentType+" +
"            \"&ContentItemId=\"+data[i].ContentItemId+\"\' title=\'\"+data[i].TitlePart.Title+\"\'>\"+data[i].TitlePart.Title+" +
"          \"</a><span style=\'float:right;margin-right:1%\'>\"+data[i].CreatedUtc.substring(0,10)+\"</span></li>\";" +
"          }                  " +
"          $(\"#ul0\").html(Str);" +
"          }                       " +
"        </script>          " +
"        <div id=\"Type0Img\" style=\"width:100%;height:40px;\">" +
"        </div >      " +
"        <div style=\"width:100%;height:410px;\">" +
"          <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"box\">" +
"            <tbody>                        " +
"              <tr>                          " +
"                <td>                            " +
"                  <ul id=\"ul0\"></ul>                           " +
"                </td>                        " +
"              </tr>                       " +
"            </tbody>             " +
"          </table>         " +
"        </div>      " +
"      </div>      " +
"    </div>     " +
"    </div>  " +
"  </center>";
           

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
