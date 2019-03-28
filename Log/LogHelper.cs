using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log
{
    public class LogHelper
    {
        /// <summary>
        /// 记录日志自定义文件夹
        /// </summary>
        private static string LogPath = string.Empty; public static void WriteLog(string log,string paths)
        {
            StreamWriter stream;
            //写入日志内容
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\"+paths;
            //检查上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            stream = new StreamWriter(path + "\\" + DateTime.Now.ToString("D") + ".log", true, Encoding.Default);
            stream.Write(DateTime.Now.ToString() + ":" + log);
            stream.Write("\r\n");
            stream.Flush();
            stream.Close();
        }
        public static void WriteLog(string log)
        {
            StreamWriter stream;
            //写入日志内容
            string path = AppDomain.CurrentDomain.BaseDirectory+"\\Log";
            //检查上传的物理路径是否存在，不存在则创建
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            stream = new StreamWriter(path + "\\"+DateTime.Now.ToString("D")+".log", true, Encoding.Default);
            stream.Write(DateTime.Now.ToString() + ":" + log);
            stream.Write("\r\n");
            stream.Flush();
            stream.Close();

            /*
            if(string.IsNullOrEmpty(LogPath))
            {
               // LogPath = Process.GetCurrentProcess().MainModule.FileName;//获得程序运行完整目录
                //LogPath = LogPath.Replace(Process.GetCurrentProcess().MainModule.ModuleName, "");//去除程序名
                LogPath = "E:\\" + "LOG";//拼接获得程序日志目录
                LogPath = System.AppDomain.CurrentDomain.BaseDirectory+"Log";
                if (Directory.Exists(LogPath) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(LogPath);
                }
            }
            string logFilePath = LogPath + "\\" + DateTime.Now.Date.ToString("D") + ".log";
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath);//创建该文件
            }
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    StreamWriter logFileWriter = new StreamWriter(logFilePath, true);
                    logFileWriter.WriteLine(log);
                    logFileWriter.Flush();
                    logFileWriter.Close();
                    break;
                }
                catch(Exception ex)
                {
                    if ((i + 1) < 10)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            */
        }
    }
}
