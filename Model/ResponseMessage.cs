using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model;

namespace Model
{
   public  class ResponseMessage
    {
        /// <summary>
        /// 状态start 0成功，1失败
        /// </summary>
        public int start { get; set; }
        /// <summary>
        /// msg 返回消息内容
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 其他消息(短信验证码等)
        /// </summary>
        public string other { get; set; }
        /// <summary>
        /// 返回成功信息
        /// </summary>
        public string sucess { get; set; }
        /// <summary>
        /// 返回错误信息
        /// </summary>
        public string errorMsg { get; set; }
        /// <summary>
        /// u8做单返回实体
        /// </summary>
        public ResultListModel model { get; set; }
    }
}
