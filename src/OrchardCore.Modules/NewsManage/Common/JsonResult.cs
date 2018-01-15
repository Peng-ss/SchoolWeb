using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NewsManage.Common
{
    public class JsonpResult : JsonResult
    {
        public JsonpResult(string callbackName) : base(callbackName)
        {
            CallbackName = callbackName;
        }

        public string CallbackName { get; set; }



        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;
            var val = context.HttpContext.Request.QueryString.Value;
            var value1 = val.IndexOf('&');
            var value2 = val.Length;
            var value22 = value2 - value1;
            var value3 = val.Remove(value1, value22);
            var value4 = value3.IndexOf('=');
            var value5 = value3.Remove(0, value4 + 1);
            string jsoncallback = CallbackName ?? value5 ;

            if (!string.IsNullOrEmpty(jsoncallback))
            {
                if (string.IsNullOrEmpty(base.ContentType))
                {
                    base.ContentType = "application/x-javascript";
                }
                await response.WriteAsync(string.Format("{0}(",jsoncallback));

            }

            await response.WriteAsync(string.Format("{0}", JsonConvert.SerializeObject(this.Value)));

            if (!string.IsNullOrEmpty(jsoncallback))
            {
                await response.WriteAsync(")");
            }

        }




    }
    public static class ControllerExtensions
    {
        public static JsonpResult Jsonp(this Controller controller, object value, string callbackName =null)
        {
            return new JsonpResult(callbackName)
            {
                Value = value,

                //SerializerSettings = JsonSerializerSettings.AllowGet
            };
        }

        public static T DeserializeObject<T>(this Controller controller, string key="") where T : class
        {
            var value = controller.HttpContext.Request.QueryString.Value;
            
            var value1 = value.IndexOf('&');
            var value2 = value.Remove(0, value1);
            var value3 = value2.IndexOf('=');
            var value4 = value2.Remove(0, value3 + 1);
            var value5 = value4.LastIndexOf('&');
            var value6 = value4.Length;
            var value7 = value6 - value5;
            var value8 = value4.Remove(value5, value7);
            var value9 = WebUtility.UrlDecode(value8);
            if (string.IsNullOrEmpty(value9))
            {
                return null;
            }
            // JsonSerializerSettings javaScriptSerializer = new JsonSerializerSettings();
            return JsonConvert.DeserializeObject<T>(value9);
        }

    }

}



