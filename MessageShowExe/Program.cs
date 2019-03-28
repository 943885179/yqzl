using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MessageShowExe
{
    class Program
    {
        /*
        public static void SendSMSSingleByYunYongForCoder(string telephone, string Message)
        {
            string Url = "http://101.201.238.246/MessageTransferWebAppJs/servlet/messageTransferServiceServletByXml";
            Url += "?cmd=sendMessage&userName=jifahy&passWord=888888&serviceCode=076&mobilePhone=" + telephone + "&body=" + Message;
            SendRequest(Url, Encoding.UTF8);
        }*/
        /// <summary>     
        /// Get方式获取url地址输出内容     
        /// </summary> /// <param name="url">url</param>     
        /// <param name="encoding">返回内容编码方式，例如：Encoding.UTF8</param>     
        public static String SendRequest(String url, Encoding encoding)
        {
            HttpWebRequest webRequest = (System.Net.HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), encoding);
            return sr.ReadToEnd();
        }
        public static string Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            string oldMsg = "";
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                oldMsg += line.ToString() + "\n";
            }
            return oldMsg;
        }
        public static void Write(string msg)
        {
            string time = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds.ToString();
            string path = System.IO.Directory.GetCurrentDirectory().Replace("bin\\Debug", "txt\\短信通知记录.txt");
            FileStream fs = new FileStream(path, FileMode.Create);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes(msg);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }
        /// <summary>
        /// 追加数据到txt
        /// </summary>
        /// <param name="msg"></param>
        public static void writes(string msg)
        {
            string path = System.IO.Directory.GetCurrentDirectory().Replace("bin\\Debug", "txt\\短信通知记录.txt");
            System.IO.File.AppendAllText(path, msg);
        }
        static void Main(string[] args)
        {
            //获取数据
            var sqlServer = "Data Source=192.168.5.3;DataBase=OAtoU8DATA;Persist Security Info=True;UID=sa;PWD=mfsj1908oa**";
            string sql = "select * from  MessageShow where isOut=0 and  phone is not null and phone!='';";
            //读取需要发送的短信
            DataTable dt = SqlHelper.ExecuteDataTable(sqlServer, sql);
            foreach (DataRow row in dt.Rows)
            {
                string msg = row["msg"].ToString();
                string phone = row["phone"].ToString();
                string id = row["id"].ToString();
                var isOut = NoteHelper.SendSMSSingleByYunYongForCoder(phone, msg);
                if (isOut)
                {
                    //短信通知接口调用后记录调用次数
                   // writes(msg + "------------" + phone + "---------" + DateTime.Now + "\r\n");
                    //读取完毕后删除该记录
                    string sql1 = "update MessageShow set isOut=1 where id=" + id;
                    SqlHelper.ExecuteNonQuery(sqlServer, sql1);
                }
                
            }
        }

    }
}
