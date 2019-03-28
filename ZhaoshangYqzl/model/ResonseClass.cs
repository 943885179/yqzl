using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZhaoshangYqzl.model
{
    public class INFO
    {
        /// <summary>
        /// FUNNAM 函数名 C(1, 20) 否
        /// </summary>
        public string FUNNAM { get; set; }
        /// <summary>
        /// 数据格式 N(1) 2：xml 格式三
        /// </summary>
        public string DATTYP { get; set; }
        /// <summary>
        /// 陆用户名 Z(1,20) 可 前置机模式必填
        /// </summary>
        public string LGNNAM { get; set; }
        /// <summary>
        /// ERRMSG 错误信息 Z(1,256) 可
        /// </summary>
        public string ERRMSG { get; set; }
        /// <summary>
        /// 返回代码 N 否
        /// </summary>
        public string RETCOD { get; set; }
    }
    /// <summary>
    /// 账户管理【请求实体】（查询账户详细信息实体）
    /// 账户详细信息查询输入接口
    /// </summary>
    public class SDKACINFX
    {
        /// <summary>
        /// BBKNBR 分行号 N(2) 附录 A.1 否 分行号和分行名称不能同时为空
        /// </summary>
        public string BBKNBR { get; set; }
        /// <summary>
        /// C_BBKNBR 分行名称 Z(1,62) 附录 A.1 是
        /// </summary>
        public string C_BBKNBR { get; set; }
        /// <summary>
        /// ACCNBR 账号 C(1,35) 否
        /// </summary>
        public string ACCNBR { get; set; }
    }
    /// <summary>
    /// 账户管理【返回实体】（查询账户详细信息实体）
    /// 账户详细信息查询输出接口
    /// </summary>
    public class NTQACINFZ
    {
        /// <summary>
        /// ACCBLV 上日余额 当 INTCOD='S'时，这个字段的值显示 为 " 头 寸 额 度（集团支付子公司余额）"是子公司的虚拟余额
        /// </summary>
        public string ACCBLV { get; set; }
        /// <summary>
        /// ACCITM 科目 C（5,5） 否 科目代码
        /// </summary>
        public string ACCITM { get; set; }
        /// <summary>
        /// ACCNAM 注解 Z（62） 否 一般为户名
        /// </summary>
        public string ACCNAM { get; set; }
        /// <summary>
        /// ACCNBR 帐号 C（35） 否
        /// </summary>
        public string ACCNBR { get; set; }
        /// <summary>
        /// AVLBLV 可用余额 M 可
        /// </summary>
        public string AVLBLV { get; set; }
        /// <summary>
        /// BBKNBR 分行号 C（2,2） 否招商银行分行代码（代码对照表请参照附录）
        /// </summary>
        public string BBKNBR { get; set; }
        /// <summary>
        /// CCYNBR 币种 C（2,2） 否帐号币种代码（代码对照表请参照附录
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        /// C_CCYNBR 币种名称 Z(10) 可
        /// </summary>
        public string C_CCYNBR { get; set; }
        /// <summary>
        /// C_INTRAT 年利率 C（11） 可
        /// </summary>
        public string C_INTRAT { get; set; }
        /// <summary>
        /// DPSTXT 存期 Z（12）定期时，取值：一天 七天一个月三个月六个月一年二年三年四年
        /// </summary>
        public string DPSTXT { get; set; }
        /// <summary>
        /// HLDBLV 冻结余额 M
        /// </summary>
        public string HLDBLV { get; set; }
        /// <summary>
        /// LMTOVR 透支额度 M
        /// </summary>
        public string LMTOVR { get; set; }
        /// <summary>
        /// ONLBLV 联机余额 M 否
        /// </summary>
        public string ONLBLV { get; set; }
        /// <summary>
        /// OPNDAT 开户日 D 否 8 位数字
        /// </summary>
        public string OPNDAT { get; set; }
        /// <summary>
        /// STSCOD 状态 C（1）A=活动，B=冻结，C=关户 否
        /// </summary>
        public string STSCOD { get; set; }
        /// <summary>
        /// MUTDAT 到期日 D 可 8 位
        /// </summary>
        public string MUTDAT { get; set; }
        /// <summary>
        /// INTCOD 利息码 C（1） S=子公司虚拟余额 可
        /// </summary>
        public string INTCOD { get; set; }
    }

    /// <summary>
    /// 支付实体
    /// </summary>
    public class CSRRCFDFY0
    {
        /// <summary>
        /// 
        /// </summary>
        public string EACNBR { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BEGDAT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ENDDAT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RRCFLG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RRCCOD { get; set; }
    }
    /// <summary>
    /// 账户交易 信息查询输入接口
    /// </summary>
    public class SDKTSINFX
    {
        /// <summary>
        /// BBKNBR 分行号 N(2) 附录 A.1 否 分行号和分行名称不能同时为空
        /// </summary>
        public string BBKNBR { get; set; }
        /// <summary>
        /// C_BBKNBR 分行名称 Z(1,62) 附录 A.1 是
        /// </summary>
        public string C_BBKNBR { get; set; }
        /// <summary>
        /// ACCNBR 账号 C(1,35) 否
        /// </summary>
        public string ACCNBR { get; set; }
        /// <summary>
        /// BGNDAT 起始日期 D 否 yyyyMMdd
        /// </summary>
        public string BGNDAT { get; set; }
        /// <summary>
        /// ENDDAT 结束日期 D 否 与结束日期的间隔不能超过 100 天 yyyyMMdd
        /// </summary>
        public string ENDDAT { get; set; }
        /// <summary>
        /// LOWAMT 最小金额 M 可 默认 0.00
        /// </summary>
        public decimal LOWAMT { get; set; }
        /// <summary>
        /// HGHAMT 最大金额 M 可 默 认9999999999999.99
        /// </summary>
        public decimal HGHAMT { get; set; }
        /// <summary>
        /// AMTCDR 借贷码 C(1) C：收入D：支出可
        /// </summary>
        public string AMTCDR { get; set; }
    }
    /// <summary>
    /// 账 户 交易 信息查询输出接口
    /// </summary>
    public class NTQTSINFZ
    {
        /// <summary>
        /// ETYDAT 交易日 D 否 交易发生的日
        /// </summary>
        public string ETYDAT { get; set; }
        /// <summary>
        /// ETYTIM 交易时间 T 否交易发生的时间，只有小时有效
        /// </summary>
        public string ETYTIM { get; set; }
        /// <summary>
        /// VLTDAT 起息日 D 可 开始计息的日期 
        /// </summary>
        public string VLTDAT { get; set; }
        /// <summary>
        ///  TRSCOD 交易类型 （8） 附录 可  
        /// </summary>
        public string TRSCOD { get; set; }
        /// <summary>
        /// NARYUR 摘要 Z（62）
        /// </summary>
        public string NARYUR { get; set; }
        /// <summary>
        /// TRSAMTD 借方金额 M
        /// </summary>
        public decimal TRSAMTD { get; set; }
        /// <summary>
        /// TRSAMTC 贷方金额 M 可
        /// </summary>
        public decimal TRSAMTC { get; set; }
        /// <summary>
        /// AMTCDR 借贷标记 C（1） C:贷；D:
        /// </summary>
        public string AMTCDR { get; set; }
        /// <summary>
        /// TRSBLV 余额 
        /// </summary>
        public string TRSBLV { get; set; }
        /// <summary>
        /// REFNBR 流水号 C（15） 否银行会计系统交易流水号
        /// </summary>
        public string REFNBR { get; set; }
        /// <summary>
        /// REQNBR 流程实例号 N（10） 可企业银行交易序号，唯一标示企业
        /// </summary>
        public string REQNBR { get; set; }
        /// <summary>
        /// USNAM 业务名称 Z（32） 可 
        /// </summary>
        public string USNAM { get; set; }
        /// <summary>
        /// NUSAGE 用途 Z（62） 可 
        /// </summary>
        public string NUSAGE { get; set; }
        /// <summary>
        /// YURREF 业务参考号 C（30） 可企
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// BUSNAR 业务摘要 Z（2
        /// </summary>
        public string BUSNAR { get; set; }
        /// <summary>
        /// OTRNAR 其它摘要Z
        /// </summary>
        public string OTRNAR { get; set; }
        /// <summary>
        /// C_RPYBBK 收/付方开户地区
        /// /// </summary>
        public string C_RPYBBK { get; set; }
        /// <summary>
        /// RPYNAM 收/付方名称Z（62）可收/付方帐户名
        /// </summary>
        public string RPYNAM { get; set; }
        /// <summary>
        /// RPYACC 收/付方帐号N（35）可收/付方的转入或转出帐号
        /// </summary>
        public string RPYACC { get; set; }
        /// <summary>
        /// RPYBBN 收/付方开户行行号
        /// </summary>
        public string RPYBBN { get; set; }
        /// <summary>
        /// RPYBNK 收/付方开户行名
        /// </summary>
        public string RPYBNK { get; set; }
        /// <summary>
        /// RPYADR 收/付方开户行地址Z
        /// </summary>
        public string RPYADR { get; set; }
        /// <summary>
        /// C_GSBBBK 母/子公司所在
        /// </summary>
        public string C_GSBBBK { get; set; }
        /// <summary>
        /// GSBACC 母/子公司帐 号
        /// </summary>
        public string GSBACC { get; set; }
        /// <summary>
        ///GSBNAM 母/子公司名
        /// </summary>
        public string GSBNAM { get; set; }
        /// <summary>
        /// INFFLG 信息标志
        /// </summary>
        public string INFFLG { get; set; }
        /// <summary>
        /// ATHFLG 有否附件信息标志 C(1) Y/N
        /// </summary>
        public string ATHFLG { get; set; }
        /// <summary>
        /// CHKNBR 票据号C
        /// </summary>
        public string CHKNBR { get; set; }
        /// <summary>
        /// RSVFLG 冲帐标志C
        /// </summary>
        public string RSVFLG { get; set; }
        /// <summary>
        /// NAREXT 扩展摘要
        /// </summary>
        public string NAREXT { get; set; }
        /// <summary>
        /// TRSANL 交易分析码 C（6
        /// </summary>
        public string TRSANL { get; set; }
        /// <summary>
        /// FRMCOD 企业识别码 N(10)
        /// </summary>
        public string FRMCOD { get; set; }
    }
    /// <summary>
    /// 支 付 输入 概要接口否
    /// </summary>
    public class SDKPAYRQX
    {
        /// <summary>
        /// BUSCOD 业务类别 C(6) N02031:直接支付N02041:直接集团支付 否 直接集团支付是指使用子公司账号付款, 总公司账号联动下划资金的支付。
        /// </summary>
        public string BUSCOD { get; set; }
        /// <summary>
        /// BUSMOD 业 务 模 式 编号C(5) 默认为 00001 可
        /// </summary>
        public string BUSMOD { get; set; }
        /// <summary>
        /// MODALS 业 务 模 式 名称业务模式编号和业务模式名称同时有值时业务模式编号有效；
        /// 可经办的业务模式，
        /// 可通过查询可经办的业务模式信息（ ListMode ） 获得
        /// ，也可以在通过前置机程序查询获得。
        /// </summary>
        public string MODALS { get; set; }

    }
    /// <summary>
    /// DCOPDPAYX 支 付 输入 明细接口否 1..30 或 者30..1500支付条数不超过 30 条，支付输出有NTQPAYRQZ 数据；超过 30 条，则无。
    /// </summary>
    public class DCOPDPAYX
    {
        /// <summary>
        /// YURREF 业务参考号 C（30） 否
        /// 用于标识该笔业务的编号，企业银行编号+业务类型+业务参考号必须唯一。
        /// 企业可以自定义业务参考号，也可使用银行缺省值（单笔支付），
        /// 批量支付须由企业提供。直联必须用企业提供
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// EPTDAT 期望日 D 默认为当前日期 可
        /// </summary>
        public string EPTDAT { get; set; }
        /// <summary>
        /// EPTTIM 期望时间 T 默认为‘00000
        /// </summary>
        public string EPTTIM { get; set; }
        /// <summary>
        /// DBTACC 付方帐号 N（35） 否
        /// </summary>
        public string DBTACC { get; set; }
        /// <summary>
        /// DBTBBK 付方开户地区代码 付方帐号的开户行所在地区，如北京、上海、深圳等付方开户地区和付方开户地区代码不能同时为空，同 时 有 值 时DBTBBK 有效
        /// </summary>
        public string DBTBBK { get; set; }
        /// <summary>
        /// TRSAMT 交易金额 M 否
        /// </summary>
        public decimal TRSAMT { get; set; }
        /// <summary>
        /// CCYNBR 币种代码 币种代码和币种名称不能同时为空同时有值时CCYNBR 
        /// 有效。。币种暂时只支持 10(人民币)
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        /// STLCHN 结算方式代码C(1)N：普通F：快速 否只对跨行交易有效
        /// </summary>
        public string STLCHN { get; set; }
        /// <summary>
        /// NUSAGE 用途 Z（62） 否对应对账单中的摘要 NARTXT
        /// </summary>
        public string NUSAGE { get; set; }
        /// <summary>
        /// BUSNAR 业务摘要 Z（200）可用于企业付款时填写说明或者备注。
        /// </summary>
        public string BUSNAR { get; set; }
        /// <summary>
        /// CRTACC 收方帐号 N（35）否
        /// </summary>
        public string CRTACC { get; set; }
        /// <summary>
        /// 收款方企业的转入帐号的帐户名称。
        /// CRTNAM 收方帐户名 Z（62） 可收方帐户名与收方长户名不能同时为空
        /// </summary>
        public string CRTNAM { get; set; }
        /// <summary>
        /// LRVEAN 收方长户名 Z(200) 可
        /// </summary>
        public string LRVEAN { get; set; }
        /// <summary>
        /// BRDNBR 收方行号 C(30) 可人行自动支付收方联行号
        /// </summary>
        public string BRDNBR { get; set; }
        /// <summary>
        /// BNKFLG 系统内外标志Y：招行；N：非招行否
        /// </summary>
        public string BNKFLG { get; set; }
        /// <summary>
        /// CRTBNK 收方开户行 Z（62）跨 行 支 付（BNKFLG=N）必
        /// </summary>
        public string CRTBNK { get; set; }
        /// <summary>
        /// CTYCOD 城市代码 C(4)附录 CRTFLG 不为 Y 时
        /// 行内支付必填行内支付填写，为空则不支持收方识别功能。
        /// </summary>
        public string CTYCOD { get; set; }
        /// <summary>
        /// CRTADR 收方行地址 Z(62)跨 行 支 付（BNKFLG=N）必填；CRTFLG 不为 Y时行内支付必
        /// </summary>
        public string CRTADR { get; set; }
        /// <summary>
        /// CRTFLG 收方信息不检查标志 C(1)Y: 行内支付不检查城市代码和收方行地址默认为 Y
        /// </summary>
        public string CRTFLG { get; set; }
        /// <summary>
        /// NTFCH1 收方电子邮件C（36） 可收款方的电子邮件地址，用于交易成功后邮件通知
        /// </summary>
        public string NTFCH1 { get; set; }
        /// <summary>
        /// NTFCH2 收方移动电话C（16） 可收款方的移动电话，用于交易 成功后短信通知
        /// </summary>
        public string NTFCH2 { get; set; }
        /// <summary>
        /// CRTSQN 收方编号 C（20） 可用于标识收款方的编号。非受限收方模式下可重复。
        /// </summary>
        public string CRTSQN { get; set; }
        /// <summary>
        /// TRSTYP 业务种类 C(6)100001=普通汇兑101001= 慈善捐款101002 =其他 默认 100001
        /// </summary>
        public string TRSTYP { get; set; }
        /// <summary>
        /// RSV28Z 保留字段 C(27) 可虚拟户支付时，前10 位填虚拟户编号；集团支付不支持虚拟户支
        /// </summary>
        public string RSV28Z { get; set; }
        /// <summary>
        /// RCVCHK  行内收方账号户名校验 C(1)1：校验空或者其他值：
        /// 不校验可如果为 1，行内收方账号与户名不相符则支付经办失败。
        /// </summary>
        public string RCVCHK { get; set; }
    }
    /// <summary>
    /// NTQPAYRQZ 支付输出接口 可 1．.30
    /// </summary>
    public class NTQPAYRQZ
    {
        /// <summary>
        /// SQRNBR 流水号 C(10) 可 批量经办时，用来表示第几笔记录
        /// </summary>
        public string SQRNBR { get; set; }
        /// <summary>
        /// YURREF 业务参考号 C(30) 可
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// REQNBR 流程实例号 C(10) 可
        /// </summary>
        public string REQNBR { get; set; }
        /// <summary>
        /// REQSTS 业务请求状态C(3) 附录 A.5 否
        /// </summary>
        public string REQSTS { get; set; }
        /// <summary>
        /// RTNFLG 业务处理结果C(1) 附录 A.6 可 REQSTS = FIN 时，RTNFLG 才有意义
        /// </summary>
        public string RTNFLG { get; set; }
        /// <summary>
        /// OPRSQN 待处理操作序列C(3) 可
        /// </summary>
        public string OPRSQN { get; set; }
        /// <summary>
        /// OPRALS 操作别名 C(32) 可
        /// </summary>
        public string OPRALS { get; set; }
        /// <summary>
        /// ERRCOD 错误码 C(7) 可
        /// </summary>
        public string ERRCOD { get; set; }
        /// <summary>
        /// ERRTXT 错误文本 Z(256) 
        /// </summary>
        public string ERRTXT { get; set; }
    }
    /// <summary>
    /// 支 付 输入 明细接口
    /// </summary>
    public class SDKPAYDTX
    {
        /// <summary>
        /// YURREF 业务参考号 C（30） 否
        /// 用于标识该笔业务的编号，企业银行编号+业务类型+业务参考号必须唯一。
        /// 企业可以自定义业务参考号，也可使用银行缺省值（单笔支付），
        /// 批量支付须由企业提供。直联必须用企业提供
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// EPTDAT 期望日 D 默认为当前日期 可
        /// </summary>
        public string EPTDAT { get; set; }
        /// <summary>
        /// EPTTIM 期望时间 T 默认为‘00000
        /// </summary>
        public string EPTTIM { get; set; }
        /// <summary>
        /// DBTACC 付方帐号 N（35） 否
        /// </summary>
        public string DBTACC { get; set; }
        /// <summary>
        /// DBTBBK 付方开户地区代码 付方帐号的开户行所在地区，如北京、上海、深圳等付方开户地区和付方开户地区代码不能同时为空，同 时 有 值 时DBTBBK 有效
        /// </summary>
        public string DBTBBK { get; set; }
        /// <summary>
        ///付方开户地区
        /// </summary>
        public string C_DBTBBK { get; set; }
        /// <summary>
        /// TRSAMT 交易金额 M 否
        /// </summary>
        public decimal TRSAMT { get; set; }
        /// <summary>
        /// CCYNBR 币种代码 币种代码和币种名称不能同时为空同时有值时CCYNBR 
        /// 有效。。币种暂时只支持 10(人民币)
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        /// 币种名称
        /// </summary>
        public string C_CCYNBR { get; set; }
        /// <summary>
        /// STLCHN 结算方式代码C(1)N：普通F：快速 否只对跨行交易有效
        /// </summary>
        public string STLCHN { get; set; }
        /// <summary>
        /// NUSAGE 用途 Z（62） 否对应对账单中的摘要 NARTXT
        /// </summary>
        public string NUSAGE { get; set; }
        /// <summary>
        /// BUSNAR 业务摘要 Z（200）可用于企业付款时填写说明或者备注。
        /// </summary>
        public string BUSNAR { get; set; }
        /// <summary>
        /// CRTACC 收方帐号 N（35）否
        /// </summary>
        public string CRTACC { get; set; }
        /// <summary>
        /// 收款方企业的转入帐号的帐户名称。
        /// CRTNAM 收方帐户名 Z（62） 可收方帐户名与收方长户名不能同时为空
        /// </summary>
        public string CRTNAM { get; set; }
        /// <summary>
        /// LRVEAN 收方长户名 Z(200) 可
        /// </summary>
        public string LRVEAN { get; set; }
        /// <summary>
        /// BRDNBR 收方行号 C(30) 可人行自动支付收方联行号
        /// </summary>
        public string BRDNBR { get; set; }
        /// <summary>
        /// BNKFLG 系统内外标志Y：招行；N：非招行否
        /// </summary>
        public string BNKFLG { get; set; }
        /// <summary>
        /// CRTBNK 收方开户行 Z（62）跨 行 支 付（BNKFLG=N）必
        /// </summary>
        public string CRTBNK { get; set; }
        /// <summary>
        /// C_CRTBBK收方开户地区名称Z（12）附录 A.1可开户地区代码与名称不能同时为
        /// </summary>
        public string C_CRTBBK { get; set; }
        /// <summary>
        /// CTYCOD 城市代码 C(4)附录 CRTFLG 不为 Y 时
        /// 行内支付必填行内支付填写，为空则不支持收方识别功能。
        /// </summary>
        public string CTYCOD { get; set; }
        /// <summary>
        /// CRTADR 收方行地址 Z(62)跨 行 支 付（BNKFLG=N）必填；CRTFLG 不为 Y时行内支付必
        /// </summary>
        public string CRTADR { get; set; }
        /// <summary>
        /// CRTFLG 收方信息不检查标志 C(1)Y: 行内支付不检查城市代码和收方行地址默认为 Y
        /// </summary>
        public string CRTFLG { get; set; }
        /// <summary>
        /// NTFCH1 收方电子邮件C（36） 可收款方的电子邮件地址，用于交易成功后邮件通知
        /// </summary>
        public string NTFCH1 { get; set; }
        /// <summary>
        /// NTFCH2 收方移动电话C（16） 可收款方的移动电话，用于交易 成功后短信通知
        /// </summary>
        public string NTFCH2 { get; set; }
        /// <summary>
        /// CRTSQN 收方编号 C（20） 可用于标识收款方的编号。非受限收方模式下可重复。
        /// </summary>
        public string CRTSQN { get; set; }
        /// <summary>
        /// TRSTYP 业务种类 C(6)100001=普通汇兑101001= 慈善捐款101002 =其他 默认 100001
        /// </summary>
        public string TRSTYP { get; set; }
        /// <summary>
        /// RSV28Z 保留字段 C(27) 可虚拟户支付时，前10 位填虚拟户编号；集团支付不支持虚拟户支
        /// </summary>
        public string RSV28Z { get; set; }
        /// <summary>
        /// RCVCHK  行内收方账号户名校验 C(1)1：校验空或者其他值：
        /// 不校验可如果为 1，行内收方账号与户名不相符则支付经办失败。
        /// </summary>
        public string RCVCHK { get; set; }
    }
    /// <summary>
    /// NTQRYSTYX1 输入接口 1
    /// </summary>
    public class NTQRYSTYX1
    {
        /// <summary>
        /// BUSCOD 业务类型 C(6) 否 为下列之一：
        /// N02020: 内部转帐
        /// N02030: 支付
        /// N02031: 直接支付
        /// N02040: 集团支付
        /// N02041: 直接集团支付
        /// </summary>
        public string BUSCOD { get; set; }
        /// <summary>
        /// YURREF 业务参考号 C(30) 否 只查询指定的业务参考号交易（不提供模糊查询）
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// BGNDAT 起始日期 D 否 yyyyMMdd
        /// 按经办日期查询，起始结束日期间隔不能超过一周起始日期不可小于当前日期前 90 天（日切零点附近的交易若查询不到可尝试跨日查询）
        /// </summary>
        public string BGNDAT { get; set; }
        /// <summary>
        /// ENDDAT 结束日期 D 否 yyyyMMdd
        /// </summary>
        public string ENDDAT { get; set; }
    }
    /// <summary>
    /// NTSTLLSTZ 输出接口(多记录)n n 为查询的实际结果数
    /// </summary>
    public class NTSTLLSTZ
    {
        /// <summary>
        /// REQNBR 流程实例号 C(10) 
        /// </summary>
        public string REQNBR { get; set; }
        /// <summary>
        /// BUSCOD 业务编码 C(6) 附录 A.4
        /// </summary>
        public string BUSCOD { get; set; }
        /// <summary>
        /// BUSMOD 业务模式 C(5) 
        /// </summary>
        public string BUSMOD { get; set; }
        /// <summary>
        /// DBTBBK 转出分行号 C(2) 附录 A.1
        /// </summary>
        public string DBTBBK { get; set; }
        /// <summary>
        /// DBTACC 付方帐号 C(35) 
        /// </summary>
        public string DBTACC { get; set; }
        /// <summary>
        /// CRTBBK 收方分行号 C(2) 附录 A.1
        /// </summary>
        public string CRTBBK { get; set; }
        /// <summary>
        /// CRTACC 收方帐号 C(35) 
        /// </summary>
        public string CRTACC { get; set; }
        /// <summary>
        /// CRTNAM 收方名称 Z(122)
        /// </summary>
        public string CRTNAM { get; set; }
        /// <summary>
        /// CCYNBR 币种 C(2) 附录 A.3
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        /// TRSAMT 交易金额 M
        /// </summary>
        public string TRSAMT { get; set; }
        /// <summary>
        /// EPTDAT 期望日
        /// </summary>
        public string EPTDAT { get; set; }
        /// <summary>
        /// EPTTIM 期望时间 T
        /// </summary>
        public string EPTTIM { get; set; }
        /// <summary>
        /// OPRDAT 经办日
        /// </summary>
        public string OPRDAT { get; set; }
        /// <summary>
        /// YURREF 对方参考号 C(30)
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// REQSTS 请求状态 C(3) 附录 A.5
        /// </summary>
        public string REQSTS { get; set; }
        /// <summary>
        /// RTNFLG 业务处理结果 C(1) 附录 A.6
        /// </summary>
        public string RTNFLG { get; set; }
        /// <summary>
        /// ATHFLG 是否有附件信息 C(1) 
        /// </summary>
        public string ATHFLG { get; set; }
        /// <summary>
        /// RSV30Z 保留字 30 C(30)
        /// </summary>
        public string RSV30Z { get; set; }
    }
    /// <summary>
    /// 3.3 支付结果列表查询 
    /// NTQRYSTNY1 输入接口 1
    /// </summary>
    public class NTQRYSTNY1
    {
        /// <summary>
        /// BUSCOD 业务类型 C(6) N02020: 内部转帐N02030: 支付N02031: 直接支付N02040: 集团支付N02041: 直接集团支
        /// </summary>
        public string BUSCOD { get; set; }
        /// <summary>
        /// BUSMOD 业务模式 C(5) 否 指定的单一模式如 00001
        /// </summary>
        public string BUSMOD { get; set; }
        /// <summary>
        /// BGNDAT 起始日期 D 否 yyyyMMdd
        /// </summary>
        public string BGNDAT { get; set; }
        /// <summary>
        /// ENDDAT 结束日期 D 否 与结束日期的间隔不能超过 100 天 yyyyMMdd
        /// </summary>
        public string ENDDAT { get; set; }
        /// <summary>
        ///DATFLG 日期类型 C(1) A：经办日期；B：期望日期可 默认为 A
        /// </summary>
        public string DATFLG { get; set; }
        /// <summary>
        /// RSV50Z 保留字50 C(50) 可
        /// </summary>
        public string HGHAMT { get; set; }
    }
    /// <summary>
    /// NTSTLINFX 查 询 支付 结果输入接口否 1..500
    /// </summary>
    public class NTSTLINFX
    {
        /// <summary>
        /// REQNBR 流程实例号 C(10) 否
        /// </summary>
        public string REQNBR { get; set; }
    }
    /// <summary>
    /// NTQPAYQYZ 查 询 支付 结果输出接口可 1..500
    /// </summary>
    public class NTQPAYQYZ
    {
        /// <summary>
        /// REQNBR 流程实例号 C(10) 
        /// </summary>
        public string REQNBR { get; set; }
        /// <summary>
        ///BUSCOD 业务代码 C(6) 附录 A.4 否
        /// </summary>
        public string BUSCOD { get; set; }
        /// <summary>
        /// BUSMOD 业务模式 C(5) 
        /// </summary>
        public string BUSMOD { get; set; }
        /// <summary>
        /// DBTBBK 转出分行号 C(2) 附录 A.1
        /// </summary>
        public string DBTBBK { get; set; }
        /// <summary>
        /// DBTACC 付方帐号 C(35) 
        /// </summary>
        public string DBTACC { get; set; }
        /// <summary>
        /// DBTNAM 付方帐户名 C(200) 否企业用于付款的转出帐号的户名
        /// </summary>
        public string DBTNAM { get; set; }
        /// <summary>
        /// DBTBNK 付方开户行 Z(62) 否企业用于付款的转出帐号的开户行名称，如：招商银行北京分行
        /// </summary>
        public string DBTBNK { get; set; }
        /// <summary>
        /// DBTADR 付方行地址 Z(62)
        /// </summary>
        public string DBTADR { get; set; }
        /// <summary>
        /// CRTBBK 收方分行号 C(2) 附录 A.1
        /// </summary>
        public string CRTBBK { get; set; }
        /// <summary>
        /// CRTACC 收方帐号 C(35) 
        /// </summary>
        public string CRTACC { get; set; }
        /// <summary>
        /// CRTNAM 收方名称 Z(122)
        /// </summary>
        public string CRTNAM { get; set; }
        /// <summary>
        /// RCVBRD 收方大额行号C(12) 二代支付
        /// </summary>
        public string RCVBRD { get; set; }
        /// <summary>
        /// CRTBNK 收方开户行 Z(62) 可收方帐号的开户行名称，如：招商
        /// </summary>
        public string CRTBNK { get; set; }
        /// <summary>
        /// GRPBBK 母公司开户地区代码 C(2)
        /// </summary>
        public string GRPBBK { get; set; }
        /// <summary>
        /// GRPACC 母公司帐号 C(35
        /// </summary>
        public string GRPACC { get; set; }
        /// <summary>
        /// CCYNBR 币种代码 N(2)
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        ///GRPNAM 母公司帐
        /// </summary>
        public string GRPNAM { get; set; }
        /// <summary>
        /// TRSAMT 交易金额 M
        /// </summary>
        public string TRSAMT { get; set; }
        /// <summary>
        /// EPTDAT 期望日
        /// </summary>
        public string EPTDAT { get; set; }
        /// <summary>
        /// EPTTIM 期望时间 T
        /// </summary>
        public string EPTTIM { get; set; }
        /// <summary>
        /// BNKFLG 系统内外标志C(1)“Y”表示系统内，“N”表示系统外 可表示该笔业务是否为招行系统内的支付结算业
        /// </summary>
        public string BNKFLG { get; set; }
        /// <summary>
        /// REGFLG 同城异地标志C(1) Y”表示同城业务；“N”表示异地业务 可表示该笔业务是否为同城业
        /// </summary>
        public string REGFLG { get; set; }
        /// <summary>
        /// STLCHN 结算方式代码C(1) N-普通；F-快速
        /// </summary>
        public string STLCHN { get; set; }
        /// <summary>
        /// NUSAGE 用途 Z(28)
        /// </summary>
        public string NUSAGE { get; set; }
        /// <summary>
        /// NTFCH1 收方电子邮件
        /// </summary>
        public string NTFCH1 { get; set; }
        /// <summary>
        /// NTFCH2 收方移动电话C(16
        /// </summary>
        public string NTFCH2 { get; set; }
        /// <summary>
        /// OPRDAT 经办日期 
        /// </summary>
        public string OPRDAT { get; set; }

        /// <summary>
        /// YURREF 业务参考号 C(30
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// BUSNAR 业务摘要 Z(196) 可用于企业付款时填写说明或者备
        /// </summary>
        public string BUSNAR { get; set; }
        /// <summary>
        /// REQSTS 业务请求状态代码 C(3) 附录 A.5 否
        /// </summary>
        public string REQSTS { get; set; }
        /// <summary>
        /// RTNFLG 业务处理结果代码 C(1) 附录 A.6
        /// </summary>
        public string RTNFLG { get; set; }
        /// <summary>
        /// OPRALS 操作别名 Z(28) 可待处理的操作名称。
        /// </summary>
        public string OPRALS { get; set; }
        /// <summary>
        /// RTNNAR 结果摘要 Z(88) 可支付结算业务处理的结果描述，如失败原因、退票原
        /// </summary>
        public string RTNNAR { get; set; }
        /// <summary>
        /// RTNDAT 退票日期 D 可
        /// </summary>
        public string RTNDAT { get; set; }
        /// <summary>
        /// ATHFLG 是否有附件信息 C(1) “Y”表示有附件，“N”表示无附件 可
        /// </summary>
        public string ATHFLG { get; set; }
        /// <summary>
        /// LGNNAM 经办用户登录名 Z(30) 可
        /// </summary>
        public string LGNNAM { get; set; }
        /// <summary>
        /// USRNAM 经办用户姓名Z(30) 可
        /// </summary>
        public string USRNAM { get; set; }
        /// <summary>
        /// TRSTYP 业务种类 C(6
        /// </summary>
        public string TRSTYP { get; set; }
        /// <summary>
        /// FEETYP 收费方式 C(1) N = 不收费Y = 收费
        /// </summary>
        public string FEETYP { get; set; }
        /// <summary>
        /// RCVTYP 收方公私标志C(1) A=对公P = 个人X=信用卡可
        /// </summary>
        public string RCVTYP { get; set; }
        /// <summary>
        /// BUSSTS 汇款业务状态C(1) A =待提出C=已撤销D =已删除P =已提出R=已退票W=待处理（待确认
        /// </summary>
        public string BUSSTS { get; set; }
        /// <summary>
        /// TRSBRN 受理机构 C(6) 可
        /// </summary>
        public string TRSBRN { get; set; }
        /// <summary>
        /// TRNBRN 转汇机构 C(6) 可
        /// </summary>
        public string TRNBRN { get; set; }
        /// <summary>
        /// RSV30Z 保留字段 C(30)虚拟户支付时前十位为虚拟户编
        /// </summary>
        public string RSV30Z { get; set; }

    }

    /// <summary>
    /// DCOPRTRFX 支 付 输入 明细接口否 1..30 批量内转最大笔数为 30
    /// </summary>
    public class DCOPRTRFX
    {
        /// <summary>
        /// YURREF 业务参考号 C（30） 否
        /// 用于标识该笔业务的编号，企业银行编号+业务类型+业务参考号必须唯一。
        /// 企业可以自定义业务参考号，也可使用银行缺省值（单笔支付），
        /// 批量支付须由企业提供。直联必须用企业提供
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// EPTDAT 期望日 D 默认为当前日期 可
        /// </summary>
        public string EPTDAT { get; set; }
        /// <summary>
        /// EPTTIM 期望时间 T 默认为‘00000
        /// </summary>
        public string EPTTIM { get; set; }
        /// <summary>
        /// DBTACC 付方帐号 N（35） 否
        /// </summary>
        public string DBTACC { get; set; }
        /// <summary>
        /// DBTBBK 付方开户地区代码 付方帐号的开户行所在地区，如北京、上海、深圳等付方开户地区和付方开户地区代码不能同时为空，同 时 有 值 时DBTBBK 有效
        /// </summary>
        public string DBTBBK { get; set; }
        /// <summary>
        /// TRSAMT 交易金额 M 否
        /// </summary>
        public decimal TRSAMT { get; set; }
        /// <summary>
        /// CCYNBR 币种代码 币种代码和币种名称不能同时为空同时有值时CCYNBR 
        /// 有效。。币种暂时只支持 10(人民币)
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        /// STLCHN 结算方式代码C(1)N：普通F：快速 否只对跨行交易有效
        /// </summary>
        public string STLCHN { get; set; }
        /// <summary>
        /// NUSAGE 用途 Z（62） 否对应对账单中的摘要 NARTXT
        /// </summary>
        public string NUSAGE { get; set; }
        /// <summary>
        /// BUSNAR 业务摘要 Z（200）可用于企业付款时填写说明或者备注。
        /// </summary>
        public string BUSNAR { get; set; }
        /// <summary>
        /// CRTACC 收方帐号 N（35）否
        /// </summary>
        public string CRTACC { get; set; }
        /// <summary>
        /// 收款方企业的转入帐号的帐户名称。
        /// CRTNAM 收方帐户名 Z（62） 可收方帐户名与收方长户名不能同时为空
        /// </summary>
        public string CRTNAM { get; set; }
        /// <summary>
        /// LRVEAN 收方长户名 Z(200) 可
        /// </summary>
        public string LRVEAN { get; set; }
        /// <summary>
        /// BRDNBR 收方行号 C(30) 可人行自动支付收方联行号
        /// </summary>
        public string BRDNBR { get; set; }
        /// <summary>
        /// BNKFLG 系统内外标志Y：招行；N：非招行否
        /// </summary>
        public string BNKFLG { get; set; }
        /// <summary>
        /// CRTBNK 收方开户行 Z（62）跨 行 支 付（BNKFLG=N）必
        /// </summary>
        public string CRTBNK { get; set; }
        /// <summary>
        /// CTYCOD 城市代码 C(4)附录 CRTFLG 不为 Y 时
        /// 行内支付必填行内支付填写，为空则不支持收方识别功能。
        /// </summary>
        public string CTYCOD { get; set; }
        /// <summary>
        /// CRTADR 收方行地址 Z(62)跨 行 支 付（BNKFLG=N）必填；CRTFLG 不为 Y时行内支付必
        /// </summary>
        public string CRTADR { get; set; }
        /// <summary>
        /// CRTFLG 收方信息不检查标志 C(1)Y: 行内支付不检查城市代码和收方行地址默认为 Y
        /// </summary>
        public string CRTFLG { get; set; }
        /// <summary>
        /// NTFCH1 收方电子邮件C（36） 可收款方的电子邮件地址，用于交易成功后邮件通知
        /// </summary>
        public string NTFCH1 { get; set; }
        /// <summary>
        /// NTFCH2 收方移动电话C（16） 可收款方的移动电话，用于交易 成功后短信通知
        /// </summary>
        public string NTFCH2 { get; set; }
        /// <summary>
        /// CRTSQN 收方编号 C（20） 可用于标识收款方的编号。非受限收方模式下可重复。
        /// </summary>
        public string CRTSQN { get; set; }
        /// <summary>
        /// TRSTYP 业务种类 C(6)100001=普通汇兑101001= 慈善捐款101002 =其他 默认 100001
        /// </summary>
        public string TRSTYP { get; set; }
        /// <summary>
        /// RSV28Z 保留字段 C(27) 可虚拟户支付时，前10 位填虚拟户编号；集团支付不支持虚拟户支
        /// </summary>
        public string RSV28Z { get; set; }
        /// <summary>
        /// RCVCHK  行内收方账号户名校验 C(1)1：校验空或者其他值：
        /// 不校验可如果为 1，行内收方账号与户名不相符则支付经办失败。
        /// </summary>
        public string RCVCHK { get; set; }
    }
    /// <summary>
    /// 单笔移动支票接口输入头
    /// </summary>
    public class NTOPRMODX {
        public string BUSMOD { get; set; }
    }
    /// <summary>
    /// 单笔移动支票接口输入体
    /// </summary>
    public class NTECKOPRX {
        /// <summary>
        /// YURREF 业务参考号 C(30) 否
        /// </summary>
        public string YURREF { get; set; }
        /// <summary>
        /// BBKNBR 分行号 C(2) A.1 招行分行 否
        /// </summary>
        public string BBKNBR { get; set; }
        /// <summary>
        /// EACNBR 账号 C(35) 否
        /// </summary>
        public string EACNBR { get; set; }
        /// <summary>
        /// CCYNBR 币种 C(2) A.3 货币代码表 否
        /// </summary>
        public string CCYNBR { get; set; }
        /// <summary>
        /// MAXAMT 金额上限 C(15) 否
        /// </summary>
        public decimal MAXAMT { get; set; }
        /// <summary>
        /// EFTDAT 生效日期 C(8) 否 yyyyMMdd
        /// </summary>
        public string EFTDAT { get; set; }
        /// <summary>
        /// EXPDAT 失效日期 C(8) 否 yyyyMMdd
        /// </summary>
        public string EXPDAT { get; set; }
        /// <summary>
        /// ADDDAT 托收日期 C(8) 否
        /// </summary>
        public string ADDDAT { get; set; }
        /// <summary>
        /// AUTUSR 授权使用人 C(10)否输入用户号，移动支票查询可选有权使用人列表可以查询
        /// </summary>
        public string AUTUSR { get; set; }
        /// <summary>
        ///ECKNBR 支票号 C(10)
        /// </summary>
        public string ECKNBR { get; set; }
        /// <summary>
        /// RSV50Z 虚拟户账号 C（50）第一位固定为 Y，第2 到 11 位为虚拟户账 号 ： 如Y1234567890
        /// </summary>
        public string RSV50Z { get; set; }
    }
    /// <summary>
    /// 单笔移动支票接口输入（收方信息）
    /// </summary>
    public class NTECKRCVX {
        /// <summary>
        /// SQRNBR 流水号 C(10)否
        /// </summary>
        public string SQRNBR { get; set; }
        /// <summary>
        /// RCVACC 收方账号 C(35) 否
        /// </summary>
        public string RCVACC { get; set; }
        /// <summary>
        /// RCVNAM 收方户名 Z(122) 否
        /// </summary>
        public string RCVNAM { get; set; }
        /// <summary>
        /// SYSFLG 系统内外标志 C(1)否 系统内：Y 系统外：N
        /// </summary>
        public string SYSFLG { get; set; }
        /// <summary>
        /// PAYCHN 支付汇路 C(3) CPS/NPS 否
        /// CPS:他行普通和行内 NPS：他行实时
        /// </summary>
        public string PAYCHN { get; set; }
        /// <summary>
        /// CDTBRD 收方行号 C(30) 当汇路是 NPS 时必填
        /// </summary>
        public string CDTBRD { get; set; }
        /// <summary>
        /// RCVBBK 收方行名称 Z(200) 否
        /// </summary>
        public string RCVBBK { get; set; }
        /// <summary>
        ///BNKADR CPS 收方行地址 Z(62)汇路是 CPS 
        ///并且是系统外时必输支持省简写，如河南郑州，河北石家庄，重庆
        /// </summary>
        public string BNKADR { get; set; }
        /// <summary>
        /// STLCHN CPS 结算通道 C(1) F/N 汇路是 CPS 时必输F：快速；N：普通
        /// </summary>
        public string STLCHN { get; set; }
        /// <summary>
        /// NUSAGE CPS 用途 Z(62) 汇路是 CPS 时必输
        /// </summary>
        public string NUSAGE { get; set; }
        /// <summary>
        /// BUSNAR NPS 附言 Z(200) 汇路是 NPS 时必输
        /// </summary>
        public string BUSNAR { get; set; }
        /// <summary>
        /// RCVNTF 收款人手机 C(20) 
        /// </summary>
        public string RCVNTF { get; set; }
    }
    /// <summary>
    /// 一事通 设置
    /// </summary>
    public class NTECKMKTX {
        /// <summary>
        /// USRNBR 一事通 C(15)
        /// </summary>
        public string USRNBR { get; set; }
        /// <summary>
        /// USRNAM 推荐人姓名 Z(62)
        /// </summary>
        public string USRNAM { get; set; }
        /// <summary>
        /// MBLNBR 手机号码 C(20)
        /// </summary>
        public string MBLNBR { get; set; }
        /// <summary>
        /// USRNB2 一事通 2 C(15)
        /// </summary>
        public string USRNB2 { get; set; }
        /// <summary>
        ///USRNA2 推荐人姓名2Z(62)   
        /// </summary>
        public string USRNA2 { get; set; }
        /// <summary>
        /// MBLNB2 手机号码 2 C(20)
        /// </summary>
        public string MBLNB2 { get; set; }
    }
    /// <summary>
    /// 移动支票单笔返回接口
    /// </summary>
    public class NTECKOPRZ {
        /// <summary>
        /// REQNBR 流程实例号 C(10)
        /// </summary>
        public string REQNBR { get; set; }
        /// <summary>
        /// ECKNBR 支票编号 C(10)
        /// </summary>
        public string ECKNBR { get; set; }
        /// <summary>
        /// REQSTS 请求状态 C(3) 
        /// </summary>
        public string REQSTS { get; set; }
        /// <summary>
        /// OPRSQN 待处理操作序列C(3)
        /// </summary>
        public string OPRSQN { get; set; }
        /// <summary>
        /// OPRALS 操作别名 Z(32)
        /// </summary>
        public string OPRALS { get; set; }
        /// <summary>
        /// RTNFLG 业务处理结果 C(1) 
        /// </summary>
        public string RTNFLG { get; set; }
        /// <summary>
        /// ERRCOD 错误码 C(7) 
        /// </summary>
        public string ERRCOD { get; set; }
        /// <summary>
        /// ERRTXT 错误文本 Z(92)
        /// </summary>
        public string ERRTXT { get; set; }
    }
    /// <summary>
    /// 移动支票查询可选有权使用人列表
    /// </summary>
    public class NTECKUSRZ {
        /// <summary>
        /// USRNBR 用户号 C(10) 
        /// </summary>
        public string USRNBR { get; set; }
        /// <summary>
        /// LGNNAM 登录名 Z(3
        /// </summary>
        public string LGNNAM { get; set; }
        /// <summary>
        /// USRNAM 用户姓名 Z(20)
        /// </summary>
        public string USRNAM { get; set; }
        /// <summary>
        /// USRTYP 用户类型 C(1) 
        /// </summary>
        public string USRTYP { get; set; }
        /// <summary>
        /// CORPST 职务编号 C(
        /// </summary>
        public string CORPST { get; set; }
        /// <summary>
        /// RELNBR 客户关系号 C(10)
        /// </summary>
        public string RELNBR { get; set; }
        /// <summary>
        /// USRSTS 用户状态 C(1)
        /// </summary>
        public string USRSTS { get; set; }
    }
    /// <summary>
    /// 业务权限
    /// </summary>
    public class SDKMDLSTX {
        /// <summary>
        /// BUSCOD 业务类别 C(6) 附录 A.4 否
        /// </summary>
        public string BUSCOD { get; set; }
    }
    public class NTECKTQYY {
        /// <summary>
        /// BEGDAT 起始日期 D否
        /// </summary>
        public string BEGDAT { get; set; }
        /// <summary>
        /// ENDDAT 结束日期 D否 起始结束日期不能超过 7 天
        /// </summary>
        public string ENDDAT { get; set; }
        /// <summary>
        /// ECKNBR 支票号码 C(10)
        /// </summary>
        public string ECKNBR { get; set; }
    }
    /// <summary>
    /// 移动支票查询返回查询接口
    /// </summary>
    public class NTECKTQYZ
    {
        /// <summary>
        /// TRSDAT 支付日期 D
        /// </summary>
        public string TRSDAT { get; set; }
        /// <summary>
        /// TRSTIM 支付时间 T
        /// </summary>
        public string TRSTIM { get; set; }

        /// <summary>
        /// TRSSTS 支付状态 C(1) S：成功（已汇出B：已退票F：失败I：支付中
        /// </summary>
        public string TRSSTS { get; set; }

        /// <summary>
        /// ERRINF 错误信息 Z(200)
        /// </summary>
        public string ERRINF { get; set; }
        
        /// <summary>
        /// RCVACC 收方账号 C(35)
        /// </summary>
        public string RCVACC { get; set; }

        /// <summary>
        /// RCVNAM 收方户名 Z(122)
        /// </summary>
        public string RCVNAM { get; set; }

        /// <summary>
        /// TRSAMT 交易金额 M
        /// </summary>
        public decimal TRSAMT { get; set; }

        /// <summary>
        /// RCVBNK 收方行名称 Z(200)
        /// </summary>
        public string RCVBNK { get; set; }
        /// <summary>
        /// BNKADR CPS 收 方 行 地址Z(62)
        /// </summary>
        public string BNKADR { get; set; }
        /// <summary>
        /// STLCHN CPS 结算通道 C(
        /// </summary>
        public string STLCHN { get; set; }
        /// <summary>
        /// SYSFLG 是否行内 C(1)
        /// </summary>
        public string SYSFLG { get; set; }
        /// <summary>
        /// PAYCHN 支付汇路 C(3) CPS：他行普通
        /// </summary>
        public string PAYCHN { get; set; }
        /// <summary>
        /// BUSTXT 用途或者附言 Z(200)
        /// </summary>
        public string BUSTXT { get; set; }
        /// <summary>
        /// NTFNBR 收款人手机 C(20)
        /// </summary>
        public string NTFNBR { get; set; }
        /// <summary>
        /// REQNBR 交易请求号 C(1
        /// </summary>
        public string REQNBR { get; set; }
        /// <summary>
        /// ECKNBR 支票编号 C(1
        /// </summary>
        public string ECKNBR { get; set; }
        /// <summary>
        /// EACNBR 付款账号 C(3
        /// </summary>
        public string EACNBR { get; set; }
    }
    /// <summary>
    /// 招行请求接口
    /// </summary>
    public class CMBSDKPGK
    {
        /// <summary>
        /// xml头部
        /// </summary>
        public INFO INFO { get; set; }
        /// <summary>
        ///账 户 详细 信息查询输入接口否 N 支持多账户查询(多号查询需要转List，目前只做单独查询)
        /// </summary>
        public SDKACINFX SDKACINFX { get; set; }
        /// <summary>
        /// 账 户 详细 信息查询输出接口否 N 支持多账户查询(多号查询需要转List，目前只做单独查询)
        /// </summary>
        public NTQACINFZ NTQACINFZ { get; set; }
        /// <summary>
        /// 支付
        /// </summary>
        public CSRRCFDFY0 CSRRCFDFY0 { get; set; }
        /// <summary>
        /// 账 户 交易 信息查询输入接口
        /// </summary>
        public SDKTSINFX SDKTSINFX { get; set; }
        /// <summary>
        /// 账 户 交易 信息查询输出接口可 0 或者 n n 为账户交易信息的实际条数。无交易或者查询错误时无此接口。
        /// </summary>
        public List<NTQTSINFZ> NTQTSINFZ { get; set; }
        /************3.6直接支付****************/
        /// <summary>
        /// 支付输入概要接口否
        /// </summary>
        public SDKPAYRQX SDKPAYRQX { get; set; }
        /// <summary>
        /// DCOPDPAYX 支 付 输入 明细接口否 1..30 或 者30..1500支付条数不超过 30 条，支付输出有NTQPAYRQZ 数据；超过 30 条，则无。
        /// </summary>
        public DCOPDPAYX DCOPDPAYX { get; set; }
        /// <summary>
        /// DCOPRTRFX 支 付 输入 明细接口否 1..30 批量内转最大笔数为 30
        /// </summary>
        public DCOPRTRFX DCOPRTRFX { get; set; }
        /// <summary>
        /// NTQPAYRQZ 支付输出接口 可 1．.30
        /// </summary>
        public NTQPAYRQZ NTQPAYRQZ { get; set; }
        /// <summary>
        /// SDKPAYDTX 支 付 输入 明细接口 否 1..30 批量内转最大笔数为 30
        /// </summary>
        public SDKPAYDTX SDKPAYDTX { get; set; }
        /// <summary>
        /// NTQRYSTYX1 输入接口 1
        /// </summary>
        public NTQRYSTYX1 NTQRYSTYX1 { get; set; }
        /// <summary>
        /// NTSTLLSTZ 输出接口(多记录)n n 为查询的实际结果数
        /// </summary>
        public List<NTSTLLSTZ> NTSTLLSTZ { get; set; }
        /// <summary>
        /// NTSTLINFX 查 询 支付 结果输入接口否 1..500
        /// </summary>
        public NTSTLINFX NTSTLINFX { get; set; }
        /// <summary>
        /// NTQPAYQYZ 查 询 支付 结果输出接口可 1..500
        /// </summary>
        public NTQPAYQYZ NTQPAYQYZ { get; set; }
        /// <summary>
        /// NTQRYSTNY1 输入接口 1
        /// </summary>
        public NTQRYSTNY1 NTQRYSTNY1 { get; set; }
        /*******移动支票*********/
        /// <summary>
        /// NTOPRMODX 输入接口 1
        /// </summary>
        public NTOPRMODX NTOPRMODX  { get; set; }
        /// <summary>
        /// NTECKOPRX 输入接口 否 1
        /// </summary>
        public NTECKOPRX NTECKOPRX { get; set; }
        /// <summary>
        /// NTECKRCVX 输入接口 可 1
        /// </summary>
        public NTECKRCVX NTECKRCVX { get; set; }
        /// <summary>
        /// NTECKMKTX 输入接口 可 1
        /// </summary>
        public NTECKMKTX NTECKMKTX { get; set; }
        /// <summary>
        /// NTECKOPRZ 输出接口 否 1
        /// </summary>
        public NTECKOPRZ NTECKOPRZ { get; set; }
        /// <summary>
        /// 移动支票查询可选有权使用人列表 
        /// </summary>
        public List<NTECKUSRZ> NTECKUSRZ { get; set; }
        public SDKMDLSTX SDKMDLSTX { get; set; }
        /// <summary>
        /// 移动支票查询
        /// </summary>
        public NTECKTQYY NTECKTQYY { get; set; }
        /// <summary>
        /// 移动支票查询返回接口
        /// </summary>
        public List<NTECKTQYZ> NTECKTQYZ { get; set; }
    }
    /// <summary>
    /// 返回接口
    /// </summary>
    public class ResonseClass
    {
        public CMBSDKPGK CMBSDKPGK { get; set; }
    }
}
