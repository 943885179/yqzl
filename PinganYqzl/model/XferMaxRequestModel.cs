using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinganYqzl.model
{
    /// <summary>
    /// 平安银行3.4企业大批量资金划转[4018]
    /// </summary>
    public class XferMaxRequestModel
    {
        /// <summary>
        /// 转账凭证号C(20)，最少10位长度必输	标示交易唯一性，同一客户上送的不可重复，建议格式：yyyymmddHHSS+8位系列要求6个月内唯一。
        /// </summary>
        public string ThirdVoucher { get; set; } //=DateTime.Now.ToString("yyyymmddHHSS")+ new Random().Next(10000000,100000000);// { get; set; }
        /// <summary>
        /// totalCts	总记录数	C(5)	必输	批量转账的笔数，笔数不大于500笔；
        /// </summary>
        public string totalCts { get; set; }
        /// <summary>
        /// totalAmt	总金额	C(13,2)	必输
        /// </summary>
        public string totalAmt { get; set; }
        /// <summary>
        /// BatchSummary	批次摘要	C(30)	非必输
        /// </summary>
        public string BatchSummary { get; set; }
        /// <summary>
        /// BSysFlag	整批转账加急标志	C(1) 	必输	Y：加急 N：不加急S：特急（汇总扣款模式不支持）
        /// </summary>
        public string BSysFlag { get; set; }
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
        /// OutAcctAddr	付款人地址	C(60)	非必输	建议填写付款账户的分行、网点名称
        /// </summary>
        public string OutAcctAddr { get; set; }
        /// <summary>
        /// PayType	扣款类型	C(1)	非必输，默认为0	0：单笔扣划1：汇总扣划BizFlag1 = 1时只支持0单笔扣划
        /// </summary>
        public int PayType { get; set; }
        /// <summary>
        /// BizFlag1	业务标识1	C(1)	非必输	1：转信托网银落地划款0或其他为普通直连交易
        /// </summary>
        public int BizFlag1 { get; set; }
        /// <summary>
        /// 以下为多条记录 标签名HOResultSet4018R
        /// </summary>
        public List<HOResultSet4018R> hOResultSet4018Rs { get; set; }
    }
    public class HOResultSet4018R
    {
        /// <summary>
        /// 单笔转账凭证号(批次中的流水号)/序号	C(20)	必输	同一个批次内不能重复，建议按顺序递增生成，若上送返回则按此字段递增排序。；建议为递增序号，如从1开始
        /// </summary>
        public int SThirdVoucher { get; set; }
        /// <summary>
        /// 客户自定义凭证号	C(20)	非必输	用于客户转账登记和内部识别，通过转账结果查询可以返回。银行不检查唯一性
        /// </summary>
        public int CstInnerFlowNo { get; set; }
        /// <summary>
        /// 收款人开户行行号	C(12)	非必输	跨行转账不落地，则必输。为人行登记在册的商业银行号
        /// </summary>
        public int InAcctBankNode { get; set; }
        /// <summary>
        /// 接收行行号	C(12)	非必输	跨行转账不落地，则必输。为人行登记在册的商业银行号
        /// </summary>
        public int InAcctRecCode { get; set; }
        /// <summary>
        /// 收款人账户	C(32)	必输	
        /// </summary>
        public int InAcctNo { get; set; }
        /// <summary>
        /// 收款人账户户名	C(60)	必输
        /// </summary>
        public int InAcctName { get; set; }
        /// <summary>
        /// 收款人开户行名称	C(60)	必输	建议格式：xxx银行xx分行xx支行
        /// </summary>
        public int InAcctBankName { get; set; }
        /// <summary>
        /// 收款账户开户省代码	C(2)	非必输	建议上送，减少跨行转账落单率。对照码参考“附录-省对照表”
        /// </summary>
        public int InAcctProvinceCode { get; set; }
        /// <summary>
        /// 收款账户开户市	C(12)	非必输	建议上送，减少跨行转账落单率。
        /// </summary>
        public int InAcctCityName { get; set; }
        /// <summary>
        /// TranAmount	转出金额	C(13,2)	必输
        /// </summary>
        public int TranAmount { get; set; }
        /// <summary>
        /// UseEx	资金用途	C(30)	必输
        /// </summary>
        public int UseEx { get; set; }
        /// <summary>
        /// UnionFlag	行内跨行标志	C(1)	必输	1：行内转账，0：跨行转账
        /// </summary>
        public int UnionFlag { get; set; }
        /// <summary>
        /// AddrFlag	同城/异地标志	C(1)  必输	1:同城 2:异地若无法区分，则默认可以填写同城
        /// </summary>
        public int AddrFlag { get; set; }

    }
}
