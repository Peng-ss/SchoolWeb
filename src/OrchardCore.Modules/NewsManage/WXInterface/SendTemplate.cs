using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NewsManage.Models;
using Newtonsoft.Json;

namespace NewsManage.WXInterface
{
    public class SendTemplate
    {
        public static string SendSearchtemplate(string accesstoken,string content)
        {
            if (accesstoken !=null && content !=null)
            {
                var searchmodel = new Template();
                var data = new TemplateData();
                searchmodel.touser = "ob1Fj0hcxN-8lCouYYZb7CF-BBJs";
                searchmodel.template_id = "rNptNzNkbzunBF3zsukNfBC-DAwrUv8gnmjrcrrGKc4";
                searchmodel.url = "111.230.151.235/New";
                data.Add("first", new Item("使用了搜索功能", "#000000"));
                data.Add("keyword1", new Item(GetLocalIP(), "#FF4040"));
                data.Add("keyword2", new Item(content, "#FF4040"));
                data.Add("keyword3", new Item(DateTime.Now.ToString(), "#FF4040"));
                data.Add("remark", new Item("请留意！！", "#000000"));
                searchmodel.data = data;
                var str = JsonConvert.SerializeObject(searchmodel);
                var urlstr = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token="+ accesstoken;
                var jsonstr = Httpgetpost.Post(urlstr,str);
                return jsonstr;
            }
            return null;
        }
        public static string SendLogintemplate(string accesstoken,string name)
        {
            if (accesstoken != null  && name !=null)
            {
                var searchmodel = new Template();
                var data = new TemplateData();
                searchmodel.touser = "ob1Fj0hcxN-8lCouYYZb7CF-BBJs";
                searchmodel.template_id = "MkAXA32hZhVDk_lb2TNtvBNR1jqh58ZlG2aB-nBaM-Y";
                searchmodel.url = "111.230.151.235/New";
                data.Add("first", new Item("用户登录管理面板", "#000000"));
                data.Add("keyword1", new Item(GetLocalIP(), "#FF4040"));
                data.Add("keyword2", new Item(name, "#FF4040"));
                data.Add("keyword3", new Item(DateTime.Now.ToString(), "#FF4040"));
                data.Add("remark", new Item("请留意！！", "#000000"));
                searchmodel.data = data;
                var str = JsonConvert.SerializeObject(searchmodel);
                var urlstr = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + accesstoken;
                var jsonstr = Httpgetpost.Post(urlstr, str);
                return jsonstr;
            }
            return null;
        }


        public static string GetLocalIP()
        {
            try
            {
                string HostName = Dns.GetHostName(); //得到主机名
                var IpEntry = Dns.GetHostEntry(HostName);
                for (var i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
    }
}
