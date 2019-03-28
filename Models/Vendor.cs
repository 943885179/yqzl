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
    
    public partial class Vendor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vendor()
        {
            this.GL_accvouch = new HashSet<GL_accvouch>();
        }
    
        public string cVenCode { get; set; }
        public string cVenName { get; set; }
        public string cVenAbbName { get; set; }
        public string cVCCode { get; set; }
        public string cDCCode { get; set; }
        public string cTrade { get; set; }
        public string cVenAddress { get; set; }
        public string cVenPostCode { get; set; }
        public string cVenRegCode { get; set; }
        public string cVenBank { get; set; }
        public string cVenAccount { get; set; }
        public Nullable<System.DateTime> dVenDevDate { get; set; }
        public string cVenLPerson { get; set; }
        public string cVenPhone { get; set; }
        public string cVenFax { get; set; }
        public string cVenEmail { get; set; }
        public string cVenPerson { get; set; }
        public string cVenBP { get; set; }
        public string cVenHand { get; set; }
        public string cVenPPerson { get; set; }
        public Nullable<double> iVenDisRate { get; set; }
        public string iVenCreGrade { get; set; }
        public Nullable<double> iVenCreLine { get; set; }
        public Nullable<int> iVenCreDate { get; set; }
        public string cVenPayCond { get; set; }
        public string cVenIAddress { get; set; }
        public string cVenIType { get; set; }
        public string cVenHeadCode { get; set; }
        public string cVenWhCode { get; set; }
        public string cVenDepart { get; set; }
        public Nullable<double> iAPMoney { get; set; }
        public Nullable<System.DateTime> dLastDate { get; set; }
        public Nullable<double> iLastMoney { get; set; }
        public Nullable<System.DateTime> dLRDate { get; set; }
        public Nullable<double> iLRMoney { get; set; }
        public Nullable<System.DateTime> dEndDate { get; set; }
        public Nullable<int> iFrequency { get; set; }
        public bool bVenTax { get; set; }
        public string cVenDefine1 { get; set; }
        public string cVenDefine2 { get; set; }
        public string cVenDefine3 { get; set; }
        public string cCreatePerson { get; set; }
        public string cModifyPerson { get; set; }
        public Nullable<System.DateTime> dModifyDate { get; set; }
        public string cRelCustomer { get; set; }
        public Nullable<int> iId { get; set; }
        public string cBarCode { get; set; }
        public string cVenDefine4 { get; set; }
        public string cVenDefine5 { get; set; }
        public string cVenDefine6 { get; set; }
        public string cVenDefine7 { get; set; }
        public string cVenDefine8 { get; set; }
        public string cVenDefine9 { get; set; }
        public string cVenDefine10 { get; set; }
        public Nullable<int> cVenDefine11 { get; set; }
        public Nullable<int> cVenDefine12 { get; set; }
        public Nullable<double> cVenDefine13 { get; set; }
        public Nullable<double> cVenDefine14 { get; set; }
        public Nullable<System.DateTime> cVenDefine15 { get; set; }
        public Nullable<System.DateTime> cVenDefine16 { get; set; }
        public byte[] pubufts { get; set; }
        public Nullable<double> fRegistFund { get; set; }
        public Nullable<int> iEmployeeNum { get; set; }
        public Nullable<short> iGradeABC { get; set; }
        public string cMemo { get; set; }
        public bool bLicenceDate { get; set; }
        public Nullable<System.DateTime> dLicenceSDate { get; set; }
        public Nullable<System.DateTime> dLicenceEDate { get; set; }
        public Nullable<int> iLicenceADays { get; set; }
        public bool bBusinessDate { get; set; }
        public Nullable<System.DateTime> dBusinessSDate { get; set; }
        public Nullable<System.DateTime> dBusinessEDate { get; set; }
        public Nullable<int> iBusinessADays { get; set; }
        public bool bProxyDate { get; set; }
        public Nullable<System.DateTime> dProxySDate { get; set; }
        public Nullable<System.DateTime> dProxyEDate { get; set; }
        public Nullable<int> iProxyADays { get; set; }
        public bool bPassGMP { get; set; }
        public bool bVenCargo { get; set; }
        public bool bProxyForeign { get; set; }
        public bool bVenService { get; set; }
        public string cVenTradeCCode { get; set; }
        public string cVenBankCode { get; set; }
        public string cVenExch_name { get; set; }
        public short iVenGSPType { get; set; }
        public Nullable<short> iVenGSPAuth { get; set; }
        public string cVenGSPAuthNo { get; set; }
        public string cVenBusinessNo { get; set; }
        public string cVenLicenceNo { get; set; }
        public bool bVenOverseas { get; set; }
        public bool bVenAccPeriodMng { get; set; }
        public string cVenPUOMProtocol { get; set; }
        public string cVenOtherProtocol { get; set; }
        public string cVenCountryCode { get; set; }
        public string cVenEnName { get; set; }
        public string cVenEnAddr1 { get; set; }
        public string cVenEnAddr2 { get; set; }
        public string cVenEnAddr3 { get; set; }
        public string cVenEnAddr4 { get; set; }
        public string cVenPortCode { get; set; }
        public string cVenPrimaryVen { get; set; }
        public Nullable<double> fVenCommisionRate { get; set; }
        public Nullable<double> fVenInsueRate { get; set; }
        public bool bVenHomeBranch { get; set; }
        public string cVenBranchAddr { get; set; }
        public string cVenBranchPhone { get; set; }
        public string cVenBranchPerson { get; set; }
        public string cVenSSCode { get; set; }
        public string cOMWhCode { get; set; }
        public string cVenCMProtocol { get; set; }
        public string cVenIMProtocol { get; set; }
        public Nullable<double> iVenTaxRate { get; set; }
        public System.DateTime dVenCreateDatetime { get; set; }
        public string cVenMnemCode { get; set; }
        public Nullable<System.Guid> cVenContactCode { get; set; }
        public string cvenbankall { get; set; }
        public Nullable<bool> bIsVenAttachFile { get; set; }
        public string cLicenceRange { get; set; }
        public string cBusinessRange { get; set; }
        public string cVenGSPRange { get; set; }
        public Nullable<System.DateTime> dVenGSPEDate { get; set; }
        public Nullable<System.DateTime> dVenGSPSDate { get; set; }
        public Nullable<int> iVenGSPADays { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_accvouch> GL_accvouch { get; set; }
    }
}