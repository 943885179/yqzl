using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using common;
using EntityFromework;
using Log;
using Newtonsoft.Json;
using PinganYqzl.model;
namespace PinganYqzl
{
    public class PinganApi
    {
        private static string url = ConfigurationManager.AppSettings["PinganUrl"].ToString();//"http://127.0.0.1:7072";//配置地址
        public string yqdm = ConfigurationManager.AppSettings["Pingan"].ToString();//银企代码 00101088100008043000 00303079800000117000
        /// <summary>
        /// 余额查询交易(qryBal) 4001
        /// </summary>
        /// <param name="Account">帐号:11002923034501</param>
        /// <param name="CcyCode">货币类型</param>
        /// <returns></returns>
        public string qryBal(string Account, string bsnCode = "4001", string CcyCode = "RMB")
        {
            string xml = "<?xml version=\"1.0\" encoding=\"GBK\"?><Result><Account>" + Account + "</Account><CcyCode>" + CcyCode + "</CcyCode></Result>";
            var data = YQUntil.asemblyPackets(yqdm, bsnCode, xml);
            var result = HttpHelps.HttpPost(url, data);
            return result;
        }
        /// <summary>
        /// 当日明细（4002）
        /// </summary>
        /// <param name="Account">帐号:11002923034501</param>
        /// <param name="CcyCode"></param>
        /// <returns></returns>
        public string qryDtlToday(string Account, string bsnCode = "4002", string CcyCode = "RMB")
        {
            string xml = "<?xml version=\"1.0\" encoding=\"GBK\"?><Result><Account>" + Account + "</Account><CcyCode>" + CcyCode + "</CcyCode></Result>";
            var data = YQUntil.asemblyPackets(yqdm, bsnCode, xml);
            var result = HttpHelps.HttpPost(url, data);
            return result;
        }
        /// <summary>
        /// 3.5单笔转账指令查询[4005]
        /// </summary>
        /// <param name="OrigThirdVoucher">交易流水号（推荐使用；使用4004接口上送的ThirdVoucher或者4014上送的SThirdVoucher）</param>
        /// <param name="OrigFrontLogNo">银行流水号	C(32)	非必输	不推荐使用；银行返回的转账流水号</param>
        /// <param name="CcyCode"></param>
        /// <returns></returns>
        public PinganResponse<XferResponseModel> qryDtlByOrig(string OrigThirdVoucher, string OrigFrontLogNo = "", string bsnCode = "4005", string CcyCode = "RMB")
        {
            using (var db = new OAtoU8DATAEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var paymentRecord = db.yq_paymentRecord.Where(o => o.thirdVoucher == OrigThirdVoucher).FirstOrDefault();
                        if (paymentRecord == null)
                        {
                            return new PinganResponse<XferResponseModel>() { message = "交易未发生或者非本系统发生交易" };
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
                        sb.Append("<Result>");
                        if (!string.IsNullOrEmpty(OrigThirdVoucher))
                        {//企业设定的唯一流水号
                            sb.Append("<OrigThirdVoucher>" + OrigThirdVoucher + "</OrigThirdVoucher>");
                        }
                        else if (!string.IsNullOrEmpty(OrigFrontLogNo))
                        {//不建议使用此参数，查询有时候查不出来，不懂为什么
                            sb.Append("<OrigFrontLogNo>" + OrigFrontLogNo + "</OrigFrontLogNo>");
                        }
                        else
                        {
                            return new PinganResponse<XferResponseModel>() { message = "交易流水号不能为空" };
                        }
                        sb.Append("</Result>");
                        var data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());
                        LogHelper.WriteLog("开始请求付款。。。。\n", "yqzl");
                        var result = HttpHelps.HttpPost(url, data);
                        string resultCode = result.Substring(87, 6);
                        LogHelper.WriteLog("请求：4004\n请求参数：" + data + "\n返回参数：\n" + result, "yqzl");

                        PinganResponse<XferResponseModel> response = new PinganResponse<XferResponseModel> { code = resultCode };
                        yq_log log = new yq_log();
                        log.createTime = DateTime.Now;
                        log.ip = GetIP.GetWebClientIp();
                        log.message = "交易查询4001";
                        log.requestXml = data;
                        log.responseXml = result;
                        log.yqCode = "4001";
                        log.userName = "微笑";
                        db.yq_log.Add(log);
                        db.SaveChanges();
                        if (resultCode == "000000")
                        {//受理成功
                            var resultXml = result.Substring(253, result.Length - 253);
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(resultXml);
                            string jsontext = JsonConvert.SerializeXmlNode(doc);
                            jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                            XferResponseModel rModel = JsonConvert.DeserializeObject<XferResponseModel>(jsontext);
                            response.result = rModel;
                            paymentRecord.frontLogNo = rModel.FrontLogNo;
                            paymentRecord.unionFlag = rModel.UnionFlag;
                            paymentRecord.stt = rModel.stt;
                            paymentRecord.fee1 = rModel.Fee1;
                            paymentRecord.fee2 = rModel.Fee2;
                            paymentRecord.hostFlowNo = rModel.hostFlowNo;
                            paymentRecord.hostTxDate = rModel.hostTxDate;
                            paymentRecord.isBack = rModel.isBack;
                            paymentRecord.backRem = rModel.backRem;
                            db.SaveChanges();
                        }
                        tran.Commit();
                        return response;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new PinganResponse<XferResponseModel>() { message = ex.ToString() };
                    }
                }
            }
        }

        /// <summary>
        /// 3.5单笔转账指令查询[4005]
        /// </summary>
        /// <param name="OrigThirdVoucher">交易流水号（推荐使用；使用4004接口上送的ThirdVoucher或者4014上送的SThirdVoucher）</param>
        /// <returns></returns>
        public PinganResponse<XferResponseModel> qryDtlByOrig(XferRequestModel requestModel, string bsnCode = "4005")
        {
            using (var db = new OAtoU8DATAEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //查询记过银行账的交易记录
                        var paymentRecord = db.yq_paymentRecord.Where(o => o.thirdVoucher == requestModel.ThirdVoucher && o.hostFlowNo != null).FirstOrDefault();
                        if (paymentRecord == null)
                        {
                            return new PinganResponse<XferResponseModel>() { message = "交易未发生或者非本系统发生交易" };
                        }
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
                        sb.Append("<Result>");
                        if (!string.IsNullOrEmpty(requestModel.ThirdVoucher))
                        {//企业设定的唯一流水号
                            sb.Append("<OrigThirdVoucher>" + requestModel.ThirdVoucher + "</OrigThirdVoucher>");
                        }
                        else
                        {
                            return new PinganResponse<XferResponseModel>() { message = "交易流水号不能为空" };
                        }
                        sb.Append("</Result>");
                        var data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());
                        LogHelper.WriteLog("开始请求付款。。。。\n", "yqzl");
                        var result = HttpHelps.HttpPost(url, data);
                        string resultCode = result.Substring(87, 6);
                        LogHelper.WriteLog("请求：4004\n请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
                        PinganResponse<XferResponseModel> response = new PinganResponse<XferResponseModel> { code = resultCode };
                        yq_log log = new yq_log();
                        log.createTime = DateTime.Now;
                        log.ip = GetIP.GetWebClientIp();
                        log.message = "单笔转账指令查询4005";
                        log.requestXml = data;
                        log.responseXml = result;
                        log.yqCode = "4005";
                        log.userName = requestModel.createMan;
                        db.yq_log.Add(log);
                        db.SaveChanges();
                        if (resultCode == "000000")
                        {//受理成功
                            var resultXml = result.Substring(253, result.Length - 253);
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(resultXml);
                            string jsontext = JsonConvert.SerializeXmlNode(doc);
                            jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                            XferResponseModel rModel = JsonConvert.DeserializeObject<XferResponseModel>(jsontext);
                            paymentRecord.frontLogNo = rModel.FrontLogNo;
                            paymentRecord.unionFlag = rModel.UnionFlag;
                            paymentRecord.stt = rModel.stt;
                            paymentRecord.isBack = rModel.isBack;
                            paymentRecord.backRem = rModel.backRem;
                            db.SaveChanges();
                            response.result = rModel;
                            response.message = "查询更新完成";
                        }
                        else
                        {
                            var err = db.yq_error.Where(o => o.errCode == resultCode).FirstOrDefault();
                            response.message = err == null ? "银行记录读取有误" : err.errMessage;
                        }
                        tran.Commit();
                        return response;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new PinganResponse<XferResponseModel>() { message = ex.ToString() };
                    }
                }
            }
        }
        /// <summary>
        /// 更新交易状态
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="response"></param>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <param name="bsnCode"></param>
        public void qryDtlByOrigNew(XferRequestModel requestModel, out PinganResponse<XferResponseModel> response,out string data,out string result, string bsnCode = "4005")
        {
            data = "";
            result = "";
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
                sb.Append("<Result>");
                if (!string.IsNullOrEmpty(requestModel.ThirdVoucher))
                {//企业设定的唯一流水号
                    sb.Append("<OrigThirdVoucher>" + requestModel.ThirdVoucher + "</OrigThirdVoucher>");
                }
                else
                {
                    response = new PinganResponse<XferResponseModel>() { message = "交易流水号不能为空" };
                }
                sb.Append("</Result>");
                data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());
                LogHelper.WriteLog("开始请求付款。。。。\n", "yqzl");
                result = HttpHelps.HttpPost(url, data);
                string resultCode = result.Substring(87, 6);
                LogHelper.WriteLog("请求：4004\n请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
                response = new PinganResponse<XferResponseModel> { code = resultCode };
                if (resultCode == "000000")
                {//受理成功
                    var resultXml = result.Substring(253, result.Length - 253);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultXml);
                    string jsontext = JsonConvert.SerializeXmlNode(doc);
                    jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                    XferResponseModel rModel = JsonConvert.DeserializeObject<XferResponseModel>(jsontext);
                    response.result = rModel;
                    response.message = "查询更新完成";
                }
            }
            catch (Exception ex)
            {
                response= new PinganResponse<XferResponseModel>() { message = ex.ToString() };
            }

        }
        /// <summary>
        ///企业单笔资金划转[4004]若收款方为平安银行保证金账户，
        ///B2B转账， B2C还未测试
        ///请使用交易码400406，输入输出同此接口。
        /// </summary>
        /// <param name="requestModel">转账信息</param>
        /// <param name="bsnCode">转账编号</param>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <param name="response"></param>
        public void xFer(XferRequestModel requestModel, out string data, out string result, out PinganResponse<XferResponseModel> response, string bsnCode = "4004")
        {
            data = "";
            result = null;
            try
            {
                requestModel.InAcctNo = requestModel.InAcctNo.Replace(" ", "").Replace("-", "");//账号自动过滤掉空格，-
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version =\"1.0\" encoding=\"GB2312\"?>");
                sb.Append("<Result>");
                sb.Append("<ThirdVoucher>" + requestModel.ThirdVoucher + "</ThirdVoucher>");
                sb.Append("<CstInnerFlowNo>" + requestModel.CstInnerFlowNo + "</CstInnerFlowNo>");
                sb.Append("<CcyCode>" + requestModel.CcyCode + "</CcyCode>");
                sb.Append("<OutAcctNo>" + requestModel.OutAcctNo + "</OutAcctNo>");
                sb.Append("<OutAcctName>" + requestModel.OutAcctName + "</OutAcctName>");
                sb.Append("<OutAcctAddr/><InAcctBankNode/><InAcctRecCode/>");
                sb.Append("<InAcctNo>" + requestModel.InAcctNo + "</InAcctNo>");
                sb.Append("<InAcctName>" + requestModel.InAcctName + "</InAcctName>");
                sb.Append("<InAcctBankName>" + requestModel.InAcctBankName + "</InAcctBankName>");
                sb.Append("<TranAmount>" + requestModel.TranAmount + "</TranAmount>");
                sb.Append("<AmountCode/>");
                sb.Append("<UseEx>" + requestModel.UseEx + "</UseEx>");
                sb.Append("<UnionFlag>" + requestModel.UnionFlag + "</UnionFlag>");
                sb.Append("<SysFlag>" + requestModel.SysFlag + "</SysFlag>");
                sb.Append("<AddrFlag>" + requestModel.AddrFlag + "</AddrFlag>");
                sb.Append("<RealFlag>2</RealFlag>");
                sb.Append("<MainAcctNo/></Result>");
                data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());   //return data;
                LogHelper.WriteLog("开始请求付款。。。。\n", "yqzl");
                result = HttpHelps.HttpPost(url, data);
                string resultCode = result.Substring(87, 6);
                var resultMessage = result.Substring(94, result.Substring(94).IndexOf(" "));
                LogHelper.WriteLog("请求：4004\n请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
                response = new PinganResponse<XferResponseModel> { code = resultCode, message = resultMessage };
                if (resultCode == "000000")
                {//受理成功
                    var resultXml = result.Substring(253, result.Length - 253);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultXml);
                    string jsontext = JsonConvert.SerializeXmlNode(doc);
                    jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                    XferResponseModel rModel = JsonConvert.DeserializeObject<XferResponseModel>(jsontext);
                    response.result = rModel;

                }
                else
                {
                    //  paymentRecord.errCode = resultCode;
                    //  paymentRecord.errMessage = resultMessage;
                    // var err = db.yq_error.Where(o => o.errCode == resultCode).FirstOrDefault();
                    // db.SaveChanges();
                    response.code = resultCode;
                    response.message = resultMessage;
                }
            }
            catch (Exception ex)
            {
                response = new PinganResponse<XferResponseModel>() { message = ex.ToString() };
            }

        }
        /// <summary>
        /// 4.1明细报表查询接口[F001]弃用改用F00101
        /// </summary>
        /// <param name="QueryDate">日期	C(8)	必输</param>
        /// <param name="Account">账号	C(20)	必输</param>
        /// <param name="BsnCodeSeach">BsnCode	交易码	C(6)	必输	4004、4014、4018、C004、C005、4047、4032</param>
        /// <param name="bsnCode">查询端口</param>
        /// <returns></returns>
        public string qryDtlXml(DateTime QueryDate, string Account, string BsnCodeSeach = "4004", string bsnCode = "F00101")
        {
            if (QueryDate == null)
            {
                QueryDate = DateTime.Now;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
            sb.Append("<Result>");
            sb.Append("<QueryDate>" + QueryDate.ToString("yyyyMMdd") + "</QueryDate>");
            sb.Append("<Account>" + Account + "</Account>");
            sb.Append("<BsnCode>" + bsnCode + "</BsnCode>");
            sb.Append("</Result>");
            var data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());
            var result = HttpHelps.HttpPost(url, data);
            return result;
        }
        /// <summary>
        /// 4.1明细报表查询接口[F001]弃用改用F00101
        /// </summary>
        /// <param name="QueryDate">日期	C(8)	必输</param>
        /// <param name="Account">账号	C(20)	必输</param>
        /// <param name="BsnCodeSeach">BsnCode	交易码	C(6)	必输	4004、4014、4018、C004、C005、4047、4032</param>
        /// <param name="bsnCode">查询端口</param>
        /// <returns></returns>
        public void qryDtlPdf(DateTime QueryDate, string Account, out string data, out string result, out PinganResponse<result> response, string BsnCodeSeach = "4004", string bsnCode = "F00101")
        {
            if (QueryDate == null)
            {
                QueryDate = DateTime.Now;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
            sb.Append("<Result>");
            sb.Append("<QueryDate>" + QueryDate.ToString("yyyyMMdd") + "</QueryDate>");
            sb.Append("<Account>" + Account + "</Account>");
            sb.Append("<BsnCode>" + bsnCode + "</BsnCode>");
            sb.Append("</Result>");
            data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());
            result = HttpHelps.HttpPost(url, data);
            if (result == null)
            {
                throw new Exception("请求错误，银行接口有误！接口错误或者银行暂时不在工作，请稍后...");
            }
            string resultCode = result.Substring(87, 6);
            LogHelper.WriteLog("读取回单交易明细,请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
            response = new PinganResponse<result> { code = resultCode };
            if (resultCode == "000000")
            {//受理成功
                var resultXml = result.Substring(253, result.Length - 253);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(resultXml);
                string jsontext = JsonConvert.SerializeXmlNode(doc);
                jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                if (jsontext.Contains("\"list\":{"))
                {//非数组改为数组
                    jsontext = jsontext.Replace("\"list\":{", "\"list\":[{");
                    jsontext = jsontext.Substring(0, jsontext.Length - 2) + "}]}";
                }
                result rModel = JsonConvert.DeserializeObject<result>(jsontext);
                response.result = rModel;
                response.code = "200";
            }
            else
            {
                response.message = "银行记录读取有误,错误位置，请查看日志...";
                response.code = "501";
            }
        }
        /// <summary>
        /// 3.8查询账户当日历史交易明细[4013]
        /// </summary>
        /// <param name="AcctNo">账号	Char(20)	Y</param>
        /// <param name="BeginDate"> 开始日期 Char(8) Y 若查询当日明细，开始、结束日期必须为当天；若查询历史明细，开始、结束日期必须是历史日期。</param>
        /// <param name="EndDate">结束日期    Char(8) Y</param>
        /// <param name="PageNo"> 查询页码 Char(6) Y	1：第一页，依次递增</param>
        /// <param name="PageSize">每页明细数量 Char(6) N 当日明细默认每页30条记录，支持最大每页100条，若上送PageSize>100无效，等同100；历史明细默认每页30条记录，支持最大每页1000条，若上送PageSize>1000则提示输入错误；</param>
        /// <param name="Reserve"> 预留字段 Char(120)</param>
        /// <param name="OrderMode">OrderMode 记录排序标志  C(3)    N	001：按交易时间降序；002：按交易时间升序①当为历史交易明细查询时，默认按照001：按交易时间降序；②当为当日明细查询时，默认按照002：按交易时间升序；（注：当日明细在交易量大的情况下，必须采用正序查询，否则会导致交易遗漏和重复）</param>
        /// <param name="CcyCode">CcyCode 币种  Char(3) Y</param>
        /// <param name="bsnCode"></param>
        /// <returns></returns>
        public PinganResponse<qryDtlResponse> qryDtlNow(string AcctNo, DateTime BeginDate, DateTime EndDate, int PageNo, int PageSize, string Reserve, string OrderMode = "001", string CcyCode = "RMB", string bsnCode = "4013")
        {
            using (var db = new OAtoU8DATAEntities())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
                        sb.Append("<Result>");
                        sb.Append("<AcctNo>" + AcctNo + "</AcctNo>");
                        sb.Append("<CcyCode>" + CcyCode + "</CcyCode>");
                        sb.Append("<BeginDate>" + BeginDate.ToString("yyyyMMdd") + "</BeginDate>");
                        sb.Append("<EndDate>" + EndDate.ToString("yyyyMMdd") + "</EndDate>");
                        sb.Append("<PageNo>" + PageNo + "</PageNo>");
                        sb.Append("<PageSize>" + PageSize + "</PageSize>");
                        sb.Append("<Reserve>" + Reserve + "</Reserve>");
                        sb.Append("<OrderMode>" + OrderMode + "</OrderMode>");
                        sb.Append("</Result>");
                        var data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());
                        LogHelper.WriteLog("请求交易记录。。。。\n", "yqzl");
                        var result = HttpHelps.HttpPost(url, data);
                        string resultCode = result.Substring(87, 6);
                        LogHelper.WriteLog("请求：4013\n请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
                        PinganResponse<qryDtlResponse> response = new PinganResponse<qryDtlResponse> { code = resultCode };
                        yq_log log = new yq_log();
                        log.createTime = DateTime.Now;
                        log.ip = GetIP.GetWebClientIp();
                        log.message = "查询账户当日历史交易明细[4013]";
                        log.requestXml = data;
                        log.responseXml = result;
                        log.yqCode = "4013";
                        log.userName = "微笑";
                        db.yq_log.Add(log);
                        db.SaveChanges();
                        if (resultCode == "000000")
                        {//受理成功
                            var resultXml = result.Substring(253, result.Length - 253);
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(resultXml);
                            string jsontext = JsonConvert.SerializeXmlNode(doc);
                            jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                            qryDtlResponse rModel = JsonConvert.DeserializeObject<qryDtlResponse>(jsontext);
                            response.result = rModel;
                        }
                        tran.Commit();
                        return response;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return new PinganResponse<qryDtlResponse>() { message = ex.ToString() };
                    }
                }
            }
        }
        /// <summary>
        /// 获取交易记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <param name="data">请求参数</param>
        /// <param name="result">返回参数</param>
        /// <param name="response">返回数据处理</param>
        public void qryDtl(qryDtlRequest request, userInfo user, out string data, out string result, out PinganResponse<qryDtlResponse> response)
        {
            data = "";
            result = "";
            try
            {
                if (string.IsNullOrEmpty(request.bsnCode))
                {
                    request.bsnCode = "4013";
                }
                if (string.IsNullOrEmpty(request.CcyCode))
                {
                    request.CcyCode = "RMB";

                }
                if (string.IsNullOrEmpty(request.OrderMode))
                {
                    request.OrderMode = "001";
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version=\"1.0\" encoding=\"GBK\"?>");
                sb.Append("<Result>");
                sb.Append("<AcctNo>" + request.AcctNo + "</AcctNo>");
                sb.Append("<CcyCode>" + request.CcyCode + "</CcyCode>");
                sb.Append("<BeginDate>" + request.BeginDate.ToString("yyyyMMdd") + "</BeginDate>");
                sb.Append("<EndDate>" + request.EndDate.ToString("yyyyMMdd") + "</EndDate>");
                sb.Append("<PageNo>" + request.PageNo + "</PageNo>");
                sb.Append("<PageSize>" + request.PageSize + "</PageSize>");
                sb.Append("<Reserve>" + request.Reserve + "</Reserve>");
                sb.Append("<OrderMode>" + request.OrderMode + "</OrderMode>");
                sb.Append("</Result>");
                data = YQUntil.asemblyPackets(yqdm, request.bsnCode, sb.ToString());
                LogHelper.WriteLog("请求交易记录。。。。\n", "yqzl");
                result = HttpHelps.HttpPost(url, data);
                if (result == null)
                {
                    throw new Exception("请求错误，银行接口有误！接口错误或者银行暂时不在工作，请稍后...");
                }
                string resultCode = result.Substring(87, 6);
                LogHelper.WriteLog("请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
                response = new PinganResponse<qryDtlResponse> { code = resultCode };
                if (resultCode == "000000")
                {//受理成功
                    var resultXml = result.Substring(253, result.Length - 253);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultXml);
                    string jsontext = JsonConvert.SerializeXmlNode(doc);
                    jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                    if (jsontext.Contains("\"list\":{"))
                    {//非数组改为数组
                        jsontext = jsontext.Replace("\"list\":{", "\"list\":[{");
                        jsontext = jsontext.Substring(0, jsontext.Length - 2) + "}]}";
                    }
                    qryDtlResponse rModel = JsonConvert.DeserializeObject<qryDtlResponse>(jsontext);
                    response.result = rModel;
                    response.code = "200";
                }
                else
                {
                    response.message = "银行记录读取有误,错误位置，请查看日志...";
                    response.code = "501";
                }
            }
            catch (Exception ex)
            {
                response = new PinganResponse<qryDtlResponse>() { code = "500", message = ex.ToString() };
            }

        }

        /// <summary>
        ///3.4企业大批量资金划转[4018] 4015查询进度，建议间隔时间为10分钟
        ///B2B转账， B2C还未测试
        ///请使用交易码400406，输入输出同此接口。
        /// </summary>
        /// <param name="requestModel">转账信息</param>
        /// <param name="bsnCode">转账编号</param>
        /// <returns></returns>
        public string xFerMax(XferMaxRequestModel requestModel, string bsnCode = "4018")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version =\"1.0\" encoding=\"GB2312\"?>");
            sb.Append("<Result>");
            sb.Append("<ThirdVoucher>" + requestModel.ThirdVoucher + "</ThirdVoucher>");
            sb.Append("<CcyCode>" + requestModel.CcyCode + "</CcyCode>");
            sb.Append("<OutAcctNo>" + requestModel.OutAcctNo + "</OutAcctNo>");
            sb.Append("<OutAcctName>" + requestModel.OutAcctName + "</OutAcctName>");
            sb.Append("<OutAcctAddr/><InAcctBankNode/><InAcctRecCode/>");
            sb.Append("<TotalCts>" + requestModel.totalCts + "</TotalCts>");
            sb.Append("<TotalAmt>" + requestModel.totalAmt + "</TotalAmt>");
            sb.Append("<AmountCode/>");
            sb.Append("<BatchSummary>" + requestModel.BatchSummary + "</BatchSummary>");
            sb.Append("<BSysFlag>" + requestModel.BSysFlag + "</BSysFlag>");
            sb.Append("<PayType>" + requestModel.PayType + "</PayType>");
            sb.Append("<BizFlag1>" + requestModel.BizFlag1 + "</BizFlag1>");
            sb.Append("<HOResultSet4018R>");
            for (int i = 0, len = requestModel.hOResultSet4018Rs.Count; i < len; i++)
            {
                sb.Append("<SThirdVoucher>" + (i + 1) + "</SThirdVoucher>");
                sb.Append("<CstInnerFlowNo>" + requestModel.hOResultSet4018Rs[i].CstInnerFlowNo + "</CstInnerFlowNo>");
                sb.Append("<InAcctBankNode>" + requestModel.hOResultSet4018Rs[i].InAcctBankNode + "</InAcctBankNode>");
                sb.Append("<InAcctRecCode>" + requestModel.hOResultSet4018Rs[i].InAcctRecCode + "</InAcctRecCode>");
                sb.Append("<InAcctNo>" + requestModel.hOResultSet4018Rs[i].InAcctNo + "</InAcctNo>");
                sb.Append("<InAcctName>" + requestModel.hOResultSet4018Rs[i].InAcctName + "</InAcctName>");
                sb.Append("<InAcctBankName>" + requestModel.hOResultSet4018Rs[i].InAcctBankName + "</InAcctBankName>");
                sb.Append("<InAcctProvinceCode>" + requestModel.hOResultSet4018Rs[i].InAcctProvinceCode + "</InAcctProvinceCode>");
                sb.Append("<InAcctCityName>" + requestModel.hOResultSet4018Rs[i].InAcctCityName + "</InAcctCityName>");
                sb.Append("<TranAmount>" + requestModel.hOResultSet4018Rs[i].TranAmount + "</TranAmount>");
                sb.Append("<UseEx>" + requestModel.hOResultSet4018Rs[i].UseEx + "</UseEx>");
                sb.Append("<UnionFlag>" + requestModel.hOResultSet4018Rs[i].UnionFlag + "</UnionFlag>");
                sb.Append("<AddrFlag>" + requestModel.hOResultSet4018Rs[i].AddrFlag + "</AddrFlag>");
            }
            sb.Append("</HOResultSet4018R>");
            sb.Append("</Result>");
            var data = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());   //return data;
            var result = HttpHelps.HttpPost(url, data);
            return result;

        }
        /// <summary>
        /// 获取交易请求数据
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="bsnCode"></param>
        /// <returns></returns>
        public string xFer_requestdata(XferRequestModel requestModel, string bsnCode = "4004")
        {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version =\"1.0\" encoding=\"GB2312\"?>");
                sb.Append("<Result>");
                sb.Append("<ThirdVoucher>" + requestModel.ThirdVoucher + "</ThirdVoucher>");
                sb.Append("<CstInnerFlowNo>" + requestModel.CstInnerFlowNo + "</CstInnerFlowNo>");
                sb.Append("<CcyCode>" + requestModel.CcyCode + "</CcyCode>");
                sb.Append("<OutAcctNo>" + requestModel.OutAcctNo + "</OutAcctNo>");
                sb.Append("<OutAcctName>" + requestModel.OutAcctName + "</OutAcctName>");
                sb.Append("<OutAcctAddr/><InAcctBankNode/><InAcctRecCode/>");
                sb.Append("<InAcctNo>" + requestModel.InAcctNo + "</InAcctNo>");
                sb.Append("<InAcctName>" + requestModel.InAcctName + "</InAcctName>");
                sb.Append("<InAcctBankName>" + requestModel.InAcctBankName + "</InAcctBankName>");
                sb.Append("<TranAmount>" + requestModel.TranAmount + "</TranAmount>");
                sb.Append("<AmountCode/>");
                sb.Append("<UseEx>" + requestModel.UseEx + "</UseEx>");
                sb.Append("<UnionFlag>" + requestModel.UnionFlag + "</UnionFlag>");
                sb.Append("<SysFlag>" + requestModel.SysFlag + "</SysFlag>");
                sb.Append("<AddrFlag>" + requestModel.AddrFlag + "</AddrFlag>");
                sb.Append("<RealFlag>2</RealFlag>");
                sb.Append("<MainAcctNo/></Result>");
                return YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());   //return data;
        }
        /// <summary>
        /// 发起交易
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        /// <param name="response"></param>
        public void xFer_pay(string data, out string result, out PinganResponse<XferResponseModel> response) { 
            result = null;
            try
            {
                LogHelper.WriteLog("开始请求付款。。。。\n", "yqzl");
                result = HttpHelps.HttpPost(url, data);
                string resultCode = result.Substring(87, 6);
                var resultMessage = result.Substring(94, result.Substring(94).IndexOf(" "));
                LogHelper.WriteLog("请求：4004\n请求参数：" + data + "\n返回参数：\n" + result, "yqzl");
                response = new PinganResponse<XferResponseModel> { code = resultCode, message = resultMessage };
                if (resultCode == "000000")
                {//受理成功
                    var resultXml = result.Substring(253, result.Length - 253);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultXml);
                    string jsontext = JsonConvert.SerializeXmlNode(doc);
                    jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                    XferResponseModel rModel = JsonConvert.DeserializeObject<XferResponseModel>(jsontext);
                    response.result = rModel;

                }
                else
                {
                    //  paymentRecord.errCode = resultCode;
                    //  paymentRecord.errMessage = resultMessage;
                    // var err = db.yq_error.Where(o => o.errCode == resultCode).FirstOrDefault();
                    // db.SaveChanges();
                    response.code = resultCode;
                    response.message = resultMessage;
                }
            }
            catch (Exception ex)
            {
                response = new PinganResponse<XferResponseModel>() { message = ex.ToString() };
            }

        }
        /// <summary>
        ///获取银联号
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <param name="response"></param>
        /// <param name="bsnCode"></param>
        public void getYLH(YlhModel requestModel, out string request, out string result, out PinganResponse<YlhModelResponse> response, string bsnCode = "4017")
        {
            request = "";
            result = null;
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<?xml version =\"1.0\" encoding=\"GB2312\"?>");
                sb.Append("<Result>");
                sb.Append("<BankNo>" + requestModel.Request.BankNo + "</BankNo>");
                sb.Append("<BankName>" + requestModel.Request.BankName + "</BankName>");
                sb.Append("<KeyWord>" + requestModel.Request.KeyWord + "</KeyWord>");
                sb.Append("</Result>");
                request = YQUntil.asemblyPackets(yqdm, bsnCode, sb.ToString());   //return data;
                result = HttpHelps.HttpPost(url, request);
                string resultCode = result.Substring(87, 6);
                var resultMessage = result.Substring(94, result.Substring(94).IndexOf(" "));
                LogHelper.WriteLog("请求：4017\n请求参数：" + request + "\n返回参数：\n" + result, "yqzl");
                response = new PinganResponse<YlhModelResponse> { code = resultCode, message = resultMessage };
                if (resultCode == "000000")
                {//受理成功
                    var resultXml = result.Substring(253, result.Length - 253);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(resultXml);
                    string jsontext = JsonConvert.SerializeXmlNode(doc);
                    jsontext = jsontext.Substring(10, jsontext.Length - 11);//"Result:{}"
                    if (jsontext.Contains("\"list\":{"))
                    {//非数组改为数组
                        jsontext = jsontext.Replace("\"list\":{", "\"list\":[{");
                        jsontext = jsontext.Substring(0, jsontext.Length - 2) + "}]}";
                    }
                    YlhModelResponse rModel = JsonConvert.DeserializeObject<YlhModelResponse>(jsontext);
                    response.result = rModel;

                }
                else
                {
                    //  paymentRecord.errCode = resultCode;
                    //  paymentRecord.errMessage = resultMessage;
                    // var err = db.yq_error.Where(o => o.errCode == resultCode).FirstOrDefault();
                    // db.SaveChanges();
                    response.code = resultCode;
                    response.message = resultMessage;
                }
            }
            catch (Exception ex)
            {
                response = new PinganResponse<YlhModelResponse>() { message = ex.ToString() };
            }

        }
    }

}
