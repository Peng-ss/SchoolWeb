using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fluid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using NewsManage.Models;
using NewsManage.ViewModels;
using NewsManage.WXInterface;
using Newtonsoft.Json;
using OrchardCore.Admin;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.Liquid;
using OrchardCore.Mvc.ActionConstraints;
using OrchardCore.Queries.Sql;
using OrchardCore.Queries.Sql.ViewModels;
using YesSql;

namespace NewsManage.Controllers
{
    public class DisplayController : Controller, IUpdateModel
    {
        private readonly ISession _session;
        private readonly IAuthorizationService _authorizationService;
        private readonly IStore _store;
        private readonly ILiquidTemplateManager _liquidTemplateManager;
        private readonly IStringLocalizer<DisplayController> _stringLocalizer;
       

        public DisplayController(
            IAuthorizationService authorizationService, 
            ILiquidTemplateManager liquidTemplateManager, 
            IStore store, 
            ISession session,
            IStringLocalizer<DisplayController> stringLocalizer)
        {
            _session = session;
            _authorizationService = authorizationService;
            _liquidTemplateManager = liquidTemplateManager;
            _store = store;
            _stringLocalizer = stringLocalizer;
        }

        public ActionResult TypeDisplayIndex()
        {
            return View();
        }

        public ActionResult NewPartDisplay()
        {
            return View();
        }

        //利用ContentItemId获取一条新闻信息
        public JsonResult ReadContent(string ContentItemId)
        {
            var NewPartData = _session.Query<ContentItem>().ListAsync().Result
                   .Where(x => x.ContentItemId == ContentItemId && x.Latest);
            return Json(NewPartData);
        }

        public JsonResult ReadTypeContents()
        {
            var list = _session.Query<ContentItem>().ListAsync().Result
                .Where(x => x.ContentType == "新闻组管理" && x.Latest);
            return Json(list);
        }

        //利用ContentType获取该类型的所有发布的新闻信息
        public ActionResult TypeDisplay(string ContentType)
        {
            var NewPartData = _session.Query<ContentItem>().ListAsync().Result
                  .Where(x => x.ContentType == ContentType && x.Latest)
                  .Where(x => x.Published == true);//筛选出未发布的
            return Json(NewPartData);
        }


        public async Task<IActionResult> SearchIndex()
        {
            var select = Request.Query["select"];
            var content = Request.Query["content"];
            var model = new AdminQueryViewModel
            {
                DecodedQuery = "SELECT * FROM ContentItemIndex",
                Parameters = "{}"
            };
            if (select == "all")
            {
                model.DecodedQuery = "SELECT * FROM Document where " +
                                     "Type=\'OrchardCore.ContentManagement.ContentItem, OrchardCore.ContentManagement.Abstractions\'" +
                                     "and Content  LIKE \'%Published\":true%\'" +
                                     "and Content  LIKE \'%ContentType_%\' " +
                                     "and Content  LIKE \'%Title%\' " +
                                     "and Content  LIKE \'%Body%\'" +
                                     "and Content  LIKE \'%" +
                                     content +
                                     "%\'";


            }
            if (select == "zhengwen")
            {

            }
            if (select == "biaoti")
            {

            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var connection = _store.Configuration.ConnectionFactory.CreateConnection();
            var dialect = SqlDialectFactory.For(connection);

            var parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(model.Parameters);
            var templateContext = new TemplateContext();
            foreach (var parameter in parameters)
            {
                templateContext.SetValue(parameter.Key, parameter.Value);
            }

            var tokenizedQuery = await _liquidTemplateManager.RenderAsync(model.DecodedQuery, templateContext);

            if (SqlParser.TryParse(tokenizedQuery, dialect, _store.Configuration.TablePrefix, out var rawQuery, out var rawParameters, out var messages))
            {
                model.RawSql = rawQuery;
                model.RawParameters = JsonConvert.SerializeObject(rawParameters);

                try
                {
                    using (connection)
                    {
                        connection.Open();
                        model.Documents = await connection.QueryAsync(rawQuery, rawParameters);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", _stringLocalizer["An error occured while executing the SQL query: {0}", e.Message]);
                }
            }
            else
            {
                foreach (var message in messages)
                {
                    ModelState.AddModelError("", message);
                }
            }

            model.Elapsed = stopwatch.Elapsed;
            //var list = new List<String>();
            //foreach (var item in model.Documents)
            //{
            //    var serializer = new JsonSerializer();
            //    var str = JsonConvert.SerializeObject(item);
            //    var sr1 = new StringReader(str);
            //    var o1 = serializer.Deserialize(new JsonTextReader(sr1), typeof(NewContent));
            //    var newContent = o1 as NewContent;
            //    var a = newContent.Content;
            //    list.Add(newContent.Content);
            //}

            SendTemplate.SendSearchtemplate(Httpgetpost.accesstoken,content);
            return View(model);
        }

        

    }

}
