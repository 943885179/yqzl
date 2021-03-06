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
    
    public partial class col_summary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public col_summary()
        {
            this.col_body = new HashSet<col_body>();
        }
    
        public long id { get; set; }
        public Nullable<int> state { get; set; }
        public string subject { get; set; }
        public Nullable<long> deadline { get; set; }
        public Nullable<int> col_type { get; set; }
        public Nullable<byte> can_archive { get; set; }
        public Nullable<byte> can_modify { get; set; }
        public Nullable<byte> can_due_reminder { get; set; }
        public Nullable<byte> can_forward { get; set; }
        public Nullable<byte> can_edit { get; set; }
        public Nullable<byte> can_track { get; set; }
        public Nullable<long> remind_interval { get; set; }
        public Nullable<long> project_id { get; set; }
        public Nullable<int> important_level { get; set; }
        public Nullable<int> resent_time { get; set; }
        public Nullable<System.DateTime> create_date { get; set; }
        public Nullable<System.DateTime> start_date { get; set; }
        public Nullable<System.DateTime> finish_date { get; set; }
        public Nullable<byte> is_audited { get; set; }
        public Nullable<long> archive_id { get; set; }
        public Nullable<long> start_member_id { get; set; }
        public string process_id { get; set; }
        public Nullable<long> case_id { get; set; }
        public Nullable<long> advance_remind { get; set; }
        public string identifier { get; set; }
        public string forward_member { get; set; }
        public Nullable<long> templete_id { get; set; }
        public string workflow_rule { get; set; }
        public string webservice_code { get; set; }
        public string body_type { get; set; }
        public Nullable<long> form_recordid { get; set; }
        public Nullable<long> formid { get; set; }
        public Nullable<long> form_appid { get; set; }
        public Nullable<long> org_department_id { get; set; }
        public Nullable<long> org_account_id { get; set; }
        public string source { get; set; }
        public Nullable<long> parentform_summaryid { get; set; }
        public Nullable<int> newflow_type { get; set; }
        public byte can_edit_attachment { get; set; }
        public Nullable<byte> is_vouch { get; set; }
        public Nullable<long> over_worktime { get; set; }
        public Nullable<long> run_worktime { get; set; }
        public Nullable<long> over_time { get; set; }
        public Nullable<long> run_time { get; set; }
        public Nullable<byte> can_autostopflow { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<col_body> col_body { get; set; }
    }
}
