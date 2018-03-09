using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using OrchardCore.Title.Model;
using YesSql;
using YesSql.Services;

namespace NewsManage.Controllers
{
    public class AdminController : Controller, IUpdateModel
    {

        private readonly IAuthorizationService _authorizationService;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly ISession _session;
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionDisplayManager _contentDefinitionDisplayManager;
        private readonly ISiteService _siteService;
        private readonly IEnumerable<IContentAdminFilter> _contentAdminFilters;
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        public dynamic New { get; set; }
        public ILogger Logger { get; set; }

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
            IContentDefinitionManager contentDefinitionManager
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
        }

        public ActionResult NewManage()
        {
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
                DevicesInfoContentItem.Alter<NewPart>(x => x.Classify = item.Classify);
                item.Name = "ContentType_" + item.NewDisplayName;
                if (_contentDefinitionManager.GetTypeDefinition(item.Name) == null)
                {
                    if (item.Classify == false)
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
                    }
                    else
                    {
                        var BodyPartSetting = new BodyPartSettings { };
                        BodyPartSetting.Editor = "Wysiwyg";
                        _contentDefinitionManager.AlterTypeDefinition(item.Name, bulid => bulid
                        .DisplayedAs(item.NewDisplayName)
                        .Draftable()
                        .Creatable()
                        .Listable()
                        .WithPart("TitlePart", part => part.WithPosition("2"))
                        .WithPart("BodyPart", part => part.WithSettings(BodyPartSetting).WithPosition("3"))
                        .WithPart("TypeNewClassifyPart", part => part.WithPosition("1"))
                        );

                    }

                }
                DevicesInfoContentItem.Alter<TitlePart>(x => x.Title = item.NewDisplayName);
                DevicesInfoContentItem.Alter<NewPart>(x => x.Name = item.Name);
                _contentDefinitionManager.GetTypeDefinition(item.Name);
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
                    Classify = m.As<NewPart>().Classify
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
                DevicesInfoContentItem.Alter<NewPart>(x =>x.Classify = item.Classify);
                item.Name = "ContentType_" + item.NewDisplayName;
                if (_contentDefinitionManager.GetTypeDefinition(item.Name) == null) { }
                else
                {
                    if (item.Classify == false)
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
                    else
                    {
                        var BodyPartSetting = new BodyPartSettings { };
                        BodyPartSetting.Editor = "Wysiwyg";
                        _contentDefinitionManager.AlterTypeDefinition(item.Name, bulid => bulid
                        .DisplayedAs(item.NewDisplayName)
                        .Draftable()
                        .Creatable()
                        .Listable()
                        .WithPart("TitlePart",part => part.WithPosition("2"))
                        .WithPart("BodyPart", part => part.WithSettings(BodyPartSetting).WithPosition("3"))
                        .WithPart("TypeNewClassifyPart", part => part.WithPosition("1"))
                        );
                    }
                }
                DevicesInfoContentItem.Alter<NewPart>(x => x.Name = item.Name);
                DevicesInfoContentItem.Alter<TitlePart>(x => x.Title = item.NewDisplayName);
                _contentDefinitionManager.GetTypeDefinition(item.Name);
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
                var contentTypeDefinition = _contentDefinitionManager.GetLastTypeDefinition(model.TypeName);
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


        
    }
}
