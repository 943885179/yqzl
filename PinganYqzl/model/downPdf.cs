using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
    /// <summary>
    /// PDF回单下载
    /// </summary>
    public class downPdf
    {
        /// <summary>
        /// FileName	文件名称	C(30)	必输
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// FilePath	文件路径	C(100)
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// RandomPwd	随机密码	C(200)	必输
        /// </summary>
        public string RandomPwd { get; set; }
        /// <summary>
        /// SignData	签名值	C(3000)	
        /// </summary>
        public string SignData { get; set; }
        /// <summary>
        /// HashData	SHA-1摘要	C(3000)	
        /// </summary>
        public string HashData { get; set; }
    }
    public class result {
        public List<downPdf> list { get; set; }
    }
}
