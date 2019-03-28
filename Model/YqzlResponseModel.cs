using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model;

namespace Model
{
    public class YqzlResponseModel<T>
    {  /// <summary>
       /// 返回编码
       /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 支付金额（借）
        /// </summary>
        public decimal payMoney { get; set; }
        /// <summary>
        /// 收款金额（贷）
        /// </summary>
        public decimal getMoney { get; set; }
        public T result { get; set; }
    }
    public class PaysModel {
        public List<PayModel> pays { get; set; }
    }
    public class PayModel
    {
        /// <summary>
        /// TRSAMTD 借方金额 M
        /// </summary>
        public decimal TRSAMTD { get; set; }
        /// <summary>
        /// TRSAMTC 贷方金额 M 可
        /// </summary>
        public decimal TRSAMTC { get; set; }
        /// <summary>
        /// AMTCDR 借贷标记 C（1） C:贷；D:
        /// </summary>
        public string AMTCDR { get; set; }
        /// <summary>
        /// TRSBLV 余额 
        /// </summary>
        public string TRSBLV { get; set; }
        /// <summary>
        /// 收款流水号
        /// </summary>
        public string liushuihan { get; set; }
        /// <summary>
        /// 收（付）方账号
        /// </summary>
        public string inAcctNo { get; set; }
        /// <summary>
        ///收(付)方名称
        /// </summary>
        public string inAcctName { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string useEx { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? tranAmount { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public string hostTxDate { get; set; }
        /// <summary>
        /// 出纳编号
        /// </summary>
        public string chunabianhao { get; set; }
        /// <summary>
        /// 对应OAID
        /// </summary>
        public ResultListModel oa { get; set; }
    }
}
