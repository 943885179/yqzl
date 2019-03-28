using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
    /// <summary>
    /// 平安银行付款请求实体
    /// </summary>
    public class XferRequestModel
    {
        /// <summary>
        /// 转账凭证号C(20)，最少10位长度必输	标示交易唯一性，同一客户上送的不可重复，建议格式：yyyymmddHHSS+8位系列要求6个月内唯一。
        /// </summary>
        public string ThirdVoucher { get; set; } //=DateTime.Now.ToString("yyyymmddHHSS")+ new Random().Next(10000000,100000000);// { get; set; }
        /// <summary>
        /// 客户自定义凭证号	C(20)	非必输	用于客户转账登记和内部识别，通过转账结果查询可以返回。银行不检查唯一性
        /// </summary>
        public string CstInnerFlowNo { get; set; }
        /// <summary>
        /// CcyCode	货币类型	C(3)	必输	RMB-人民币
        /// </summary>
        public string CcyCode { get; set; }
        /* get
         {
             return CcyCode;
         }
         set
         {
             if (string.IsNullOrEmpty(value)) {
                 value = "RMB";
             }
             CcyCode = value;
         }
     }*/
        /// <summary>
        /// OutAcctNo	付款人账户	C(20)	必输	扣款账户
        /// </summary>
        public string OutAcctNo { get; set; }
        /// <summary>
        /// OutAcctName	付款人名称	C(60)	必输	付款账户户名
        /// </summary>
        public string OutAcctName { get; set; }
        /// <summary>
        /// OutAcctAddr	付款人地址	C(60)	非必输	建议填写付款账户的分行、网点名称
        /// </summary>
        public string OutAcctAddr { get; set; }
        /// <summary>
        /// InAcctBankNode	收款人开户行行号	C(12)	非必输	跨行转账建议必输。为人行登记在册的商业银行号，若输入则长度必须在4 ~12位之间；
        /// </summary>
        public string InAcctBankNode { get; set; }
        /// <summary>
        /// InAcctRecCode	接收行行号	C(12)	非必输	建议同收款人开户行行号
        /// </summary>
        public string InAcctRecCode { get; set; }
        /// <summary>
        /// InAcctNo	收款人账户	C(32)	必输	
        /// </summary>
        public string InAcctNo { get; set; }
        /// <summary>
        /// InAcctName	收款人账户户名	C(60)	必输	
        /// </summary>
        public string InAcctName { get; set; }
        /// <summary>
        /// InAcctBankName	收款人开户行名称	C(60)	必输	建议格式：xxx银行
        /// </summary>
        public string InAcctBankName { get; set; }
        /// <summary>
        /// InAcctProvinceCode	收款账户银行开户省代码或省名称	C(10)	非必输	建议跨行转账输入；对照码参考“附录-省对照表”；也可输入“附录-省对照表”中的省名称。
        /// </summary>
        public string InAcctProvinceCode { get; set; }
        /// <summary>
        /// InAcctCityName	收款账户开户市	C(12)	非必输	建议跨行转账输入； 
        /// </summary>
        public string InAcctCityName { get; set; }
        /// <summary>
        /// TranAmount	转出金额	C(13,2)	必输	如为XML报文，则直接输入输出以元为单位的浮点数值，如2.50 (两元五角)
        /// </summary>
        public decimal TranAmount { get; set; }
        /// <summary>
        /// UseEx 资金用途	C(100)	非必输	100个汉字，对方能否看到此用途视收款方银行的支持。
        /// </summary>
        public string UseEx { get; set; }
        /// <summary>
        /// UnionFlag	行内跨行标志	C(1)	必输	1：行内转账，0：跨行转账
        /// </summary>
        public string UnionFlag { get; set; }
        /// <summary>
        /// SysFlag	转账加急标志	C(1) 非必输 N：普通（大小额自动选择），默认值；Y：加急 （大额）；S：特急(超级网银)；T1：深圳同城普通；T2：深圳同城实时；默认为N
        ///  STLCHN 结算方式代码C(1)N：普通F：快速 否只对跨行交易有效
        /// </summary>
        public string SysFlag { get; set; }
        /// <summary>
        /// AddrFlag	同城/异地标志	C(1) 必输	“1”—同城   “2”—异地；若无法区分，可默认送1-同城。
        /// </summary>
        public int AddrFlag { get; set; }
        /// <summary>
        /// MainAcctNo	付款虚子账户	C(32)	非必须	必须签约了银行现金管理合约才能使用此域；用于现金管理代理结算（不同与代理行支付功能）：填写虚子账号。虚子账户代理主账户付款。
        /// </summary>
        public string MainAcctNo { get; set; }
        /// <summary>
        /// InIDType	收款人证件类型	C(2)	非必输	参考附录-证件号码对照表上送则验证证件号码是否一致，只对行内个人借记卡收款账户有效（不支持信用卡）
        /// </summary>
        public string InIDType { get; set; }
        /// <summary>
        /// InIDNo	收款人证件号码	C(30)	非必输	上送则验证证件号码是否一致
        /// </summary>
        public string InIDNo { get; set; }
        /// <summary>
        /// 付款人
        /// </summary>
        public string createMan { get; set; }
    }
   
}
