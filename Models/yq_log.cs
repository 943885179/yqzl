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
    
    public partial class yq_log
    {
        public int id { get; set; }
        public System.DateTime createTime { get; set; }
        public string yqCode { get; set; }
        public string message { get; set; }
        public string userName { get; set; }
        public string ip { get; set; }
        public string requestXml { get; set; }
        public string responseXml { get; set; }
    }
}