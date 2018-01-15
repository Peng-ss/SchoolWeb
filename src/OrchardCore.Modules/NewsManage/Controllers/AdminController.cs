using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NewsManage.Common;
using NewsManage.Models;
using NewsManage.ViewModels;
using OrchardCore.Body.Settings;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentManagement.Records;
using OrchardCore.Contents.Services;
using OrchardCore.Contents.ViewModels;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.ContentTypes.Services;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.Modules;
using OrchardCore.Navigation;
using OrchardCore.Settings;
using OrchardCore.Templates.Controllers;
using OrchardCore.Templates.Models;
using OrchardCore.Templates.Services;
using OrchardCore.Templates.ViewModels;
using OrchardCore.Title.Model;
using YesSql;
using YesSql.Services;

namespace NewsManage.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {
       
        private readonly IContentManager _contentManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISession _session;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionDisplayManager _contentDefinitionDisplayManager;
        private readonly ISiteService _siteService;
        private readonly IEnumerable<IContentAdminFilter> _contentAdminFilters;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly TemplatesManager _templatesManager;
        public dynamic New { get; set; }
        public ILogger Logger { get; set; }

        public IStringLocalizer T { get; set; }
        public AdminController(
            ISession session,
            ISiteService siteService,
            IContentManager contentManager,
            IAuthorizationService authorizationService,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionDisplayManager contentDefinitionDisplayManager,
            IShapeFactory shapeFactory,
            IContentItemDisplayManager contentItemDisplayManager,
            IEnumerable<IContentAdminFilter> contentAdminFilters,
            ILogger<AdminController> logger,
            IContentDefinitionManager contentDefinitionManager,
            TemplatesManager templatesManager,
            IStringLocalizer<TemplateController> stringLocalizer
          )
        {
            _contentDefinitionDisplayManager = contentDefinitionDisplayManager;
            _contentManager = contentManager;
            _authorizationService = authorizationService;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionService = contentDefinitionService;
            _session = session;
            _siteService = siteService;
            New = shapeFactory;
            _contentAdminFilters = contentAdminFilters;
            _contentItemDisplayManager = contentItemDisplayManager;
            Logger = logger;
            _templatesManager = templatesManager;
            T = stringLocalizer;
        }
        public ActionResult NewManage()
        {
           //新建一个Page模版
           CreateIndexTemplate();
            return View();
        }

        public ActionResult ReleaseManage()
        {
            return View();
        }
        //返回所有的新闻内容
        public JsonResult ReadPartContents()
        {
            var Data = new List<object>();
            var NewTypeData = _session.Query<ContentItem>().ListAsync().Result
                .Where(x => x.ContentType == "新闻组管理" && x.Latest)
                .Select(m => new NewPartVM
                {
                    NewID = m.ContentItemId,
                    NewDisplayName = m.As<NewPart>().NewDisplayName,
                    NewDescription = m.As<NewPart>().NewDescription,
                    Name = m.As<NewPart>().Name,
                });
            foreach (var item in NewTypeData)
            {
                var NewPartData = _session.Query<ContentItem>().ListAsync().Result
                    .Where(x => x.ContentType == item.Name && x.Latest);
                Data.Add(NewPartData);
            }
            return Json(Data);
        }

        #region TypeContents
        public JsonResult CreateTypeContents()
        {
            var temp = this.DeserializeObject<IEnumerable<NewPartVM>>();
            foreach (var item in temp)
            {
                ContentItem DevicesInfoContentItem = _contentManager.New("新闻组管理");
                DevicesInfoContentItem.Alter<NewPart>(x => x.NewDisplayName = item.NewDisplayName);
                DevicesInfoContentItem.Alter<NewPart>(x => x.NewDescription = item.NewDescription);
                item.Name = "ContentType_" + item.NewDisplayName;
                if (_contentDefinitionManager.GetTypeDefinition(item.Name) == null)
                {
                    var BodyPartSetting = new BodyPartSettings { };
                    BodyPartSetting.Editor = "Wysiwyg";
                    _contentDefinitionService.AddType(item.Name, item.NewDisplayName);
                    _contentDefinitionManager.AlterTypeDefinition(item.Name, bulid => bulid
                                                                    .Draftable()
                                                                    .Creatable()
                                                                    .Listable()
                                                                    .WithPart("TitlePart")
                                                                    .WithPart("BodyPart", part => part.WithSettings(BodyPartSetting))
                                                                    );

                    //把添加的新闻内容类型加到ContentType——新闻主页
                    //_contentDefinitionService.AddReusablePartToType(item.Name, item.NewDisplayName, item.NewDescription, "BagPart", "新闻主页");
                    //_contentDefinitionManager.AlterTypeDefinition("新闻主页", menu => menu
                    //    .WithPart(item.Name, part => part
                    //        .WithPosition("1")
                    //       ));
                }
                DevicesInfoContentItem.Alter<TitlePart>(x => x.Title = item.NewDisplayName);
                DevicesInfoContentItem.Alter<NewPart>(x => x.Name = item.Name);
                _contentDefinitionManager.GetTypeDefinition(item.NewDisplayName);
                _contentManager.Create(DevicesInfoContentItem);
                item.NewID = DevicesInfoContentItem.ContentItemId;

            }
            return this.Jsonp(temp);
        }
        public JsonResult ReadTypeContents()
        {
            var list = _session.Query<ContentItem>().ListAsync().Result
                .Where(x => x.ContentType == "新闻组管理" && x.Latest)
                .Select(m => new NewPartVM
                {
                    NewID = m.ContentItemId,
                    NewDisplayName = m.As<NewPart>().NewDisplayName,
                    NewDescription = m.As<NewPart>().NewDescription,
                    Name = m.As<NewPart>().Name,

                });
            return this.Jsonp(list);
        }
        public async Task<JsonResult> UpdateTypeContents()
        {
            var temp = this.DeserializeObject<IEnumerable<NewPartVM>>();
            foreach (var item in temp)
            {
                ContentItem DevicesInfoContentItem = await _contentManager.GetAsync(item.NewID);
                DevicesInfoContentItem.Alter<NewPart>(x => x.NewDisplayName = item.NewDisplayName);
                DevicesInfoContentItem.Alter<NewPart>(x => x.NewDescription = item.NewDescription);
                if (_contentDefinitionManager.GetTypeDefinition(item.Name) == null) { }
                else
                {
                    var BodyPartSetting = new BodyPartSettings { };
                    BodyPartSetting.Editor = "Wysiwyg";
                    _contentDefinitionManager.AlterTypeDefinition(item.Name, bulid => bulid
                    .DisplayedAs(item.NewDisplayName)
                    .Draftable()
                    .Creatable()
                    .Listable()
                    .WithPart("TitlePart")
                    .WithPart("BodyPart", part => part.WithSettings(BodyPartSetting))
                    );
                }
                DevicesInfoContentItem.Alter<TitlePart>(x => x.Title = item.NewDisplayName);
                _session.Save(DevicesInfoContentItem);
            }
            return this.Jsonp(temp);
        }
        public async Task<JsonResult> DestroyTypeContents()
        {
            var temp = this.DeserializeObject<IEnumerable<NewPartVM>>();
            foreach (var item in temp)
            {
                ContentItem DeviceItem = await _contentManager.GetAsync(item.NewID);
                if (_contentDefinitionManager.GetTypeDefinition(item.Name) == null)
                {
                }
                else
                {
                    _contentDefinitionService.RemoveType(item.Name, true);
                    _contentDefinitionService.RemovePartFromType(item.NewDisplayName, "新闻主页");
                }
                await _contentManager.RemoveAsync(DeviceItem);
            }
            return this.Jsonp(temp);
        }

        #endregion

        public async Task<IActionResult> List(ListContentsViewModel model, PagerParameters pagerParameters, string ModelName)
        {
            var siteSettings = await _siteService.GetSiteSettingsAsync();
            Pager pager = new Pager(pagerParameters, siteSettings.PageSize);
            model.Id = ModelName;
            var query = _session.Query<ContentItem, ContentItemIndex>();

            switch (model.Options.ContentsStatus)
            {
                case ContentsStatus.Published:
                    query = query.With<ContentItemIndex>(x => x.Published);
                    break;
                case ContentsStatus.Draft:
                    query = query.With<ContentItemIndex>(x => x.Latest && !x.Published);
                    break;
                case ContentsStatus.AllVersions:
                    query = query.With<ContentItemIndex>(x => x.Latest);
                    break;
                default:
                    query = query.With<ContentItemIndex>(x => x.Latest);
                    break;
            }

            if (!string.IsNullOrEmpty(model.TypeName))
            {
                var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(model.TypeName);
                if (contentTypeDefinition == null)
                    return NotFound();

                model.TypeDisplayName = contentTypeDefinition.ToString();

                // We display a specific type even if it's not listable so that admin pages
                // can reuse the Content list page for specific types.
                query = query.With<ContentItemIndex>(x => x.ContentType == model.TypeName);
            }
            else
            {
                var listableTypes = (await GetListableTypesAsync()).Select(t => t.Name).ToArray();
                if (listableTypes.Any())
                {
                    query = query.With<ContentItemIndex>(x => x.ContentType.IsIn(listableTypes));
                }
            }

            switch (model.Options.OrderBy)
            {
                case ContentsOrder.Modified:
                    query = query.OrderByDescending(x => x.ModifiedUtc);
                    break;
                case ContentsOrder.Published:
                    query = query.OrderByDescending(cr => cr.PublishedUtc);
                    break;
                case ContentsOrder.Created:
                    query = query.OrderByDescending(cr => cr.CreatedUtc);
                    break;
                default:
                    query = query.OrderByDescending(cr => cr.ModifiedUtc);
                    break;
            }

            //if (!String.IsNullOrWhiteSpace(model.Options.SelectedCulture))
            //{
            //    query = _cultureFilter.FilterCulture(query, model.Options.SelectedCulture);
            //}

            //if (model.Options.ContentsStatus == ContentsStatus.Owner)
            //{
            //    query = query.Where<CommonPartRecord>(cr => cr.OwnerId == Services.WorkContext.CurrentUser.Id);
            //}

            model.Options.SelectedFilter = model.TypeName;
            model.Options.FilterOptions = (await GetListableTypesAsync())
                .Select(ctd => new KeyValuePair<string, string>(ctd.Name, ctd.DisplayName))
                .ToList().OrderBy(kvp => kvp.Value);

            //model.Options.Cultures = _cultureManager.ListCultures();

            // Invoke any service that could alter the query
            await _contentAdminFilters.InvokeAsync(x => x.FilterAsync(query, model, pagerParameters, this), Logger);

            var maxPagedCount = siteSettings.MaxPagedCount;
            if (maxPagedCount > 0 && pager.PageSize > maxPagedCount)
                pager.PageSize = maxPagedCount;

            var pagerShape = (await New.Pager(pager)).TotalItemCount(maxPagedCount > 0 ? maxPagedCount : await query.CountAsync());
            var pageOfContentItems = await query.Skip(pager.GetStartIndex()).Take(pager.PageSize).ListAsync();

            var contentItemSummaries = new List<dynamic>();
            foreach (var contentItem in pageOfContentItems)
            {
                contentItemSummaries.Add(await _contentItemDisplayManager.BuildDisplayAsync(contentItem, this, "SummaryAdmin"));
            }

            var viewModel = (await New.ViewModel())
                .ContentItems(contentItemSummaries)
                .Pager(pagerShape)
                .Options(model.Options)
                .TypeDisplayName(model.TypeDisplayName ?? "")
                .TypeName(model.TypeName ?? "");
            return View(viewModel);
        }

        private async Task<IEnumerable<ContentTypeDefinition>> GetListableTypesAsync()
        {
            var listable = new List<ContentTypeDefinition>();
            foreach (var ctd in _contentDefinitionManager.ListTypeDefinitions())
            {
                if (ctd.Settings.ToObject<ContentTypeSettings>().Listable)
                {
                    var authorized = await _authorizationService.AuthorizeAsync(User, Permissions.NewManage, _contentManager.New(ctd.Name));
                    if (authorized)
                    {
                        listable.Add(ctd);
                    }
                }
            }
            return listable;
        }


        //新建模版函数
        public async void CreateIndexTemplate()
        {
            var MyTemplate = new TemplateViewModel { };
            MyTemplate.Name = "Content__Page";
            MyTemplate.Description = "A template for the  Page content type";
            MyTemplate.Content = " <script src=\"/OrchardCore.Resources/Scripts/jquery.min.js\" type=\"text/javascript\"></script>" +
"<script> " +
"  var NewTypeData;" +
"  $.ajax({        " +
"  async: false,       " +
"  type:\'get\',        " +
"  url:\"/New/NewsManage/Display/ReadTypeContents\",        " +
"  success:function(Content){            " +
"  NewTypeData = Content;" +
"  }    " +
"  });  " +
"  " +
"  function createShowingNewType(data) {" +
"  			var Str = \"<a href =\'\'><img src = \'/New/media/index_02.gif\' style =\'width:119px; height: 40px;float:left\'></a>\";  		" +
"  			for (var i = 0; i < data.length; i++) {                 " +
"         				Str = Str + \"<a href =\'/New/NewsManage/Display/TypeDisplayIndex?ContentType=\"+ data[i].NewPart.Name+" +
"                                    \"\'><img src = \'/New/media/\" +data[i].NewPart.Name+ \".gif\' style =\'width:120px; height: 40px;float:left\'></a>\";" +
"             }       " +
"             $(\"#NewType\").html(Str);    " +
"  }        " +
"   $(function() {" +
"    createShowingNewType(NewTypeData);" +
"    var Type4ImgStr =\" <img src= \'/New/media/\"+NewTypeData[4].NewPart.NewDescription+" +
"                     \".gif\' style = \'float:left;margin-left:1px;\' >\";" +
"    var Type0ImgStr =\" <img src= \'/New/media/\"+NewTypeData[0].NewPart.NewDescription+" +
"                     \".gif\' style = \'float:left;margin-left:1px;\' >\";" +
"    $(\"#Type4Img\").html(Type4ImgStr);" +
"    $(\"#Type0Img\").html(Type0ImgStr);                                       " +
"   });                   " +
"</script>" +
"  <center> <div  style =\"width:980px;\">  " +
"    <img src = \"/New/media/index_01.gif\" style =\"width:980px; height: 131px;\"> " +
"    <div id =\"NewType\">  </div> " +
"    <div style=\"width:980px;\"> 		" +
"      <div style=\"width:980px;height:40px;\">             	</div>" +
"      <div style=\"width:980px;height:40px;\">             	</div>  " +
"      <div style=\"width:360px;height:450px;float:left;border:1px solid #cdf5ed;\">" +
"          <script>" +
"              $.ajax({        " +
"  					type:\'get\',        " +
"  					url:\"/New/NewsManage/Display/TypeDisplay?ContentType=\"+NewTypeData[0].NewPart.Name,      " +
"  					success:function(Content){            " +
"            			ShowingNewTypeContent(Content);" +
"  					}    " +
"  					});  " +
"  			" +
"             function ShowingNewTypeContent(data) {" +
"  				var Str = \"\";" +
"  				for (var i =(data.length-1); i>-1;i--) { " +
"         			Str = Str +\" <li ><a href=\'/New/NewsManage/Display/NewPartDisplay?ContentType=\"+data[i].ContentType+" +
"                                        \"&ContentItemId=\"+data[i].ContentItemId+\"\' title=\'\"+data[i].TitlePart.Title+\"\'>\"+data[i].TitlePart.Title+" +
"                                        \"</a><span style=\'float:right;\'>\"+data[i].CreatedUtc.substring(0,10)+\"</span></li>\";" +
"             	} " +
"                 $(\"#ul0\").html(Str);" +
"              }  " +
"            " +
"          </script>" +
"          <div id=\"Type4Img\" style=\"width:100%;height:40px;\">" +
"          " +
"          </div >" +
"          <div style=\"width:100%;height:410px;\">" +
"             <table width=\"100%\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" class=\"box\">" +
"                      <tbody>" +
"                         <tr>" +
"                           <td>" +
"                             <ul id=\"ul0\"></ul>" +
"                           </td>" +
"                         </tr>" +
"                       </tbody>" +
"             </table>" +
"          </div>" +
"      </div>    	" +
"      <div style=\"width:620px;height:650px;float:left;border:1px solid #cdf5ed;\">" +
"       <div id=\"Type0Img\" style=\"width:100%;height:40px;\">" +
"          " +
"          </div >" +
"      </div>  " +
"    </div>  " +
"    </div>" +
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
    }
}
