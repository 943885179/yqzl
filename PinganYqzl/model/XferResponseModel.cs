using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
   public class XferResponseModel
    {

        /// <summary>
        /// 转账凭证号C(20)，最少10位长度必输	标示交易唯一性，同一客户上送的不可重复，建议格式：yyyymmddHHSS+8位系列要求6个月内唯一。
        /// </summary>
        public string ThirdVoucher { get; set; } //=DateTime.Now.ToString("yyyymmddHHSS")+ new Random().Next(10000000,100000000);// { get; set; }
        /// <summary>
        /// FrontLogNo	银行流水号	C(32)	必输	银行业务流水号；可以用于对账
        /// </summary>
        public string FrontLogNo { get; set; }
        /// <summary>
        /// 客户自定义凭证号	C(20)	非必输	用于客户转账登记和内部识别，通过转账结果查询可以返回。银行不检查唯一性
        /// </summary>
        public string CstInnerFlowNo { get; set; }
        /// <summary>
        /// CcyCode	货币类型	C(3)	必输	RMB-人民币
        /// </summary>
        public string CcyCode { get; set; }
        /// <summary>
        /// OutAcctNo	付款人账户	C(20)	必输	扣款账户
        /// </summary>
        public string OutAcctNo { get; set; }
        /// <summary>
        /// OutAcctName	付款人名称	C(60)	必输	付款账户户名
        /// </summary>
        public string OutAcctName { get; set; }
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
        /// TranAmount	转出金额	C(13,2)	必输	如为XML报文，则直接输入输出以元为单位的浮点数值，如2.50 (两元五角)
        /// </summary>
        public decimal TranAmount { get; set; }
        /// <summary>
        /// UnionFlag	行内跨行标志	C(1)	必输	1：行内转账，0：跨行转账
        /// </summary>
        public string UnionFlag { get; set; }
        /// <summary>
        /// Fee1	手续费	C(13)	必输	转账手续费预算，实际手续费用以实际扣取的为准。
        /// </summary>
        public decimal Fee1 { get; set; }
        /// <summary>
        /// Fee2	邮电费	C(13)	非必输	
        /// </summary>
        public decimal Fee2 { get; set; }
        /// <summary>
        /// hostFlowNo	银行返回流水号	C(32)	必输	银行记账流水号；转账成功后，银行返回的流水号。
        /// </summary>
        public string hostFlowNo { get; set; }
        /// <summary>
        /// hostTxDate	记账日期	C(8)	非必输	银行交易成功后的记账日期，仅对行内实时转账交易有效。
        /// </summary>
        public string hostTxDate { get; set; }
        /// <summary>
        /// stt	交易状态标志	C(2)	非必输	20：交易成功30：失败；其他为银行受理成功处理中，请使用“交易进度查询4005”接口获取最终状态
        /// </summary>
        public string stt { get; set; }
        /// <summary>
        /// 转账退票标志	C(1)	非必输	0:未退票; 默认为0,1:退票;
        /// </summary>
        public int isBack { get; set; }
        /// <summary>
        /// 支付失败或退票原因描述	C(20)	非必输	如果是超级网银则返回如下信息:RJ01对方返回：账号不存在RJ02对方返回：账号、户名不符大小额支付则返回失败描述
        /// </summary>
        public string backRem { get; set; }
    }
}
