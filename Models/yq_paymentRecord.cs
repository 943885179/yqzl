//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityFromework
{
    using System;
    using System.Collections.Generic;
    
    public partial class yq_paymentRecord
    {
        public int id { get; set; }
        public string payCode { get; set; }
        public string thirdVoucher { get; set; }
        public string cstInnerFlowNo { get; set; }
        public string ccyCode { get; set; }
        public string outAcctNo { get; set; }
        public string outAcctName { get; set; }
        public string outAcctAddr { get; set; }
        public string inAcctBankNode { get; set; }
        public string inAcctRecCode { get; set; }
        public string inAcctNo { get; set; }
        public string inAcctName { get; set; }
        public string inAcctBankName { get; set; }
        public string inAcctProvinceCode { get; set; }
        public string inAcctCityName { get; set; }
        public decimal tranAmount { get; set; }
        public string useEx { get; set; }
        public string unionFlag { get; set; }
        public string sysFlag { get; set; }
        public int addrFlag { get; set; }
        public string mainAcctNo { get; set; }
        public string inIDType { get; set; }
        public string inIDNo { get; set; }
        public string frontLogNo { get; set; }
        public Nullable<decimal> fee1 { get; set; }
        public Nullable<decimal> fee2 { get; set; }
        public string hostFlowNo { get; set; }
        public string hostTxDate { get; set; }
        public string stt { get; set; }
        public string errCode { get; set; }
        public string errMessage { get; set; }
        public Nullable<System.DateTime> createTime { get; set; }
        public string createMan { get; set; }
        public Nullable<System.DateTime> updateTime { get; set; }
        public string updateMan { get; set; }
        public Nullable<int> isBack { get; set; }
        public string backRem { get; set; }
        public string eptdat { get; set; }
        public string epttim { get; set; }
        public string requestdata { get; set; }
        public string responsedata { get; set; }
        public Nullable<bool> isApproval { get; set; }
        public string approvalMan { get; set; }
        public Nullable<System.DateTime> approvalDate { get; set; }
        public string requestModelYY { get; set; }
        public string requestModelYQ { get; set; }
        public Nullable<int> company { get; set; }
        public Nullable<bool> isDel { get; set; }
    }
}