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
    
    public partial class Person
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Person()
        {
            this.GL_accvouch = new HashSet<GL_accvouch>();
        }
    
        public string cPersonCode { get; set; }
        public string cPersonName { get; set; }
        public string cDepCode { get; set; }
        public string cPersonProp { get; set; }
        public Nullable<double> fCreditQuantity { get; set; }
        public Nullable<int> iCreDate { get; set; }
        public string cCreGrade { get; set; }
        public Nullable<double> iLowRate { get; set; }
        public string cOfferGrade { get; set; }
        public Nullable<double> iOfferRate { get; set; }
        public byte[] pubufts { get; set; }
        public string cPersonEmail { get; set; }
        public string cPersonPhone { get; set; }
        public Nullable<System.DateTime> dPValidDate { get; set; }
        public Nullable<System.DateTime> dPInValidDate { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GL_accvouch> GL_accvouch { get; set; }
    }
}