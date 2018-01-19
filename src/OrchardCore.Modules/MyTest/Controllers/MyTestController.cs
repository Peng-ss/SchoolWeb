using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using NewsManage.Common;
using NewsManage.Models;

namespace NewsManage.Controllers
{
    public  class MyTestController : Controller
    {
        public JsonResult Test2()
        {
            var aa = new MyTestPart();
            aa.id = 10;
            aa.Name = "aa";
            return this.Jsonp(aa);
        }

        public ActionResult Test1()
        {
            return View();
        }
    }
}
