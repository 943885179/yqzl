using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
    /// <summary>
    /// 交易记录查询明细请求参数
    /// </summary>
  public  class qryDtlResponse
    {
        /// <summary>
        /// AcctNo	账号	Char(20)	Y	
        /// </summary>
        public string AcctNo { get; set; }
        /// <summary>
        /// CcyCode	币种	Char(3)	Y	
        /// </summary>
        public string CcyCode { get; set; }
        /// <summary>
        ///EndFlag	数据结束标志	Char(1)	Y	“Y”---表示查询结果已全部输出完毕；
        ///“N”---表示查询结果只输出一部分，后续部分有待请求输出；
        /// </summary>
        public string EndFlag { get; set; }
        /// <summary>
        /// EndDate	结束日期	Char(8)	Y	
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// PageNo	查询页码	Char(6)	Y
        /// </summary>
        public int PageNo { get; set; }
        /// <summary>
        ///  PageRecCount 记录笔数    Char(2)
        /// </summary>
        public int PageRecCount { get; set; }
        public List<sbList> list { get; set; }
    }
    public class sbList
    {
        /// <summary>
        /// AcctDate	主机记账日期	Char(8)	
        /// </summary>
        public string AcctDate { get; set; }
        /// <summary>
        /// TxTime	交易时间	Char(6)
        /// </summary>
        public string TxTime { get; set; }
        /// <summary>
        /// HostTrace	主机流水号	Char(32)	
        /// </summary>
        public string HostTrace { get; set; }
        /// <summary>
        /// BussSeqNo	业务流水号	Char(32)
        /// </summary>
        public string BussSeqNo { get; set; }
        /// <summary>
        /// DetailSerialNo	明细序号	Num(19)		明细序号，原来和核心水号一起区分交易唯一性
        /// </summary>
        public string DetailSerialNo { get; set; }
        /// <summary>
        /// OutNode	付款方网点号	Char(9)
        /// </summary>
        public string OutNode { get; set; }
        /// <summary>
        /// OutBankNo	付款方联行号	Char(16)	
        /// </summary>
        public string OutBankNo { get; set; }
        /// <summary>
        /// OutBankName	付款行名称	Char(120)
        /// </summary>
        public string OutBankName { get; set; }
        /// <summary>
        /// OutAcctNo	付款方账号	Char(32)
        /// </summary>
        public string OutAcctNo { get; set; }
        /// <summary>
        /// OutAcctName	付款方户名	Char(120)
        /// </summary>
        public string OutAcctName { get; set; }
        /// <summary>
        /// CcyCode	结算币种	Char(3)	
        /// </summary>
        public string CcyCode { get; set; }
        /// <summary>
        /// TranAmount	交易金额	Char (15)
        /// </summary>
        public decimal TranAmount { get; set; }
        /// <summary>
        /// InNode	收款方网点号	Char(9)
        /// </summary>
        public string MyProperty { get; set; }
        /// <summary>
        /// InBankNo	收款方联行号	Char(16)
        /// </summary>
        public string InBankNo { get; set; }
        /// <summary>
        /// InBankName	收款方行名	Char(120)
        /// </summary>
        public string InBankName { get; set; }
        /// <summary>
        /// InAcctNo	收款方账号	Char(32)
        /// </summary>
        public string InAcctNo { get; set; }
        /// <summary>
        /// InAcctName	收款方户名	Char(120)	
        /// </summary>
        public string InAcctName { get; set; }
        /// <summary>
        /// DcFlag	借贷标志	Char(1)
        /// </summary>
        public string DcFlag { get; set; }
        /// <summary>
        /// AbstractStr	摘要，未翻译的摘要，如TRS	Char(120)	
        /// </summary>
        public string AbstractStr { get; set; }
        /// <summary>
        /// VoucherNo	凭证号	Char(20)	
        /// </summary>
        public string VoucherNo { get; set; }
        /// <summary>
        /// TranFee	手续费	Char (15)
        /// </summary>
        public string TranFee { get; set; }
        /// <summary>
        /// PostFee	邮电费	Char (15)	
        /// </summary>
        public string PostFee { get; set; }
        /// <summary>
        /// AcctBalance	账户余额	Char (15)
        /// </summary>
        public string AcctBalance { get; set; }
        /// <summary>
        /// Purpose	用途，附言	定长报文无；Char(300)
        /// </summary>
        public string Purpose { get; set; }
        /// <summary>
        /// AbstractStr_Desc	中文摘要，AbstractStr的中文翻译	定长报文无；Char(100)
        /// </summary>
        public string AbstractStr_Desc { get; set; }
        /// <summary>
        /// ProxyPayName	代理人户名	定长报文无；Char(100)
        /// </summary>
        public string ProxyPayName { get; set; }
        /// <summary>
        /// ProxyPayAcc	代理人账号	定长报文无；Char(100)
        /// </summary>
        public string ProxyPayAcc { get; set; }
        /// <summary>
        /// ProxyPayBankName	代理人银行名称	定长报文无；Char(100)
        /// </summary>
        public string ProxyPayBankName { get; set; }
        /// <summary>
        /// HostDate	主机日期	Char (8)
        /// </summary>
        public string HostDate { get; set; }
    }
}
 