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
    
    public partial class Ap_CloseBills
    {
        public int iID { get; set; }
        public int ID { get; set; }
        public byte iType { get; set; }
        public bool bPrePay { get; set; }
        public string cCusVen { get; set; }
        public decimal iAmt_f { get; set; }
        public decimal iAmt { get; set; }
        public decimal iRAmt_f { get; set; }
        public decimal iRAmt { get; set; }
        public string cKm { get; set; }
        public string cXmClass { get; set; }
        public string cXm { get; set; }
        public string cDepCode { get; set; }
        public string cPersonCode { get; set; }
        public string cOrderID { get; set; }
        public string cItemName { get; set; }
        public string cConType { get; set; }
        public string cConID { get; set; }
        public Nullable<double> iAmt_s { get; set; }
        public Nullable<double> iRAmt_s { get; set; }
        public Nullable<byte> iOrderType { get; set; }
        public string cDLCode { get; set; }
        public string ccItemCode { get; set; }
        public Nullable<int> RegisterFlag { get; set; }
        public string cDefine22 { get; set; }
        public string cDefine23 { get; set; }
        public string cDefine24 { get; set; }
        public string cDefine25 { get; set; }
        public Nullable<double> cDefine26 { get; set; }
        public Nullable<double> cDefine27 { get; set; }
        public string cDefine28 { get; set; }
        public string cDefine29 { get; set; }
        public string cDefine30 { get; set; }
        public string cDefine31 { get; set; }
        public string cDefine32 { get; set; }
        public string cDefine33 { get; set; }
        public Nullable<int> cDefine34 { get; set; }
        public Nullable<int> cDefine35 { get; set; }
        public Nullable<System.DateTime> cDefine36 { get; set; }
        public Nullable<System.DateTime> cDefine37 { get; set; }
        public string cStageCode { get; set; }
        public string cCoVouchID { get; set; }
        public string cExecID { get; set; }
        public string cMemo { get; set; }
        public Nullable<int> iSrcClosesID { get; set; }
        public decimal ifaresettled_f { get; set; }
        public string cEBOrderCode { get; set; }
    
        public virtual Ap_CloseBill Ap_CloseBill { get; set; }
    }
}
