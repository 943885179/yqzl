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
    
    public partial class GL_accvouch
    {
        public int i_id { get; set; }
        public byte iperiod { get; set; }
        public string csign { get; set; }
        public Nullable<int> isignseq { get; set; }
        public Nullable<short> ino_id { get; set; }
        public short inid { get; set; }
        public System.DateTime dbill_date { get; set; }
        public short idoc { get; set; }
        public string cbill { get; set; }
        public string ccheck { get; set; }
        public string cbook { get; set; }
        public byte ibook { get; set; }
        public string ccashier { get; set; }
        public Nullable<byte> iflag { get; set; }
        public string ctext1 { get; set; }
        public string ctext2 { get; set; }
        public string cdigest { get; set; }
        public string ccode { get; set; }
        public string cexch_name { get; set; }
        public decimal md { get; set; }
        public decimal mc { get; set; }
        public decimal md_f { get; set; }
        public decimal mc_f { get; set; }
        public double nfrat { get; set; }
        public double nd_s { get; set; }
        public double nc_s { get; set; }
        public string csettle { get; set; }
        public string cn_id { get; set; }
        public Nullable<System.DateTime> dt_date { get; set; }
        public string cdept_id { get; set; }
        public string cperson_id { get; set; }
        public string ccus_id { get; set; }
        public string csup_id { get; set; }
        public string citem_id { get; set; }
        public string citem_class { get; set; }
        public string cname { get; set; }
        public string ccode_equal { get; set; }
        public Nullable<byte> iflagbank { get; set; }
        public Nullable<byte> iflagPerson { get; set; }
        public Nullable<bool> bdelete { get; set; }
        public string coutaccset { get; set; }
        public Nullable<short> ioutyear { get; set; }
        public string coutsysname { get; set; }
        public string coutsysver { get; set; }
        public Nullable<System.DateTime> doutbilldate { get; set; }
        public Nullable<byte> ioutperiod { get; set; }
        public string coutsign { get; set; }
        public string coutno_id { get; set; }
        public Nullable<System.DateTime> doutdate { get; set; }
        public string coutbillsign { get; set; }
        public string coutid { get; set; }
        public Nullable<bool> bvouchedit { get; set; }
        public Nullable<bool> bvouchAddordele { get; set; }
        public Nullable<bool> bvouchmoneyhold { get; set; }
        public Nullable<bool> bvalueedit { get; set; }
        public Nullable<bool> bcodeedit { get; set; }
        public string ccodecontrol { get; set; }
        public Nullable<bool> bPCSedit { get; set; }
        public Nullable<bool> bDeptedit { get; set; }
        public Nullable<bool> bItemedit { get; set; }
        public Nullable<bool> bCusSupInput { get; set; }
        public string cDefine1 { get; set; }
        public string cDefine2 { get; set; }
        public string cDefine3 { get; set; }
        public Nullable<System.DateTime> cDefine4 { get; set; }
        public Nullable<int> cDefine5 { get; set; }
        public Nullable<System.DateTime> cDefine6 { get; set; }
        public Nullable<double> cDefine7 { get; set; }
        public string cDefine8 { get; set; }
        public string cDefine9 { get; set; }
        public string cDefine10 { get; set; }
        public string cDefine11 { get; set; }
        public string cDefine12 { get; set; }
        public string cDefine13 { get; set; }
        public string cDefine14 { get; set; }
        public Nullable<int> cDefine15 { get; set; }
        public Nullable<double> cDefine16 { get; set; }
        public Nullable<System.DateTime> dReceive { get; set; }
        public string cWLDZFlag { get; set; }
        public Nullable<System.DateTime> dWLDZTime { get; set; }
        public bool bFlagOut { get; set; }
        public Nullable<int> iBG_OverFlag { get; set; }
        public string cBG_Auditor { get; set; }
        public Nullable<System.DateTime> dBG_AuditTime { get; set; }
        public string cBG_AuditOpinion { get; set; }
        public Nullable<bool> bWH_BgFlag { get; set; }
        public Nullable<int> ssxznum { get; set; }
        public string CErrReason { get; set; }
        public string BG_AuditRemark { get; set; }
        public string cBudgetBuffer { get; set; }
        public Nullable<short> iBG_ControlResult { get; set; }
        public string NCVouchID { get; set; }
        public Nullable<System.DateTime> daudit_date { get; set; }
        public string RowGuid { get; set; }
        public string cBankReconNo { get; set; }
        public Nullable<short> iyear { get; set; }
        public Nullable<int> iYPeriod { get; set; }
        public Nullable<System.DateTime> wllqDate { get; set; }
        public Nullable<int> wllqPeriod { get; set; }
        public Nullable<System.DateTime> tvouchtime { get; set; }
        public string cblueoutno_id { get; set; }
        public string ccodeexch_equal { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Department Department { get; set; }
        public virtual Person Person { get; set; }
        public virtual Vendor Vendor { get; set; }
    }
}