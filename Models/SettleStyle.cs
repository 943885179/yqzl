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
    
    public partial class SettleStyle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SettleStyle()
        {
            this.Ap_CloseBill = new HashSet<Ap_CloseBill>();
        }
    
        public string cSSCode { get; set; }
        public string cSSName { get; set; }
        public bool bSSFlag { get; set; }
        public byte iSSGrade { get; set; }
        public bool bSSEnd { get; set; }
        public byte[] pubufts { get; set; }
        public short iSSBillType { get; set; }
        public Nullable<bool> bPortalSettle { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ap_CloseBill> Ap_CloseBill { get; set; }
    }
}
