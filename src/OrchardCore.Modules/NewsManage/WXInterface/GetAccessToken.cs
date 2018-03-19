using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

namespace NewsManage.WXInterface
{
   public class GetAccessToken
    {
        public static string Getaccesstoken ()
        {
            if (Httpgetpost.appsecret != null || Httpgetpost.appid != null)
            {
                var urlstr = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid="
                     + Httpgetpost.appid + "&secret=" + Httpgetpost.appsecret;
                var jsonstr = JObject.Parse(Httpgetpost.Get(urlstr));
                Debug.WriteLine(jsonstr);
                try
                {
                    Httpgetpost.accesstoken = jsonstr["access_token"].ToString();
                    return "success";
                }
                catch
                {
                    var str = jsonstr["errcode"].ToString()+ jsonstr["errmsg"].ToString();
                    return str;
                    //throw new Exception("获取不了accesstoken", ex);
                }
            }
            return null;
        }
    }
}
