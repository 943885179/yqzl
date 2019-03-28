using System;

namespace model
{
   public class WriteModel
    {
        public string col_id { get; set; }//单据编号
        public DateTime? chuangjinshijin { get; set; }//创建时间（2019-01-23后修改签字读取规则）
        public string sybmjl { get;set; }//受益部门经理
        public string sybmzg { get; set; }//受益部门主管
        public string name { get; set; }//签字人
        public string depatement { get; set; }//签字部门
        public string leave { get; set; }//签字人等级

        public int attitude { get; set; }//签字人态度
        public DateTime? create_date { get; set; }//签字日期
        public int opinion_type { get; set; }//签字状态0发起人，1，同意的审核人，2，待办 3回退7 超时
    }
}
