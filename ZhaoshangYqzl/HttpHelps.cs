using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZhaoshangYqzl
{
    public class HttpHelps
    {
        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HttpPost(string url, string data)
        {
            string strResult = "";
            try
            {
                //.net core 需要注册GB2312
               // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                System.Net.ServicePointManager.DefaultConnectionLimit = 50;
                byte[] bytes = Encoding.GetEncoding("GBK").GetBytes(data);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentLength = bytes.Length;
                request.ContentType = " text/xml; charset=GBK";
                var cccc = data.Length;
                // request.UserAgent = "sdb client";
                request.Accept = "text/xml";
                request.Timeout = 60000;
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version11;
                Stream reqstream = request.GetRequestStream();
                reqstream.Write(bytes, 0, bytes.Length);
                //request.Timeout = 10000; 
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream streamReceive = response.GetResponseStream();
               // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                Encoding encoding = Encoding.GetEncoding("GBK");

                StreamReader streamReader = new StreamReader(streamReceive, encoding);
                strResult = streamReader.ReadToEnd();
                streamReceive.Dispose();
                streamReader.Dispose();
            }
            catch (Exception e)
            {
                return null;
            }
            finally
            {
            }
            return strResult;
        }
        public static string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=utf-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        /// <summary>
        /// GB2312转换成UTF8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string gb2312_utf8(string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //.net core 需要注册GB2312
           // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //gb2312   
            gb2312 = Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string utf8_gb2312(string text)
        {
            var xxx = System.Text.Encoding.GetEncodings();
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
           // System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            var ssss = gb2312.GetString(utf);
            //返回转换后的字符   
            byte[] buffer = Encoding.GetEncoding("GB2312").GetBytes(text);
            string strDest = Encoding.GetEncoding("GB2312").GetString(buffer);
            var turn = strDest;

            byte[] buffers1 = Encoding.GetEncoding("gb2312").GetBytes(text);
            buffers1 = System.Text.Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gb2312"), buffers1);
            string strDestss = Encoding.GetEncoding("utf-8").GetString(buffers1);

            byte[] buffers = utf8.GetBytes(strDestss);
            buffers = System.Text.Encoding.Convert(Encoding.GetEncoding("gb2312"), Encoding.UTF8, buffers);
            string strDests = Encoding.GetEncoding("gb2312").GetString(buffers);
            return strDests;

        }
        /// <summary>
        /// Unicode转中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string unicode_ch(string text)
        {
            return string.Format("{0:x4}", text);
        }
        /// <summary>
        /// 字符串转Unicode码
        /// </summary>
        /// <returns>The to unicode.</returns>
        /// <param name="value">Value.</param>
        public static string StringToUnicode(string value)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(value);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                // 取两个字符，每个字符都是右对齐。
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Unicode转字符串
        /// </summary>
        /// <returns>The to string.</returns>
        /// <param name="unicode">Unicode.</param>
        public static string UnicodeToString(string unicode)
        {
            string resultStr = "";
            string[] strList = unicode.Split('u');
            for (int i = 1; i < strList.Length; i++)
            {
                resultStr += (char)int.Parse(strList[i], System.Globalization.NumberStyles.HexNumber);
            }
            return resultStr;
        }
    }

}
