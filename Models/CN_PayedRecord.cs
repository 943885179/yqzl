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
    
    public partial class CN_PayedRecord
    {
        public int ID { get; set; }
        public Nullable<int> iMainID { get; set; }
        public Nullable<int> iSubID { get; set; }
        public Nullable<int> iAcctBookID { get; set; }
        public Nullable<double> fMoneyF { get; set; }
        public Nullable<double> fMoney { get; set; }
        public Nullable<int> lMakeVouch { get; set; }
        public string cMainID { get; set; }
        public string cSubID { get; set; }
    }
}