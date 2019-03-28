using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduAI
{
    public class BaiduAiHelp
    {
        // 设置APPID/AK/SK
        private string APP_ID = "15575286";
        private string API_KEY = "5wK7lVIbne3nZOZf2G6KZKim";
        private string SECRET_KEY = "T99BpChKXoyVvyevcjOA5ME4QnE5OphH";

        public BaiduAiHelp()
        {
        }
        /// <summary>
        /// 图片自定义模板识别接口
        /// </summary>
        /// <param name="path">图片地址</param>
        /// <param name="templateSign">模板id：http://ai.baidu.com/iocr#/templatelist 创建</param>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JObject CustomDemo(string path,string templateSign= "e8f4c78e78cd41e4fa4fea6f8fafc11a")
        {
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = File.ReadAllBytes(path);
            // 调用自定义模板文字识别，可能会抛出网络等异常，请使用try/catch捕获
            try
            {
                return client.Custom(image, templateSign);
            }
            catch (Exception ex)
            {
                throw new Exception("自定义模板识别失败，网络故障..."+ex.ToString());
            }
        }
        /// <summary>
        /// 自定义模板
        /// </summary>
        public void CustomDemo()
        {
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = File.ReadAllBytes("C:/Program Files/CMB/FbSdk/Receipt/755915712110113_20180217-20180217_1133500014708.jpg");
            var templateSign = "e8f4c78e78cd41e4fa4fea6f8fafc11a";

            // 调用自定义模板文字识别，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.Custom(image, templateSign);
            Console.WriteLine(result);
        }
        /// <summary>
        /// 高精度识别
        /// </summary>
        public void AccurateBasicDemo()
        {
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = File.ReadAllBytes("C:/Program Files/CMB/FbSdk/Receipt/755915712110113_20180217-20180217_1133500014708.jpg");
            // 调用通用文字识别（高精度版），可能会抛出网络等异常，请使用try/catch捕获
            var result = client.AccurateBasic(image);
            Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
        {"detect_direction", "true"},
        {"probability", "true"}
    };
            // 带参数调用通用文字识别（高精度版）
            result = client.AccurateBasic(image, options);
            Console.WriteLine(result);
        }
        public void GeneralBasicDemo()
        {
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            var image = File.ReadAllBytes("C:/Program Files/CMB/FbSdk/Receipt/755915712110113_20180217-20180217_1133500014708.jpg");
            // 调用通用文字识别, 图片参数为本地图片，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GeneralBasic(image);
            Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
        {"language_type", "CHN_ENG"},
        {"detect_direction", "true"},
        {"detect_language", "true"},
        {"probability", "true"}
    };
            // 带参数调用通用文字识别, 图片参数为本地图片
            result = client.GeneralBasic(image, options);
            Console.WriteLine(result);
        }
        public void GeneralBasicUrlDemo()
        {
            var url = "http://img0.imgtn.bdimg.com/it/u=3440274309,261591393&fm=26&gp=0.jpg";
            var client = new Baidu.Aip.Ocr.Ocr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;  // 修改超时时间
            // 调用通用文字识别, 图片参数为远程url图片，可能会抛出网络等异常，请使用try/catch捕获
            var result = client.GeneralBasicUrl(url);
            Console.WriteLine(result);
            // 如果有可选参数
            var options = new Dictionary<string, object>{
                {"language_type", "CHN_ENG"},
                {"detect_direction", "true"},
                {"detect_language", "true"},
                {"probability", "true"}
             };
            // 带参数调用通用文字识别, 图片参数为远程url图片
            result = client.GeneralBasicUrl(url, options);
            Console.WriteLine(result);
        }
    }
}
