using EntityFromework;
using System;

namespace model
{
  public  class Condition
    {
        /// <summary>
        /// formmain id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 流水号 id
        /// </summary>
        public string liuShui { get; set; }
        public long col_id { get; set; }
        /// <summary>
        /// 时间范围开始
        /// </summary>
        public DateTime? startDate { get; set; }
        /// <summary>
        /// 时间范围结束
        /// </summary>
        public DateTime? endDate { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string menberName { get; set; }
        public string start { get; set; }

        public string type { get; set; }
        //TODO：这些配置后续放到数据库中方便拓展,目前因为也就三个公司用的不广直接放在代码中
        /// <summary>
        /// 申请主表名称
        /// </summary>
        public string formmainName { get; set; }
        /// <summary>
        /// 申请主表的主键名
        /// </summary>
        public string formmainId { get; set; }
        /// <summary>
        /// 申请子表名称
        /// </summary>
        public string  formsonName { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public userInfo userInfo{get;set;}
        public form_enumvalue form_enumvalue { get; set; }
        /// <summary>
        /// 金额（支付）
        /// </summary>
        public decimal money { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string AcctNo { get; set; }
    }
}
