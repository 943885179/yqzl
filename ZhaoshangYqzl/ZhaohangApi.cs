
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using dal;
using Log;
using Newtonsoft.Json;
using ZhaoshangYqzl.model;

namespace ZhaoshangYqzl
{
    public class ZhaohangApi
    {
        private static ZhaohangApi _ZhaohangApi = null;
        private static object Singleton_Lock = new object(); //锁同步
        public ZhaohangApi CreateInstance() {
            lock (Singleton_Lock)
            {
                if (_ZhaohangApi == null)
                {
                    _ZhaohangApi = new ZhaohangApi();
                }
            }
            return _ZhaohangApi;
        }
        private static string url = ConfigurationManager.AppSettings["ZhaohangUrl"].ToString();//"http://127.0.0.1:8080";//配置地址
        /// <summary>
        ///  3.6直接支付DCPAYMNT
        /// </summary>
        public void zhaoHangApi(CMBSDKPGK cmbc,out string request,out string result, out ResonseClass response)
        {
            request = XmlSerializeHelper.XmlSerialize<CMBSDKPGK>(cmbc);
            result = HttpHelps.HttpPost(url, request);
            LogHelper.WriteLog("请求：请求参数：" + request + "\n返回参数：\n" + result, "yqzl_zhaohang");
            // return result;
            //参数装json再转实体
            if (result != null)
            {

                result = result.Replace("<?xml version=\"1.0\" encoding=\"GBK\"?>", "");//消除xml头部
                result = result.Replace("<![CDATA[", "").Replace("&C]]>", "").Replace("&", "").Replace("]]>", "");//将一些非法字符消除
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                string jsontext = JsonConvert.SerializeXmlNode(doc);
                if (jsontext.Contains("\"NTSTLLSTZ\":{"))
                {//非数组改为数组
                    jsontext = jsontext.Replace("\"NTSTLLSTZ\":{", "\"NTSTLLSTZ\":[{");
                    jsontext = jsontext.Substring(0, jsontext.Length - 3) + "}]}}";
                }
                if (jsontext.Contains("\"NTQTSINFZ\":{"))
                {//非数组改为数组
                    jsontext = jsontext.Replace("\"NTQTSINFZ\":{", "\"NTQTSINFZ\":[{");
                    jsontext = jsontext.Substring(0, jsontext.Length - 3) + "}]}}";
                }
                if (jsontext.Contains("\"NTECKUSRZ\":{"))
                {//非数组改为数组
                    jsontext = jsontext.Replace("\"NTECKUSRZ\":{", "\"NTECKUSRZ\":[{");
                    jsontext = jsontext.Substring(0, jsontext.Length - 3) + "}]}}";
                }
                if (jsontext.Contains("\"NTECKTQYZ\":{"))
                {//非数组改为数组
                    jsontext = jsontext.Replace("\"NTECKTQYZ\":{", "\"NTECKTQYZ\":[{");
                    jsontext = jsontext.Substring(0, jsontext.Length - 3) + "}]}}";
                }
               response = JsonConvert.DeserializeObject<ResonseClass>(jsontext);
            }
            else
            {
                result = "";
                response = new ResonseClass();
            }
        }
        /// <summary>
        ///  返回请求参数
        /// </summary>
        public string  zhaoHangApi_requestData(CMBSDKPGK cmbc)
        {
            return XmlSerializeHelper.XmlSerialize<CMBSDKPGK>(cmbc);
        }
        /// <summary>
        ///  通过参数直接支付DCPAYMNT
        /// </summary>
        public void zhaoHangApi_pay(string request, out string result, out ResonseClass response)
        {
            result = HttpHelps.HttpPost(url, request);
            LogHelper.WriteLog("请求：请求参数：" + request + "\n返回参数：\n" + result, "yqzl_zhaohang");
            // return result;
            //参数装json再转实体
            result = result == null ? "" : result.Replace("<?xml version=\"1.0\" encoding=\"GBK\"?>", "");//消除xml头部
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(result);
            string jsontext = JsonConvert.SerializeXmlNode(doc);
            response = JsonConvert.DeserializeObject<ResonseClass>(jsontext);
        }
    }

}
