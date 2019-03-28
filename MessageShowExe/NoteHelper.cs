using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MessageShowExe
{

    /// <summary>
    /// 手机短信帮助类
    /// </summary>
    public static class NoteHelper
    {
        private const string softwareSerialNo = "8SDK-EMY-6699-SETPS";
        private const string key = "626584";

        /// <summary>
        /// 云融通知短信
        /// </summary>
        /// <param name="telephone">手机号</param>
        /// <param name="Message">短信内容</param>
        /// <returns></returns> 
        public static bool SendSMSSingleByYunYongForCoder(string telephone, string Message)
        {
            string username = "jifahy";
            string pwd = "rE3iX2sV";
            string url = "http://47.98.61.138:9001/smsSend.do";
            string md5_pwd = (username + pwd.To32Md5("x2")).To32Md5("x2");
            string param = "username=" + username + "&password=" + md5_pwd + "&mobile=" + telephone + "&content=" + Message;
            return RequestUrlByPOST(url, param, "application/x-www-form-urlencoded");


            //string Url = "http://101.201.238.246/MessageTransferWebAppJs/servlet/messageTransferServiceServletByXml";
            //Url += "?cmd=sendMessage&userName=jifahy&passWord=888888&serviceCode=076&mobilePhone=" + telephone + "&body=" + Message;
            //string result = RequestHelper.RequestUrlByPOST(Url);
            //string FinalResult = Regex.Match(result, "\"resultCode\">(.*?)</field><field name=\"errorCode\"></field></body></message>").Groups[1].Value;
            //if ("0".Equals(FinalResult))
            //    return true;
            //else
            //{
            //    // 写日志(数据库)
            //    //new ErrorHelper().WriteErrorToDataBase(new ShowException("短信发送失败", "接收手机号：" + telephone + "， 发送内容：" + Message + "， 返回信息：" + FinalResult));
            //    return false;
            //}
        }

        /// <summary>
        /// POST方式调用接口
        /// </summary>
        /// <param name="Url">要请求Url</param>
        /// <returns></returns>
        public static bool RequestUrlByPOST(string Url, string codestr = "UTF-8", string ConType = "text/xml")
        {
            string strHtml = "";
            try
            {
                HttpWebRequest wr;
                System.GC.Collect();
                wr = (HttpWebRequest)WebRequest.Create(Url);
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] bytes = encoding.GetBytes(codestr);
                wr.Method = "POST";
                wr.Timeout = Int32.MaxValue;
                wr.Credentials = CredentialCache.DefaultCredentials;
                wr.ContentType = ConType;
                wr.ContentLength = bytes.Length;
                wr.ServicePoint.Expect100Continue = false;
                using (Stream requestStream = wr.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                using (HttpWebResponse response = (HttpWebResponse)wr.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK && wr.HaveResponse)
                    {
                        if (response != null)
                        {
                            using (Stream stream = response.GetResponseStream())//获取返回的字符流格式
                            {
                                using (StreamReader sr = new StreamReader(stream, System.Text.Encoding.UTF8))//解决乱码：设置utf-8字符格式
                                {
                                    if (sr != null)
                                    {
                                        strHtml = sr.ReadToEnd();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
                //throw new Exception(ex.Message);
                return false;
            }
            //return strHtml;
            return true;
        }


        public static string To32Md5(this string str, string code = "X")
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 

                pwd = pwd + s[i].ToString(code);

            }
            return pwd;
        }
    }

}
