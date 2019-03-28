using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
    /// <summary>
    /// 银联号Model
    /// </summary>
   public class YlhModel
    {
        public Request Request { get; set; }
        public YlhModelResponse Response { get; set; }
    }
    public class Request {
        /// <summary>
        /// BankNo       	银行代码	Char(14)	Y
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// BankName	银行名称	Char(30)	N
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// KeyWord	网点名称关键字	Char(30)	Y
        /// </summary>
        public string KeyWord { get; set; }
    }

    public class YlhModelResponse
    {
        /// <summary>
        /// BankNo       	银行代码	Char(14)	Y	
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// BankName	银行名称	Char(30)	N	
        /// </summary>
        public string BankName { get; set; }
        /// <summary>
        /// size	返回的记录数	9(3)	Y
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// list
        /// </summary>
        public List<YHHList> list { get; set; }
    }

    public class YHHList {
        /// <summary>
        /// NodeName	网点名称	Char(30)	Y
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// NodeCode	网点联行号	Char(14)	Y	
        /// </summary>
        public string NodeCode { get; set; }
      
    }
}
