using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using YesSql;

namespace NewsManage.Controllers
{
    public class DisplayController : Controller, IUpdateModel
    {
        private readonly ISession _session;

        public DisplayController(ISession session)
        {
            _session = session;
            
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
    }
}
