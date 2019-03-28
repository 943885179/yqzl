using System;
using System.Collections.Generic;

namespace model
{
    public class ResultList {
        public List<ResultListModel> list { get; set; }
    }
    public class ResultListModel
    {
        /// <summary>
        /// 收还是付
        /// </summary>
        public Boolean isPay { get; set; }
        /// <summary>
        /// formmain id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? chuangjinshijin { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string faqiren { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string company { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }
        /// <summary>
        /// 小写总金额
        /// </summary>
        public decimal lAmount { get; set; }
        /// <summary>
        /// 大写金额
        /// </summary>
        public string cAmount { get; set; }
        /// <summary>
        /// 受益部门
        /// </summary>
        public string shouyibumen { get; set; }
        /// <summary>
        /// 受益部门对应的U8编码
        /// </summary>
        public string shouyibumenCode { get; set; }
        /// <summary>
        /// 表单类型，只支持Form
        /// </summary>
        public string body_type { get; set; }
        /// <summary>
        /// 申请表头
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public string finish_date { get; set; }
        /// <summary>
        /// 发起部门
        /// </summary>
        public string faqibumen { get; set; }
        /// <summary>
        /// 票据id
        /// </summary>
        public string colId { get; set; }
        /// <summary>
        /// 票据所在表
        /// </summary>
        public string tName { get; set; }
        /// <summary>
        /// 发起人id
        /// </summary>
        public string mid { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        //**************拓展后的新模板要拥有滴*************************//
        /// <summary>
        /// 预支
        /// </summary>
        public decimal yuzhi { get; set; }
        /// <summary>
        /// 应退、补
        /// </summary>
        public decimal yinhuan { get; set; }
        /// <summary>
        /// 收款单位
        /// </summary>
        public string shoukuandanwei { get; set; }
        /// <summary>
        /// 收款银行
        /// </summary>
        public string shoukuanyh { get; set; }
        /// <summary>
        /// 收款账号
        /// </summary>
        public string zhanhao { get; set; }
        /// <summary>
        /// 付款日期
        /// </summary>
        public string fukuanriqi { get; set; }
        /// <summary>
        /// 费用类型(判断是采购销售委外商的必须生单再制单)
        /// </summary>
        public string feiyongleixing { get; set; }
        //*****************差旅费特有的**********************//
        /// <summary>
        /// 替他人报销受益人为他人
        /// </summary>
        public string shouyiren { get; set; }
        /// <summary>
        /// 共计天数
        /// </summary>
        public double? gongjitianshu { get; set; }
        /// <summary>
        ///开始时间
        /// </summary>
        public DateTime? starDate { get; set; }
        /// <summary>
        /// 出差事由
        /// </summary>
        public string chuchaishiyou { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string endDate { get; set; }
        public double? danjushu { get; set; }
        //出纳的时候要选择的
        public string xiaozhang { get; set; }//付款银行
        public string fukuanpingzhenhao { get; set; }//付款确认凭证号
        public int? contentId { get; set; }
        public Content content { get; set; }
        /// <summary>
        /// 日记账日期
        /// </summary>
        public DateTime acctDate { get; set; }
        public string zhiwu { get; set; }//等级（流程签字需要）
        //申请明细
        public List<Detail> list { get; set; }
        public List<Piaoju> piaoju { get; set; }
        public int? UnitType { get; set; }//供应商或者客户1：客户 2 ：供应商
        public int? UnitID { get; set; }//供应商或者客户对应的id
        public int? shifouPingzhen { get; set; }//是否生成日记账
        public int? shifouShengdan { get; set; }//是否生单
        public int? shifouZhidan { get; set; }//是否制单
        public string liuShui { get; set; }//流水号
        public string chunabianhao { get; set; }//一单一证后拥有出纳编号
        public string danjubianhao { get; set; }//一单一证后拥有单据编号
        public string pingzhenhao { get; set; }//一单一证后拥有凭证号
        public  string beizhu { get; set; }//备注
        public string shouyibumenjl { get; set; }//受益部门经理
        public string shouyibumenzg { get; set; }//受益部门主管
        //打印记录模块
        public int? printNum { get; set; }//打印次数
        public string lastProcessName { get; set; }//最后审核人该签字的打印标志
        /// <summary>
        /// 来源表
        /// </summary>
        public string tabName { get; set; }
        /// <summary>
        /// 流水号所在字段
        /// </summary>
        public string  liushuiCol { get; set; }
        /// <summary>
        /// 审单财务对应枚举VALUE,不做连表查询，太多表联查了
        /// </summary>
        public  string caiwuValue { get; set; }
        /// <summary>
        /// 审单财务
        /// </summary>
        public string caiwuCol { get; set; }
        /// <summary>
        /// UnionFlag	行内跨行标志	C(1)	必输	1：行内转账，0：跨行转账 BNKFLG 系统内外标志Y：招行；N：非招行否
        /// </summary>
        public String UnionFlag { get; set; }
        /// <summary>
        /// SysFlag	转账加急标志	C(1) 非必输 N：普通（大小额自动选择），默认值；Y：加急 （大额）；S：特急(超级网银)；T1：深圳同城普通；T2：深圳同城实时；默认为N
        /// </summary>
        public string SysFlag { get; set; }
        /// <summary>
        /// AddrFlag	同城/异地标志	C(1) 必输	“1”—同城   “2”—异地；若无法区分，可默认送1-同城。
        /// </summary>
        public int AddrFlag { get; set; }
        /// <summary>
        /// 银行业务流水号
        /// </summary>
        public string thirdVoucher { get; set; }
        /// <summary>
        /// 结账类型
        /// </summary>
        public int contentType { get; set; }
        /// <summary>
        /// 是否启用银企直联付款 0启动付款，1不启动付款
        /// </summary>
        public int isYq { get; set; }
        /// <summary>
        /// 收款地址
        /// </summary>
        public string Dizhi { get; set; }
        /// <summary>
        /// 银行联号
        /// </summary>
        public string yqCode { get; set; }
    }

    public class Content {
        public int ID { get; set; }
        public string AcctName { get; set; }
        public string SubjectCode { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string UnitName { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string BankAcct { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string BankName  { get; set; }

        public string ccode_name { get; set; }
        /// <summary>
        /// 开户市（招行参照yq_cityCode_zhaohang表进行中文设置，不能加市字）
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 开户省
        /// </summary>
        public string Province { get; set; }
    }
    /// <summary>
    /// 每个票据
    /// </summary>
    public class Piaoju {//取消
        public int? tabClass { get; set; }//票据编号
        public string yinhuan { get; set; }//每个票据应还款
        public string yuzhijine { get; set; }//每个票据应付款
        public string yinhuanQuanbu { get; set; }//每个票据的累计金额（发票的金额总计）
        public string yuzhijineQuanbu { get; set; }//应付款
        public List<Detail> Detail { get; set; }//每个票据集合
        public Yinhan Yinhan { get; set; }//银行流量记录部分
        public string chunabianhao{get;set;}
        public string pingzhenhao { get; set; }
        public string danjubianhao { get; set; }
        public List<Liuliang> liuliangList { get; set; }//流量集合部分
        public List<Detail> Yuzhi { get; set; }//预支冲账部分


    }
    public class Yinhan {
        public string miaoshu { get; set; }//描述,做打印
        public decimal jine { get; set; }//kemu
        public string kemu { get; set; }//liushuizhang
        public string shouyibumen { get; set; }
        public decimal jiefan { get; set; }
    }
    public class Liuliang {
        public string citemccode { get; set; }
        public string cDirection { get; set; }
        public string citemcode { get; set; }
        public string citemname { get; set; }

        public string iotherused { get; set; }
        public decimal jine { get; set; }//支出（企业付款）
        public decimal md { get; set; }//收入（企业收款）
    }
    public class Detail
    {
        //报销单有
        public string miaoshu { get; set; }//描述,做打印
        public string xianqing { get; set; }//详情不做打印，太多了
        /// <summary>
        /// 金额mc
        /// </summary>
        public decimal? jine { get; set; }
        public DateTime? shijin { get; set; }//期限
        //差旅费特有
        public decimal? jinexiaoji { get; set; }//金额小计
        public DateTime? riqi { get; set; }//日期
        public decimal? qita { get; set; }//其他
        public decimal? jintie { get; set; }//差旅费津贴
        public decimal? shineijiaotong { get; set; }//本市内交通费
        public decimal? chuchaijiaotong { get; set; }//出差地市内交通费
        public decimal? zhusu { get; set; }//住宿费
        public decimal? chechuan { get; set; }//车船费
        public decimal? jipiao { get; set; }//机票费
        public double? tianshu { get; set; }//天数
        public string qizhididian { get; set; }//起止地点
        public DateTime? kaishiriqi { get; set; }
        public DateTime? jiezhiriqi { get; set; }
        //修改模板后，删除原有的 归类大纲
        public string kemu { get; set; }//科目
        public string shouyibumen { get; set; }//受益部门（改为返回u8部门的id）
        /// <summary>
        /// 税后金额
        /// </summary>
        public Double? shuihoujine { get; set; }
        /// <summary>
        /// 税点
        /// </summary>
        public int? shuidian { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public decimal? shuie { get; set; }
        public decimal? jiefan { get; set; }//借方金额md

        /****参数改变*****/
        public decimal? mc { get; set; }

        public decimal? md { get; set; }

    }

}
