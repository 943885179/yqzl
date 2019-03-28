using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace Log
{
    public class Log4Helper
    {
      //  log4net.Config.XmlConfigurator=new System.IO.FileInfo("log4net.config");  
        /*private static ILog log = LogManager.GetLogger(typeof(Log4Helper));*/
        /*[STAThread]
        static void Main(string[] args)
        {
            log.Info("info");
            log.Debug("Debug");
            log.Error("Error");
        }*/
              /// <summary>      
              /// 输出日志到Log4Net      
              /// /// </summary>        
              /// /// <param name="t"></param>  
              /// /// <param name="ex"></param>      
        #region
        public static void WriteLog(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error", ex);
        }
        #endregion
        /// <summary>        
        /// /// 输出日志到Log4Net   
        /// /// </summary>       
        /// /// <param name="t"></param> 
        /// /// <param name="msg"></param>   
        #region 
        public static void WriteLog(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error(msg);
        }
        #endregion
        #region 
        public static void Info(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Info(msg);
        }
        #endregion
        #region 
        public static void Warn(Type t, string msg)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Warn(msg);
        }
#endregion
    }
}
