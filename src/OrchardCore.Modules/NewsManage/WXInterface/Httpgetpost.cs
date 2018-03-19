using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace NewsManage.WXInterface
{
    public class Httpgetpost
    {
        public static string accesstoken = "";
        public static string appid = "wxf9615400c4af6007";
        public static string appsecret = "abfdc0f96365d27e3ececb0d6a089ea2";
        
        public static string Get(string urlString)
        {
            //定义局部变量  
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebRespones = null;
            Stream stream = null;
            string htmlString = string.Empty;

            //请求页面  
            try
            {
                httpWebRequest = WebRequest.Create(urlString) as HttpWebRequest;
            }
            //处理异常  
            catch (Exception ex)
            {
                throw new Exception("建立页面请求时发生错误！", ex);
            }
            httpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; Maxthon 2.0)";
            //获取服务器的返回信息  
            try
            {
                httpWebRespones = (HttpWebResponse)httpWebRequest.GetResponse();
                stream = httpWebRespones.GetResponseStream();
            }
            //处理异常  
            catch (Exception ex)
            {
                throw new Exception("接受服务器返回页面时发生错误！", ex);
            }
            StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
            //读取返回页面  
            try
            {
                htmlString = streamReader.ReadToEnd();
            }
            //处理异常  
            catch (Exception ex)
            {
                throw new Exception("读取页面数据时发生错误！", ex);
            }
            //释放资源返回结果  
            streamReader.Close();
            stream.Close();
            return htmlString;
        }

        public static string Post(string url, string param)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }

    }
}
