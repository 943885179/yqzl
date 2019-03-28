using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
    /// <summary>
    /// 平安返回接口
    /// </summary>
   public class PinganResponse<T>
    {
        /// <summary>
        /// 返回编码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回内容
        /// </summary>
        public T result{ get; set; }
    }
}
