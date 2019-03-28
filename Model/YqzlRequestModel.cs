using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using model;

namespace Model
{
    public class YqzlRequestModel
    {
        /// <summary>
        /// 支付/收款 C/D
        /// </summary>
        public String isPay { get; set; }
        /// <summary>
        /// U8银行
        /// </summary>
        public Content content { get; set; }
        /// <summary>
        /// 请求编码 4013
        /// </summary>
        public string bsnCode { get; set; }
        /// <summary>
        /// AcctNo	账号	Char(20)	Y	
        /// </summary>
        public string AcctNo { get; set; }
        /// <summary>
        /// CcyCode	币种	Char(3)	Y	
        /// </summary>
        public string CcyCode { get; set; }
        /// <summary>
        /// BeginDate	开始日期	Char(8)	Y	若查询当日明细，开始、结束日期必须为当天；若查询历史明细，开始、结束日期必须是历史日期。
        /// </summary>
        public DateTime BeginDate { get; set; }
        /// <summary>
        /// EndDate	结束日期	Char(8)	Y	
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// PageNo	查询页码	Char(6)	Y	1：第一页，依次递增
        /// </summary>
        public int PageNo { get; set; }
        /// <summary>
        /// PageSize	每页明细数量	Char(6)	N	当日明细默认每页30条记录，支持最大每页100条，若上送PageSize>100无效，等同100；
        /// 历史明细默认每页30条记录，支持最大每页1000条，若上送PageSize>1000则提示输入错误；
        /// 且每次查询必须固定为此值，否则出现明细遗漏
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Reserve	预留字段	Char(120)		
        /// </summary>
        public string Reserve { get; set; }
        /// <summary>
        /// OrderMode	记录排序标志	C(3)	N	001：按交易时间降序；002：按交易时间升序；
        ///说明：
        ///①当为历史交易明细查询时，默认按照001：按交易时间降序；
        ///②当为当日明细查询时，默认按照002：按交易时间升序；
        ///（注：当日明细在交易量大的情况下，必须采用正序查询，否则会导致交易遗漏和重复）
        /// </summary>
        public string OrderMode { get; set; }
    }
}
