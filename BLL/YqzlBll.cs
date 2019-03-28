using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using common;
using dal;
using EntityFromework;
using model;
using Model;
using Newtonsoft.Json;
using PinganYqzl;
using PinganYqzl.model;
using ZhaoshangYqzl;
using ZhaoshangYqzl.model;

namespace BLL
{
    public class YqzlBll
    {
        private static string LGNNAM = ConfigurationManager.AppSettings["Zhaohang"].ToString();//招行登录账号
        private static string yueJiLGNNAM = ConfigurationManager.AppSettings["ZhaohangYueji"].ToString();//悦肌招行登录账号

        private YqzlDal dal = new YqzlDal();
        /// <summary>
        /// 读取银企直联的账号
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public yq_userAccount yq_userAccount(Content content)
        {
            return dal.yq_userAccount(content);
        }

        /// <summary>
        /// 读取系统交易记录
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<yq_paymentRecord> yq_paymentRecordList(string isPay, userInfo userInfo)
        {
            if (isPay == "Yes")
            {//读取已经付款审批的记录时候顺便遍历更新状态
                var pays = dal.yq_paymentRecordList(null, (int)userInfo.company);//获取所有审批的交易记录
                foreach (var pay in pays)
                {
                    qryDtlByOrig(pay.thirdVoucher, userInfo);
                }
            }
            return dal.yq_paymentRecordList(isPay, (int)userInfo.company);
        }
        public object delPay(int id)
        {
            var delPay = dal.delPay(id);
            if (delPay == null)
            {
                return new
                {
                    start = 1,
                    errorMsg = "未找到付款记录，如果无法再次付款请联系系统管理员！"
                };

            }
            else
            {
                var delR = new FromOADal().delRecordTable(delPay.cstInnerFlowNo);
                if (delR == null)
                {
                    return new
                    {
                        start = 1,
                        errorMsg = "交易记录已删除,但是找不到U8录入记录，如果无法重新付款请联系管理员"
                    };
                }
                return new
                {
                    start = 0,
                    errorMsg = "交易已清除，可以重新发起交易"
                };
            }
        }
        /// <summary>
        /// 获取交易记录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public YqzlResponseModel<PaysModel> qryDtlList(YqzlRequestModel request, userInfo userinfo)
        {
            request.EndDate = request.BeginDate;//银行默认只能查询一天内的，所以开始时间和结束时间设置一样
            YqzlResponseModel<PaysModel> response = new YqzlResponseModel<PaysModel>();
            response.result = new PaysModel();
            response.result.pays = new List<PayModel>();
            if (request.content.BankName == null)
            {
                response.code = "404";
                response.message = "未设置银行，请在U8账户管理设置银行";
                return response;

            }
            if (request.content.BankName.Contains("招商") || request.content.BankName.Contains("招行"))
            {//包含此关键字的为招商银行，调用招行API接口，读取招行付款记录
                INFO iNFO = new INFO()
                {
                    FUNNAM = "GetTransInfo",
                    DATTYP = "2",
                    LGNNAM = LGNNAM
                };
                if (userinfo.company == (int)CompanyEnum.yueji)
                {
                    iNFO.LGNNAM = yueJiLGNNAM;
                }
                var SDKTSINFX = new SDKTSINFX()
                {
                    C_BBKNBR = request.content.City,
                    ACCNBR = request.content.BankAcct,//账号
                    BGNDAT = request.BeginDate.ToString("yyyyMMdd"),
                    ENDDAT = request.EndDate.ToString("yyyyMMdd")
                };
                var cmbc = new CMBSDKPGK()
                {
                    INFO = iNFO,
                    SDKTSINFX = SDKTSINFX
                };

                ZhaohangApi ZhaohangApi = new ZhaohangApi().CreateInstance();
                string requests;
                string results;
                ResonseClass result;
                ZhaohangApi.zhaoHangApi(cmbc, out requests, out results, out result);
                if (result.CMBSDKPGK == null || result.CMBSDKPGK.INFO == null)
                {
                    response.code = "500";
                    response.message = "请登录网银再操作";
                    return response;
                }
                YqzlDal yqzl = new YqzlDal();
                yqzl.AddLog("查询招行收款记录", "seach", requests, results, userinfo.name);
                // var result = ZhaohangApi.zhaoHangApi(cmbc);
                if (!string.IsNullOrEmpty(result.CMBSDKPGK.INFO.ERRMSG))
                {
                    response.code = "500";
                    response.message = result.CMBSDKPGK.INFO.ERRMSG;
                }
                if (result.CMBSDKPGK.NTQTSINFZ != null)
                {
                    FromYYDal yYDal = new FromYYDal(userinfo);
                    var allOaList = new FromOADal().getList(new Condition()
                    {
                        userInfo = userinfo
                    });
                    var allPayList = yYDal.GetYq_paymentRecordList();
                    var recordTableList = new List<RecordTable>();
                    if (request.BeginDate <= Convert.ToDateTime("2019-03-25"))
                    {
                        recordTableList = yYDal.GetRecordList(null);
                    }
                    else
                    {
                        recordTableList = yYDal.GetRecordList(request.BeginDate);
                    }
                    var UnitList = yYDal.UnitList();//获取所有的供应商客户信息备用
                    foreach (var item in result.CMBSDKPGK.NTQTSINFZ)
                    {
                        if (request.isPay == "F")
                        {
                            //  var record = yYDal.getRecordByPId(item.RPYACC + item.REFNBR);
                            //读取是否日记账
                            // var rliushui = item.RPYACC + item.REFNBR;//REQNBR
                            var rliushui = item.REFNBR;//REQNBR  收款后录入u8的流水号跟银行一致
                            var oaLiuShui = "";
                            var record = new RecordTable();
                            if (rliushui != null)
                            {// 直接找中间表（U8日记账中间表）
                            // record = yYDal.getRecordByLiushui(rliushui, item.AMTCDR.Equals("C") ? "收" : "付", request.content.ID.ToString());
                             record = recordTableList.Where(o => o.liushuihao == rliushui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.AMTCDR.Equals("C") ? "收" : "付")).FirstOrDefault();
                            }
                            if (record==null)
                            {
                                record = new RecordTable();
                            }
                            ResultListModel oas = new ResultListModel();//理论上是不存在的（一笔交易必须再OA上走流程）
                            oas.Id = item.RPYACC + item.REFNBR+ request.content.ID;
                            var fukuandanwei = item.RPYNAM;
                            var naryur = "付" + item.RPYNAM + item.NARYUR + "款";
                            if (item.AMTCDR.Equals("D"))
                            {//付款已经记录到日记账中了（获取PID，稍后日记账时传入最为Pid）
                                oaLiuShui = yYDal.getRecordByPayLiushui(item.REQNBR)?.cstInnerFlowNo;//OA流水号（表示通过系统生单的）,直接支付为直接生成日记账（找交易记录表，然后根据交易记录表找寻日记账表）
                                if (!string.IsNullOrEmpty(oaLiuShui))
                                {
                                    //var oaRecord = yYDal.getRecordByLiushui(oaLiuShui, item.AMTCDR.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                    var oaRecord = recordTableList.Where(o => o.liushuihao == oaLiuShui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.AMTCDR.Equals("C") ? "收" : "付")).FirstOrDefault();

                                    record = oaRecord == null ? record : new RecordTable();
                                    record.chunabianhao = oaRecord == null ? record?.chunabianhao : oaRecord.chunabianhao;
                                }
                                /*var oaList = new FromOADal().getList(new Condition()
                                {
                                    money = item.TRSAMTD,
                                    AcctNo = item.RPYACC,
                                    liuShui = oaLiuShui,
                                    userInfo = userinfo
                                }).OrderByDescending(o => o.fukuanriqi).ToList();*/
                                var oaList = allOaList
                                  .Where(o => o.yinhuan == item.TRSAMTD && Regex.Replace(o.zhanhao, @"[^0-9]+", "") == item.RPYACC)
                                  .OrderByDescending(o => o.fukuanriqi).ToList();
                                if (oaList.Count > 0)
                                {
                                    oas = oaList[0];
                                }
                                if (oaList.Count > 1)
                                {//找到不止一个单
                                    foreach (var oa in oaList)
                                    {
                                        var detail = new FromOADal().getDetail(oa, new Condition()
                                        {
                                            userInfo = userinfo
                                        });
                                        oa.zhanhao = oa.zhanhao.Replace(" ", "").Replace("    ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                        oa.shoukuanyh = oa.shoukuanyh.Replace(" ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                        var message = detail.list[0].miaoshu
                                            .Replace("预支", "").Replace("报销", "")
                                            .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                            .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                            .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                            .Replace(" ", "").Replace("；", "").Replace("]", "")
                                            .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                        item.NARYUR = item.NARYUR.Replace("预支", "").Replace("报销", "")
                                            .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                            .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                            .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                            .Replace(" ", "").Replace("；", "").Replace("]", "")
                                            .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                        if (item.NARYUR.Contains(message))
                                        {
                                            oas = oa;
                                            break;
                                        }

                                    }
                                }
                                //更改付款摘要
                                if (oas.type != null)
                                {
                                    var detail = new FromOADal().getDetail(oas, new Condition()
                                    {
                                        userInfo = userinfo
                                    });
                                    if (oas.type.Contains("预支单"))
                                    {
                                        naryur = oas.faqibumen + oas.faqiren + "借支" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                    }
                                    else
                                    {
                                        naryur = oas.faqibumen + oas.faqiren + "报销" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                    }
                                }
                                //var userEx = item.NARYUR.Substring(0, item.NARYUR.IndexOf("报销") - 1) + fukuandanwei + item.NARYUR.Substring(item.NARYUR.IndexOf("报销") + 1, item.NARYUR.Length - item.NARYUR.IndexOf("报销") + 1);
                            }
                            //item.NARYUR = naryur;
                            if (item.AMTCDR.Equals("C") && (record == null || string.IsNullOrEmpty(record?.chunabianhao)))
                            {//表示没有记账到U8的需要读取供应商
                             /* var fukuandanweis = yYDal.getUnit(item.OutAcctName);//供应商或者个人
                                if (!string.IsNullOrEmpty(fukuandanweis))
                                {
                                    fukuandanwei = fukuandanweis;
                                }
                                */
                                var units = UnitList.Where(o => o.cCusName == item.RPYNAM).FirstOrDefault();
                                if (units != null)
                                {
                                    fukuandanwei = units.UnitTypeName + units.cDepName + units.cCusName;
                                }
                            }
                            response.result.pays.Add(new PayModel
                            {
                                hostTxDate = item.ETYDAT,
                                inAcctName = item.RPYNAM,
                                inAcctNo = item.RPYACC,
                                tranAmount = item.TRSAMTC,
                                liushuihan = item.REFNBR,
                                useEx = item.AMTCDR.Equals("C") ? "收" + fukuandanwei + item.NARYUR + "货款" : naryur,
                                chunabianhao = record?.chunabianhao,
                                AMTCDR = item.AMTCDR,
                                TRSAMTC = item.TRSAMTC,
                                TRSAMTD = item.TRSAMTD,
                                TRSBLV = item.TRSBLV,
                                oa = oas
                            });
                            response.getMoney += item.TRSAMTC;
                            response.payMoney += item.TRSAMTD;
                        }
                        else
                        {
                            if (item.AMTCDR.Equals(request.isPay))
                            {
                                //  var record = yYDal.getRecordByPId(item.RPYACC + item.REFNBR);
                                //读取是否日记账
                                // var rliushui = item.RPYACC + item.REFNBR;//REQNBR
                                var rliushui = item.REFNBR;//REQNBR  收款后录入u8的流水号跟银行一致
                                var oaLiuShui = "";
                                var record = new RecordTable();
                                if (rliushui != null)
                                {// 直接找中间表（U8日记账中间表）
                                 // record = yYDal.getRecordByLiushui(rliushui, item.AMTCDR.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                    record = recordTableList.Where(o => o.liushuihao == rliushui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.AMTCDR.Equals("C") ? "收" : "付")).FirstOrDefault();
                                }
                                if (record == null)
                                {
                                    record = new RecordTable();
                                }
                                ResultListModel oas = new ResultListModel();//理论上是不存在的（一笔交易必须再OA上走流程）
                                oas.Id = item.RPYACC + item.REFNBR + request.content.ID;
                                var fukuandanwei = item.RPYNAM;
                                var naryur = "付" + item.RPYNAM + item.NARYUR + "款";
                                if (item.AMTCDR.Equals("D"))
                                {//付款已经记录到日记账中了（获取PID，稍后日记账时传入最为Pid）
                                    oaLiuShui = yYDal.getRecordByPayLiushui(item.REQNBR)?.cstInnerFlowNo;//OA流水号（表示通过系统生单的）,直接支付为直接生成日记账（找交易记录表，然后根据交易记录表找寻日记账表）
                                    if (!string.IsNullOrEmpty(oaLiuShui))
                                    {
                                        //var oaRecord = yYDal.getRecordByLiushui(oaLiuShui, item.AMTCDR.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                        var oaRecord = recordTableList.Where(o => o.liushuihao == oaLiuShui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.AMTCDR.Equals("C") ? "收" : "付")).FirstOrDefault();

                                        record = oaRecord == null ? record : new RecordTable();
                                        record.chunabianhao = oaRecord == null ? record?.chunabianhao : oaRecord.chunabianhao;
                                    }
                                    /*var oaList = new FromOADal().getList(new Condition()
                                    {
                                        money = item.TRSAMTD,
                                        AcctNo = item.RPYACC,
                                        liuShui = oaLiuShui,
                                        userInfo = userinfo
                                    }).OrderByDescending(o => o.fukuanriqi).ToList();*/
                                    var oaList = allOaList
                                      .Where(o => o.yinhuan == item.TRSAMTD && Regex.Replace(o.zhanhao, @"[^0-9]+", "") == item.RPYACC)
                                      .OrderByDescending(o => o.fukuanriqi).ToList();
                                    if (oaList.Count > 0)
                                    {
                                        oas = oaList[0];
                                    }
                                    if (oaList.Count > 1)
                                    {//找到不止一个单
                                        foreach (var oa in oaList)
                                        {
                                            var detail = new FromOADal().getDetail(oa, new Condition()
                                            {
                                                userInfo = userinfo
                                            });
                                            oa.zhanhao = oa.zhanhao.Replace(" ", "").Replace("    ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                            oa.shoukuanyh = oa.shoukuanyh.Replace(" ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                            var message = detail.list[0].miaoshu
                                                .Replace("预支", "").Replace("报销", "")
                                                .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                                .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                                .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                                .Replace(" ", "").Replace("；", "").Replace("]", "")
                                                .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                            item.NARYUR = item.NARYUR.Replace("预支", "").Replace("报销", "")
                                                .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                                .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                                .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                                .Replace(" ", "").Replace("；", "").Replace("]", "")
                                                .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                            if (item.NARYUR.Contains(message))
                                            {
                                                oas = oa;
                                                break;
                                            }

                                        }
                                    }
                                    //更改付款摘要
                                    if (oas.type != null)
                                    {
                                        var detail = new FromOADal().getDetail(oas, new Condition()
                                        {
                                            userInfo = userinfo
                                        });
                                        if (oas.type.Contains("预支单"))
                                        {
                                            naryur = oas.faqibumen + oas.faqiren + "借支" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                        }
                                        else
                                        {
                                            naryur = oas.faqibumen + oas.faqiren + "报销" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                        }
                                    }
                                    //item.NARYUR = naryur;
                                    //var userEx = item.NARYUR.Substring(0, item.NARYUR.IndexOf("报销") - 1) + fukuandanwei + item.NARYUR.Substring(item.NARYUR.IndexOf("报销") + 1, item.NARYUR.Length - item.NARYUR.IndexOf("报销") + 1);
                                }
                                //item.NARYUR = naryur;
                                if (item.AMTCDR.Equals("C") && (record == null || string.IsNullOrEmpty(record?.chunabianhao)))
                                {//表示没有记账到U8的需要读取供应商
                                 /* var fukuandanweis = yYDal.getUnit(item.OutAcctName);//供应商或者个人
                                    if (!string.IsNullOrEmpty(fukuandanweis))
                                    {
                                        fukuandanwei = fukuandanweis;
                                    }
                                    */
                                    var units = UnitList.Where(o => o.cCusName == item.RPYNAM).FirstOrDefault();
                                    if (units != null)
                                    {
                                        fukuandanwei = units.UnitTypeName + units.cDepName + units.cCusName;
                                    }
                                }
                                response.result.pays.Add(new PayModel
                                {
                                    hostTxDate = item.ETYDAT,
                                    inAcctName = item.RPYNAM,
                                    inAcctNo = item.RPYACC,
                                    tranAmount = item.TRSAMTC,
                                    liushuihan = item.REFNBR,
                                    useEx = item.AMTCDR.Equals("C") ? "收" + fukuandanwei + item.NARYUR + "货款" : naryur,
                                    chunabianhao = record?.chunabianhao,
                                    AMTCDR = item.AMTCDR,
                                    TRSAMTC = item.TRSAMTC,
                                    TRSAMTD = item.TRSAMTD,
                                    TRSBLV = item.TRSBLV,
                                    oa = oas
                                });
                                //response.getMoney += item.TRSAMTC;
                                //response.payMoney += item.TRSAMTD;
                            }
                        }
                    }
                }
            }
            else if (request.content.BankName.Contains("平安"))
            {
                qryDtlRequest qry = new qryDtlRequest();
                qry.BeginDate = request.BeginDate;
                qry.EndDate = request.EndDate;
                if (qry.BeginDate.Equals(DateTime.Now.ToString("yyyyMMdd")))
                {
                    qry.PageSize = 100;//当日最多查100条记录
                }
                else
                {//历史最多查询1000条记录
                    qry.PageSize = 1000;
                }
                qry.AcctNo = request.content.BankAcct;
#if DEBUG
                // qry.AcctNo = "0122100598214";
                // qry.AcctNo = "11014901041000";
                //  requestModel.OutAcctName = "广东悦肌化妆品有限公司";
#endif
                PinganApi PinganApi = new PinganApi();
                FromYYDal yYDal = new FromYYDal(userinfo);
                YqzlDal yqzl = new YqzlDal();
                for (int i = 0; i < i + 1; i++)
                {
                    qry.PageNo = i + 1;
                    string requests, results;
                    PinganResponse<qryDtlResponse> result;
                    PinganApi.qryDtl(qry, userinfo, out requests, out results, out result);
                    yqzl.AddLog("查询平安收款记录", "seach", requests, results, userinfo.name);//添加交易记录
                    if (result.code.Equals("500"))
                    {
                        response.code = "500";
                        response.message = result.message;
                        break;
                    }
                    else if (result.result == null)
                    {
                        response.code = "404";
                        response.message = results;
                        break;
                    }
                    else if (result.result.list != null)
                    {
                        var allOaList = new FromOADal().getList(new Condition()
                        {
                            userInfo = userinfo
                        });
                        var allPayList = yYDal.GetYq_paymentRecordList();
                        var recordTableList =new List<RecordTable>();
                        if (request.BeginDate<= Convert.ToDateTime("2019-03-25"))
                        {
                            recordTableList = yYDal.GetRecordList(null);
                        }
                        else
                        {
                            recordTableList = yYDal.GetRecordList(request.BeginDate);
                        }
                        var UnitList = yYDal.UnitList();//获取所有的供应商客户信息备用
                        foreach (var item in result.result.list)
                        {
                            if (request.isPay == "F")
                            { //item.OutAcctName = "广州佳能物流有限公司";

                                //读取是否日记账frontLogNo
                                var rliushui = item.BussSeqNo;//REQNBR  收款后录入u8的流水号跟银行一致
                                var oaLiuShui = "";
                                ResultListModel oas = new ResultListModel();//理论上是不存在的（一笔交易必须再OA上走流程）
                                oas.Id = item.InAcctNo + item.BussSeqNo + request.content.ID;

                                if (item.DcFlag.Equals("D"))
                                {//付款已经记录到日记账中了
                                 //每条收款记录都读数据库太慢了
                                 // var payXitong = yYDal.getRecordByPayLiushui(item.BussSeqNo);//通过系统付款
                                    var payXitong = allPayList.Where(o => o.frontLogNo.Equals(item.BussSeqNo)).FirstOrDefault();
                                    oaLiuShui = payXitong == null ? rliushui : payXitong.cstInnerFlowNo;//OA流水号
                                }
                                var record = new RecordTable();
                                if (rliushui != null)
                                {
                                    //每次查询都读数据库太慢了
                                    // record = yYDal.getRecordByLiushui(rliushui, item.DcFlag.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                    record = recordTableList.Where(o => o.liushuihao == rliushui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.DcFlag.Equals("C") ? "收" : "付")).FirstOrDefault();
                                }
                                if (record == null)
                                {
                                    record = new RecordTable();
                                }
                                if (!string.IsNullOrEmpty(oaLiuShui))
                                {
                                    // var oaRecord = yYDal.getRecordByLiushui(oaLiuShui, item.DcFlag.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                    var oaRecord = recordTableList.Where(o => o.liushuihao == oaLiuShui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.DcFlag.Equals("C") ? "收" : "付")).FirstOrDefault();

                                    if (oaRecord != null)
                                    {
                                        record.chunabianhao = oaRecord.chunabianhao;
                                    }
                                }
                                //每条记录都去读数据库太慢了，先一次读取保存在本地

                                /*var oaList = new FromOADal().getList(new Condition()
                                {
                                    money = item.TranAmount,
                                    AcctNo = item.InAcctNo,
                                    liuShui = oaLiuShui,
                                    userInfo = userinfo
                                }).OrderByDescending(o => o.fukuanriqi).ToList();*/
                                var oaList = allOaList
                                    .Where(o => o.yinhuan == item.TranAmount && Regex.Replace(o.zhanhao==null?"": o.zhanhao, @"[^0-9]+", "") == item.InAcctNo)
                                    .OrderByDescending(o => o.fukuanriqi).ToList();
                                if (oaList.Count > 0)
                                {
                                    oas = oaList[0];
                                }
                                if (oaList.Count > 1)
                                {//找到不止一个单
                                    foreach (var oa in oaList)
                                    {
                                        var detail = new FromOADal().getDetail(oa, new Condition()
                                        {
                                            userInfo = userinfo
                                        });
                                        oa.zhanhao = oa.zhanhao.Replace(" ", "").Replace("    ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                        oa.shoukuanyh = oa.shoukuanyh.Replace(" ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                        var message = detail.list[0].miaoshu
                                            .Replace("预支", "").Replace("报销", "")
                                            .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                            .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                            .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                            .Replace(" ", "").Replace("；", "").Replace("]", "")
                                            .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                        item.Purpose = item.Purpose.Replace("预支", "").Replace("报销", "")
                                            .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                            .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                            .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                            .Replace(" ", "").Replace("；", "").Replace("]", "")
                                            .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                        if (item.Purpose.Contains(message))
                                        {
                                            oas = oa;
                                            break;
                                        }
                                    }
                                }
                                var naryur = "付" + item.InAcctName + item.Purpose + "款";
                                //更改付款摘要
                                if (oas.type != null)
                                {
                                    var detail = new FromOADal().getDetail(oas, new Condition()
                                    {
                                        userInfo = userinfo
                                    });
                                    if (oas.type.Contains("预支单"))
                                    {
                                        naryur = oas.faqibumen + oas.faqiren + "借支" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                    }
                                    else
                                    {
                                        naryur = oas.faqibumen + oas.faqiren + "报销" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                    }
                                }
                                //item.Purpose = naryur;
                                //var fukuandanwei = yYDal.getUnit(item.OutAcctName);//供应商或者个人
                                var fukuandanwei = item.OutAcctName;
                                if (item.DcFlag.Equals("C")) //&& (record == null || string.IsNullOrEmpty(record?.chunabianhao)))
                                {//表示没有记账到U8的需要读取供应商
                                 /* var fukuandanweis = yYDal.getUnit(item.OutAcctName);//供应商或者个人
                                    if (!string.IsNullOrEmpty(fukuandanweis))
                                    {
                                        fukuandanwei = fukuandanweis;
                                    }
                                    */
                                    CustomertModel units = null;
                                    if (item.OutAcctNo == "210401324")
                                    {//拉卡拉
                                        fukuandanwei = "";
                                        var PurposeLeft = item.Purpose.Substring(0, item.Purpose.Length / 2);
                                        var PurposeRight = item.Purpose.Substring(item.Purpose.Length / 2, item.Purpose.Length / 2);
                                        if (PurposeLeft== PurposeRight)
                                        {
                                            item.Purpose = PurposeLeft;
                                        }
                                        units = UnitList.Where(o => o.cCusName == item.Purpose).FirstOrDefault();
                                    }
                                    else {
                                         units = UnitList.Where(o => o.cCusName == item.OutAcctName).FirstOrDefault();
                                    }
                                    oas.piaoju = new List<Piaoju>();//只有一张（原先设置生成多张）
                                    if (units != null)
                                    {
                                        fukuandanwei = units.UnitTypeName + units.cDepName + units.cCusName;
                                        oas.shouyibumenCode = request.content.SubjectCode == "1002010101" ? "10" : units.cDepCode;
                                    }
                                    var Piaoju = new Piaoju();
                                    Piaoju.Detail = new List<Detail>();
                                    var Detail = new Detail
                                    {
                                        miaoshu = "收" + fukuandanwei + item.Purpose + "款",
                                        jine = item.TranAmount, //(第一条收款凭证的话md有值)
                                        jiefan = 0,
                                        shouyibumen = request.content.SubjectCode == "1002010101" ? "10" : units?.cDepCode,
                                        kemu = request.content.SubjectCode,
                                    };
                                    Piaoju.Detail.Add(Detail);
                                    var Details = new Detail
                                    {
                                        miaoshu = "收" + fukuandanwei + item.Purpose + "款",
                                        jine = 0, //(第二条摘要MC)
                                        jiefan = item.TranAmount,
                                        shouyibumen = request.content.SubjectCode == "1002010101" ? "10" : units?.cDepCode,
                                        kemu = request.content.SubjectCode == "1002010101" ? "1221000003" : "1122"
                                    };
                                    Piaoju.Detail.Add(Detail);
                                    oas.piaoju.Add(Piaoju);
                                }
                                if (item.OutAcctNo == "210401324")
                                {//拉卡拉
                                    fukuandanwei = "";
                                    item.Purpose = item.Purpose.Substring(0, item.Purpose.Length / 2);
                                }
                                response.result.pays.Add(new PayModel()
                                {
                                    hostTxDate = item.HostDate,
                                    inAcctName = item.DcFlag.Equals("C") ? item.OutAcctName : item.InAcctName,
                                    inAcctNo = item.DcFlag.Equals("C") ? item.OutAcctNo : item.InAcctNo,
                                    tranAmount = item.TranAmount,
                                    liushuihan = item.BussSeqNo,
                                    useEx = item.DcFlag.Equals("C") ? "收" + fukuandanwei + item.Purpose + "款" : naryur, //"付" + item.Purpose + item.InAcctName + "款",
                                    chunabianhao = record?.chunabianhao,
                                    AMTCDR = item.DcFlag,
                                    TRSAMTC = item.DcFlag.Equals("C") ? item.TranAmount : 0,
                                    TRSAMTD = item.DcFlag.Equals("C") ? 0 : item.TranAmount,
                                    TRSBLV = item.AcctBalance,
                                    oa = oas
                                });
                                response.getMoney = item.DcFlag.Equals("C") ? response.getMoney + item.TranAmount : response.getMoney;
                                response.payMoney = item.DcFlag.Equals("D") ? response.payMoney + item.TranAmount : response.payMoney;
                            }
                            else
                            {//分类查询（收支分开）
                                if (item.DcFlag.Equals(request.isPay))
                                {
                                    //读取是否日记账frontLogNo
                                    var rliushui = item.BussSeqNo;//REQNBR  收款后录入u8的流水号跟银行一致
                                    if (rliushui == "4653291903051736550252")
                                    {
                                    }
                                    var oaLiuShui = "";
                                    ResultListModel oas = new ResultListModel();//理论上是不存在的（一笔交易必须再OA上走流程）
                                    oas.Id = item.InAcctNo + item.BussSeqNo + request.content.ID;

                                    if (item.DcFlag.Equals("D"))
                                    {//付款已经记录到日记账中了
                                     //每条收款记录都读数据库太慢了
                                     // var payXitong = yYDal.getRecordByPayLiushui(item.BussSeqNo);//通过系统付款
                                        var payXitong = allPayList.Where(o => o.frontLogNo.Equals(item.BussSeqNo)).FirstOrDefault();
                                        oaLiuShui = payXitong == null ? rliushui : payXitong.cstInnerFlowNo;//OA流水号
                                    }
                                    var record = new RecordTable();
                                    if (rliushui != null)
                                    {
                                        //每次查询都读数据库太慢了
                                        // record = yYDal.getRecordByLiushui(rliushui, item.DcFlag.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                        record = recordTableList.Where(o => o.liushuihao == rliushui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.DcFlag.Equals("C") ? "收" : "付")).FirstOrDefault();
                                    }
                                    if (record == null)
                                    {
                                        record = new RecordTable();
                                    }
                                    if (!string.IsNullOrEmpty(oaLiuShui))
                                    {
                                        // var oaRecord = yYDal.getRecordByLiushui(oaLiuShui, item.DcFlag.Equals("C") ? "收" : "付", request.content.ID.ToString());
                                        var oaRecord = recordTableList.Where(o => o.liushuihao == oaLiuShui && o.contents == request.content.ID.ToString() && o.chunabianhao.Contains(item.DcFlag.Equals("C") ? "收" : "付")).FirstOrDefault();

                                        if (oaRecord != null)
                                        {
                                            record.chunabianhao = oaRecord.chunabianhao;
                                        }
                                    }
                                    //每条记录都去读数据库太慢了，先一次读取保存在本地

                                    /*var oaList = new FromOADal().getList(new Condition()
                                    {
                                        money = item.TranAmount,
                                        AcctNo = item.InAcctNo,
                                        liuShui = oaLiuShui,
                                        userInfo = userinfo
                                    }).OrderByDescending(o => o.fukuanriqi).ToList();*/
                                    var oaList = allOaList
                                        .Where(o => o.yinhuan == item.TranAmount && Regex.Replace(o.zhanhao, @"[^0-9]+", "") == item.InAcctNo)
                                        .OrderByDescending(o => o.fukuanriqi).ToList();
                                    if (oaList.Count > 0)
                                    {
                                        oas = oaList[0];
                                    }
                                    if (oaList.Count > 1)
                                    {//找到不止一个单
                                        foreach (var oa in oaList)
                                        {
                                            var detail = new FromOADal().getDetail(oa, new Condition()
                                            {
                                                userInfo = userinfo
                                            });
                                            oa.zhanhao = oa.zhanhao.Replace(" ", "").Replace("    ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                            oa.shoukuanyh = oa.shoukuanyh.Replace(" ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                                            var message = detail.list[0].miaoshu
                                                .Replace("预支", "").Replace("报销", "")
                                                .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                                .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                                .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                                .Replace(" ", "").Replace("；", "").Replace("]", "")
                                                .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                            item.Purpose = item.Purpose.Replace("预支", "").Replace("报销", "")
                                                .Replace(oa.faqibumen, "").Replace(oa.faqiren, "").Replace(oa.shoukuandanwei, "")
                                                .Replace(oa.shoukuandanwei, "").Replace("?", "").Replace("<", "")
                                                .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                                                .Replace(" ", "").Replace("；", "").Replace("]", "")
                                                .Replace("-", "").Replace("\n", "").Replace("\t", "");
                                            if (item.Purpose.Contains(message))
                                            {
                                                oas = oa;
                                                break;
                                            }
                                        }
                                    }
                                    var naryur = "付" + item.InAcctName + item.Purpose + "款";
                                    //更改付款摘要
                                    if (oas.type != null)
                                    {
                                        var detail = new FromOADal().getDetail(oas, new Condition()
                                        {
                                            userInfo = userinfo
                                        });
                                        if (oas.type.Contains("预支单"))
                                        {
                                            naryur = oas.faqibumen + oas.faqiren + "借支" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                        }
                                        else
                                        {
                                            naryur = oas.faqibumen + oas.faqiren + "报销" + oas.shouyibumen + oas.shoukuandanwei + detail.list[0].miaoshu + "款";
                                        }
                                    }
                                    //item.Purpose = naryur;
                                    //var fukuandanwei = yYDal.getUnit(item.OutAcctName);//供应商或者个人
                                    var fukuandanwei = item.OutAcctName;
                                    if (item.DcFlag.Equals("C") && (record == null || string.IsNullOrEmpty(record?.chunabianhao)))
                                    {//表示没有记账到U8的需要读取供应商
                                     /* var fukuandanweis = yYDal.getUnit(item.OutAcctName);//供应商或者个人
                                        if (!string.IsNullOrEmpty(fukuandanweis))
                                        {
                                            fukuandanwei = fukuandanweis;
                                        }
                                        */
                                        var units = UnitList.Where(o => o.cCusName == item.OutAcctName).FirstOrDefault();
                                        if (units != null)
                                        {
                                            fukuandanwei = units.UnitTypeName + units.cDepName + units.cCusName;
                                        }
                                    }
                                    if (item.OutAcctNo == "210401324")
                                    {//拉卡拉
                                        fukuandanwei = "";
                                    }
                                    response.result.pays.Add(new PayModel()
                                    {
                                        hostTxDate = item.HostDate,
                                        inAcctName = item.DcFlag.Equals("C") ? item.OutAcctName : item.InAcctName,
                                        inAcctNo = item.DcFlag.Equals("C") ? item.OutAcctNo : item.InAcctNo,
                                        tranAmount = item.TranAmount,
                                        liushuihan = item.BussSeqNo,
                                        useEx = item.DcFlag.Equals("C") ? "收" + fukuandanwei + item.Purpose + "款" : naryur, //"付" + item.Purpose + item.InAcctName + "款",
                                        chunabianhao = record?.chunabianhao,
                                        AMTCDR = item.DcFlag,
                                        TRSAMTC = item.DcFlag.Equals("C") ? item.TranAmount : 0,
                                        TRSAMTD = item.DcFlag.Equals("C") ? 0 : item.TranAmount,
                                        TRSBLV = item.AcctBalance,
                                        oa = oas
                                    });
                                    // response.getMoney = item.DcFlag.Equals("C") ? response.getMoney + item.TranAmount : response.getMoney;
                                    // response.payMoney = item.DcFlag.Equals("D") ? response.payMoney + item.TranAmount : response.payMoney;

                                }
                            }
                        }
                    }
                    if (result.result.EndFlag.Equals("Y"))
                    {//读取结束
                        break;
                    }
                }
            }
            else
            {
                response.code = "404";
                response.message = "暂时还未开放该银行的银企直联";
            }
            return response;
        }
        /// <summary>
        /// 获取收款单位的省市
        /// </summary>
        /// <param name="shoukuanyinhan"></param>
        /// <returns></returns>
        public string ecs_regionPcity(string shoukuanyinhan)
        {
            return new YqzlDal().ecs_regionPcity(shoukuanyinhan);
        }
        /// <summary>
        /// 获取收款单位的省市
        /// </summary>
        /// <param name="shoukuanyinhan"></param>
        /// <returns></returns>
        public List<ecs_region> ecs_regionPcity_New(string shoukuanyinhan)
        {
            return new YqzlDal().ecs_regionPcity_New(shoukuanyinhan);
        }
        /// <summary>
        /// 审核付款（直接付款了）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseMessage Approval(userInfo user, string liushuihao)
        {
            ResponseMessage responsMessage = new ResponseMessage();
            if (user.type == 3)
            {
                var pay = dal.yq_paymentRecordByliushuihao(liushuihao);//读取代付款的交易
                if (pay == null)
                {
                    responsMessage.errorMsg = "交易不存在或者交易已撤销...";
                    return responsMessage;
                }
                else if (pay.outAcctAddr.Contains("招行") || pay.outAcctAddr.Contains("招商"))
                {
                    string request;//返回的请求参数
                    string result;//返回的数据
                    ResonseClass response;//返回的实体
                    ZhaohangApi api = new ZhaohangApi().CreateInstance();
                    CMBSDKPGK cmbc = JsonConvert.DeserializeObject<CMBSDKPGK>(pay.requestModelYQ);//读取数据
                    cmbc.INFO.LGNNAM = LGNNAM;//修改操作员（真实付款人员）
                    if (user.company == (int)CompanyEnum.yueji)
                    {
                        cmbc.INFO.LGNNAM = yueJiLGNNAM;
                    }
                    if (cmbc.DCOPDPAYX != null)
                    {//直接支付
                        cmbc.DCOPDPAYX.EPTDAT = DateTime.Now.ToString("yyyyMMdd");//修改预计付款日期
                    }
                    if (cmbc.NTECKOPRX != null)
                    {//跨行/异地
                        cmbc.NTECKOPRX.EFTDAT = DateTime.Now.ToString("yyyyMMdd");
                        cmbc.NTECKOPRX.EXPDAT = DateTime.Now.AddDays(4).ToString("yyyyMMdd");
                        cmbc.NTECKOPRX.ADDDAT = DateTime.Now.ToString("yyyyMMdd");
                        if (user.company == (int)CompanyEnum.yueji)
                        {
                            cmbc.NTOPRMODX.BUSMOD = "00003";//修改业务模式为00003（悦肌有三个移动支票的业务模式。所以只能用第三个）
                        }
                    }
                    api.zhaoHangApi(cmbc, out request, out result, out response);
                    dal.AddLog("招行付款", "add", request, result, user.name);//添加交易记录
                    if (string.IsNullOrEmpty(result))
                    {
                        responsMessage.errorMsg ="请登录招行前置机...";
                        return responsMessage;
                    }
                    if (!string.IsNullOrEmpty(response.CMBSDKPGK.INFO.ERRMSG))
                    {
                        responsMessage.errorMsg = response.CMBSDKPGK.INFO.ERRMSG;
                        return responsMessage;
                    }
                    if (response.CMBSDKPGK.NTQPAYRQZ != null && response.CMBSDKPGK.NTQPAYRQZ.ERRTXT != null)
                    {
                        /* 先取消，行内转账未成功的话选择跨行转账
                         * if (response.CMBSDKPGK.NTQPAYRQZ.ERRCOD.Equals("CSAB051"))
                         {//第一次默认本行转账，本行转账不成功则转成跨行转账
                             pay.unionFlag = "Y";
                             pay.requestdata = pay.requestdata.Replace("<BNKFLG>Y</BNKFLG>", "<BNKFLG>N</BNKFLG>");//转成跨行
                             api.zhaoHangApi_pay(pay.requestdata, out results, out response);
                             if (!string.IsNullOrEmpty(response.CMBSDKPGK.INFO.ERRMSG))
                             {
                                 return new { errorMsg = response.CMBSDKPGK.INFO.ERRMSG };
                             }
                             if (response.CMBSDKPGK.NTQPAYRQZ != null && response.CMBSDKPGK.NTQPAYRQZ.ERRTXT != null)
                             {
                                 return new { errorMsg = response.CMBSDKPGK.NTQPAYRQZ.ERRTXT };
                             }
                         }
                         else
                         {
                             return new { errorMsg = response.CMBSDKPGK.NTQPAYRQZ.ERRTXT };
                         }*/
                        responsMessage.errorMsg = response.CMBSDKPGK.NTQPAYRQZ.ERRTXT;
                        return responsMessage;
                    }
                    if (response.CMBSDKPGK.NTQPAYRQZ != null)
                    {//交易成功,银行受理(同城本行)
                        pay.frontLogNo = response.CMBSDKPGK.NTQPAYRQZ.REQNBR;
                        pay.stt = string.IsNullOrEmpty(response.CMBSDKPGK.NTQPAYRQZ.RTNFLG) ? response.CMBSDKPGK.NTQPAYRQZ.REQSTS : response.CMBSDKPGK.NTQPAYRQZ.RTNFLG;
                        pay.responsedata = result;
                        pay.requestdata = request;
                        pay.hostFlowNo = response.CMBSDKPGK.NTQPAYRQZ.YURREF;
                        pay.isApproval = true;
                        pay.approvalMan = user.name;
                        pay.approvalDate = DateTime.Now;
                        pay.hostTxDate = DateTime.Now.ToString("yyyyMMdd");
                        dal.editPay(pay);//添加交易信息
                        ResultListModel resultListModel = JsonConvert.DeserializeObject<ResultListModel>(pay.requestModelYY);
                        resultListModel.acctDate = DateTime.Now;
                        YYDal yDal = new YYDal(user);
                        user.name = pay.createMan;//修改U8日记账人为发起单据者
                                                  /*if (pay.addrFlag == 1 && pay.unionFlag == "1")
                                                  {*/
                        return yDal.AddToAcctBook(resultListModel, user);
                        /* }
                         else
                         {
                             responsMessage.sucess = "已提交审批，请在手机端审核后在录入日记账";
                             return responsMessage;

                         }*/
                    }
                    else if (response.CMBSDKPGK.NTECKOPRZ != null)
                    {
                        //交易成功,银行受理（异地跨行）
                        pay.frontLogNo = response.CMBSDKPGK.NTECKOPRZ.REQNBR;
                        pay.stt = string.IsNullOrEmpty(response.CMBSDKPGK.NTECKOPRZ.RTNFLG) ? response.CMBSDKPGK.NTECKOPRZ.REQSTS : response.CMBSDKPGK.NTECKOPRZ.RTNFLG;
                        pay.responsedata = result;
                        pay.requestdata = request;
                        pay.hostFlowNo = response.CMBSDKPGK.NTECKOPRZ.REQNBR;
                        pay.isApproval = true;
                        pay.approvalMan = user.name;
                        pay.approvalDate = DateTime.Now;
                        pay.hostTxDate = DateTime.Now.ToString("yyyyMMdd");
                        dal.editPay(pay);//添加交易信息
                        ResultListModel resultListModel = JsonConvert.DeserializeObject<ResultListModel>(pay.requestModelYY);
                        resultListModel.acctDate = DateTime.Now;
                        YYDal yDal = new YYDal(user);
                        user.name = pay.createMan;//修改U8日记账人为发起单据者
                                                  /*if (pay.addrFlag == 1 && pay.unionFlag == "1")
                                                  {
                                                      return yDal.AddToAcctBook(resultListModel, user);
                                                  }
                                                  else
                                                  {*/
                        responsMessage.sucess = "已提交审批，请在手机端审核后在录入日记账";
                        return responsMessage;

                        /*  }*/
                    }
                    else
                    {
                        responsMessage.errorMsg = "银企直联接口暂时无法使用，请联系管理员..." + result;
                        return responsMessage;

                    }
                }
                else if (pay.outAcctAddr.Contains("平安"))
                {
                    string request;//返回的请求参数
                    string result;//返回的数据
                    PinganResponse<XferResponseModel> response;//返回的实体
                    XferRequestModel xferRequest = JsonConvert.DeserializeObject<XferRequestModel>(pay.requestModelYQ);//读取数据
                    PinganApi api = new PinganApi();
                    api.xFer(xferRequest, out request, out result, out response);//发起付款

                    if (result == null)
                    {//返回数据为空
                        responsMessage.errorMsg = "平安银企直联接口暂时无法使用或者平安接口服务未开启，请联系管理员...";
                        return responsMessage;
                    }
                    /* 如果行内转账不成功的话转成跨行，先取消
                     * if (response.code.Equals("RE1342"))
                     {//转成跨行
                        var newthirdVoucher = DateTime.Now.ToString("yyyyMMddHHssmm");
                         pay.unionFlag = "0";
                         pay.requestdata = pay.requestdata.Replace("<ThirdVoucher>" + pay.thirdVoucher + "</ThirdVoucher>", "<ThirdVoucher>" + newthirdVoucher + "</ThirdVoucher>");//生成新的流水号
                         pay.thirdVoucher = newthirdVoucher;
                         pay.requestdata = pay.requestdata.Replace("<UnionFlag>1</UnionFlag>", "<UnionFlag>0</UnionFlag>");//转成跨行
                         api.xFer_pay(pay.requestdata, out results, out response);//发起付款 
                         if (results==null)
                         {//返回数据为空
                             return new { errorMsg = "平安银企直联接口暂时无法使用或者平安接口服务未开启，请联系管理员..." };
                         }
                     }*/
                    dal.AddLog("平安付款", "add", request, result, user.name);//添加交易记录
                    if (response.code.Equals("000000"))
                    {//交易成功,银行受理
                        pay.frontLogNo = response.result.FrontLogNo;
                        pay.unionFlag = response.result.UnionFlag;
                        pay.stt = response.result.stt;
                        pay.fee1 = response.result.Fee1;
                        pay.fee2 = response.result.Fee2;
                        pay.hostFlowNo = response.result.hostFlowNo;
                        pay.hostTxDate = response.result.hostTxDate;
                        pay.isApproval = true;
                        pay.approvalMan = user.name;
                        pay.approvalDate = DateTime.Now;
                        pay.responsedata = result;
                        pay.requestdata = request;
                        dal.editPay(pay);//添加交易信息
                        ResultListModel resultListModel = JsonConvert.DeserializeObject<ResultListModel>(pay.requestModelYY);
                        resultListModel.acctDate = DateTime.Now;
                        YYDal yDal = new YYDal(user);
                        return yDal.AddToAcctBook(resultListModel, user);
                    }
                    else
                    {
                        var err = dal.Getyq_error(response.code);
                        // db.SaveChanges();
                        responsMessage.errorMsg = err == null ? "交易失败，请查看日志..." + result : err.errMessage;
                        return responsMessage;
                    }
                }
                else
                {
                    responsMessage.errorMsg = "该银行银企直联还未开通";
                    return responsMessage;
                }
            }
            else
            {
                responsMessage.errorMsg = "无权操作";
                return responsMessage;
            }
        }
        /// <summary>
        /// 交易状态更新
        /// </summary>
        /// <param name="ThirdVoucher"></param>
        /// <returns></returns>
        public PinganResponse<XferResponseModel> qryDtlByOrig(String ThirdVoucher, userInfo userInfo)
        {
            var pay = dal.yq_paymentRecordByThirdVoucher(ThirdVoucher);
            if (pay == null)
            {
                return new PinganResponse<XferResponseModel>() { message = "交易未发生或者非本系统发生交易" };
            }
            if (pay.outAcctAddr.Contains("平安") || pay.outAcctAddr.Contains("中信"))
            {
                string data, result;
                PinganResponse<XferResponseModel> response;
                PinganApi api = new PinganApi();
                api.qryDtlByOrigNew(new XferRequestModel() { ThirdVoucher = ThirdVoucher }, out response, out data, out result);
                dal.AddLog("查询更新交易记录4005", "4005", data, result, "admin");
                if (response != null && response.code == "000000")
                {
                    pay.frontLogNo = response.result.FrontLogNo;
                    pay.unionFlag = response.result.UnionFlag;
                    pay.stt = response.result.stt;
                    pay.fee1 = response.result.Fee1;
                    pay.fee2 = response.result.Fee2;
                    pay.hostFlowNo = response.result.hostFlowNo;
                    pay.hostTxDate = response.result.hostTxDate;
                    pay.isBack = response.result.isBack;
                    pay.backRem = response.result.backRem;
                    dal.editPay(pay);
                }
                return response;
            }
            if (pay.outAcctAddr.Contains("招商") || pay.outAcctAddr.Contains("招行"))
            {
                string data, result;
                ResonseClass response;
                if (pay.addrFlag == 1 && pay.unionFlag == "Y")
                {//同城 
                    INFO iNFO = new INFO()
                    {
                        FUNNAM = "NTQRYSTY",
                        DATTYP = "2",
                        LGNNAM = LGNNAM
                    };
                    //发起付款后需要联系银行才能付款
                    var NTQRYSTYX1 = new NTQRYSTYX1()
                    {
                        BUSCOD = "N02031",
                        YURREF = pay.thirdVoucher,
                        BGNDAT = pay.hostTxDate,
                        ENDDAT = pay.hostTxDate

                    };
                    var cmbc = new CMBSDKPGK()
                    {
                        INFO = iNFO,
                        NTQRYSTYX1 = NTQRYSTYX1
                    };
                    var api = new ZhaohangApi().CreateInstance();
                    api.zhaoHangApi(cmbc, out data, out result, out response);
                    if (string.IsNullOrEmpty(result))
                    {//代表没有插入key登录前置机
                        return new PinganResponse<XferResponseModel>
                        {
                            code = "500",
                            message = "请插如KEY再登录招商银行前置机"
                        };
                    }
                    dal.AddLog("查询更新交易记录NTQRYSTY", "NTQR", data, result, "admin");
                    if (!string.IsNullOrEmpty(response.CMBSDKPGK.INFO.ERRMSG))
                    {
                        return new PinganResponse<XferResponseModel>
                        {
                            code = response.CMBSDKPGK.INFO.RETCOD,
                            message = response.CMBSDKPGK.INFO.ERRMSG
                        };
                    }
                    if (response.CMBSDKPGK.NTSTLLSTZ == null || response.CMBSDKPGK.NTSTLLSTZ.Count == 0)
                    {
                        return new PinganResponse<XferResponseModel>
                        {
                            code = "404",
                            message = "未找到改交易，交易可能正在处理中请稍后再查询.或者转移到付款记录查询..."
                        };
                    }
                    pay.stt = string.IsNullOrEmpty(response.CMBSDKPGK.NTSTLLSTZ[0].RTNFLG) ? response.CMBSDKPGK.NTSTLLSTZ[0].REQSTS : response.CMBSDKPGK.NTSTLLSTZ[0].RTNFLG;
                    pay.hostFlowNo = response.CMBSDKPGK.NTSTLLSTZ[0].OPRDAT;
                    dal.editPay(pay);
                    PinganResponse<XferResponseModel> res = new PinganResponse<XferResponseModel>();
                    res.code = "000000";
                    res.message = response.CMBSDKPGK.INFO.ERRMSG;
                    return res;
                }
                else
                {//移动支票更新
                    INFO iNFO = new INFO()
                    {
                        FUNNAM = "NTECKTQY",
                        DATTYP = "2",
                        LGNNAM = LGNNAM
                    };
                    if (userInfo.company == (int)CompanyEnum.yueji)
                    {
                        iNFO.LGNNAM = yueJiLGNNAM;
                    }
                    DateTime dt = DateTime.ParseExact(pay.hostTxDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    //发起付款后需要联系银行才能付款
                    var NTECKTQYY = new NTECKTQYY()
                    {
                        BEGDAT = pay.hostTxDate,
                        ENDDAT = dt.AddDays(2).ToString("yyyyMMdd"),

                    };
                    var cmbc = new CMBSDKPGK()
                    {
                        INFO = iNFO,
                        NTECKTQYY = NTECKTQYY
                    };
                    var api = new ZhaohangApi().CreateInstance();
                    api.zhaoHangApi(cmbc, out data, out result, out response);
                    dal.AddLog("查询更新交易记录NTECKTQYZ", "NTEC", data, result, "admin");
                    if (!string.IsNullOrEmpty(response.CMBSDKPGK.INFO.ERRMSG))
                    {
                        return new PinganResponse<XferResponseModel>
                        {
                            code = response.CMBSDKPGK.INFO.RETCOD,
                            message = response.CMBSDKPGK.INFO.ERRMSG
                        };
                    }
                    if (response.CMBSDKPGK.NTECKTQYZ == null || response.CMBSDKPGK.NTECKTQYZ.Count == 0)
                    {
                        return new PinganResponse<XferResponseModel>
                        {
                            code = "404",
                            message = "未找到改交易，交易可能正在处理中请稍后再查询.或者转移到付款记录查询..."
                        };
                    }
                    foreach (var item in response.CMBSDKPGK.NTECKTQYZ)
                    {
                        pay.inAcctNo = pay.inAcctNo.Replace("\t", "");
                        if (item.RCVACC == pay.inAcctNo && item.TRSAMT == pay.tranAmount && item.BUSTXT == pay.useEx)
                        {

                            pay.stt = item.TRSSTS;
                            pay.hostFlowNo = item.TRSDAT;
                            pay.errMessage = item.ERRINF;
                            break;
                        }
                    }
                    dal.editPay(pay);
                    PinganResponse<XferResponseModel> res = new PinganResponse<XferResponseModel>();
                    res.code = "000000";
                    res.message = response.CMBSDKPGK.INFO.ERRMSG;
                    return res;
                }
            }
            return new PinganResponse<XferResponseModel>
            {
                code = "500",
                message = "请求错误！请重试..."
            };
        }
        /// <summary>
        /// 更新回单
        /// </summary>
        /// <returns></returns>
        public object GetImg(YqzlRequestModel request, userInfo userinfo)
        {
            request.EndDate = request.BeginDate;//银行默认只能查询一天内的，所以开始时间和结束时间设置一样
            YqzlResponseModel<PaysModel> response = new YqzlResponseModel<PaysModel>();
            response.result = new PaysModel();
            response.result.pays = new List<PayModel>();
            if (request.content.BankName == null)
            {
                response.code = "404";
                response.message = "未设置银行，请在U8账户管理设置银行";
                return response;

            }
            if (request.content.BankName.Contains("招商") || request.content.BankName.Contains("招行"))
            {//包含此关键字的为招商银行，调用招行API接口，读取招行付款记录
                INFO iNFO = new INFO()
                {
                    FUNNAM = "SDKCSFDFBRT",
                    DATTYP = "2",
                    LGNNAM = LGNNAM
                };
                var CSRRCFDFY0 = new CSRRCFDFY0()
                {
                    EACNBR = "755915712110113",

                    RRCFLG = "1",
                    BEGDAT = "20180218",
                    ENDDAT = "20180218",
                    // RRCCOD = "HD00014"

                };
                //这里测试招行统一用测试的账号
#if DEBUG
                CSRRCFDFY0.EACNBR = "755915712110113";//账号 755915712110113 755915712110815
                                                      //SDKTSINFX.BGNDAT = "20180218";
                                                      //SDKTSINFX.ENDDAT = "20180218";
#endif

                var cmbc = new CMBSDKPGK()
                {
                    INFO = iNFO,
                    CSRRCFDFY0 = CSRRCFDFY0
                };

                ZhaohangApi ZhaohangApi = new ZhaohangApi().CreateInstance();
                string requests;
                string results;
                ResonseClass result;
                ZhaohangApi.zhaoHangApi(cmbc, out requests, out results, out result);
                YqzlDal yqzl = new YqzlDal();
                yqzl.AddLog("回单图片下载", "down", requests, results, userinfo.name);//添加交易记录
            }
            else if (request.content.BankName.Contains("平安") || request.content.BankName.Contains("中信"))
            {

            }
            else
            {
                response.code = "404";
                response.message = "暂时还未开放该银行的银企直联";
            }
            return response;
        }
        /// <summary>
        /// 撤销交易
        /// </summary>
        /// <param name="liushuihao"></param>
        /// <returns></returns>
        public string RemovePay(string liushuihao)
        {
            return dal.RemovePay(liushuihao);
        }
        /// <summary>
        /// 获取移动支票授权人
        /// </summary>
        /// <returns></returns>
        public ResonseClass qryYidong()
        {
            string data, result;
            ResonseClass response;
            INFO iNFO = new INFO()
            {
                FUNNAM = "NTECKUSR",
                DATTYP = "2",
                LGNNAM = LGNNAM
            };
            var cmbc = new CMBSDKPGK()
            {
                INFO = iNFO
            };
            var api = new ZhaohangApi().CreateInstance();
            api.zhaoHangApi(cmbc, out data, out result, out response);
            //dal.AddLog("查询用户列表 ", "NTECKUSR", data, result, "admin");
            return response;
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ResonseClass ywms(string ywdm)
        {
            string data, result;
            ResonseClass response;
            INFO iNFO = new INFO()
            {
                FUNNAM = "ListMode",
                DATTYP = "2",
                LGNNAM = LGNNAM
            };
            var cmbc = new CMBSDKPGK()
            {
                INFO = iNFO,
                SDKMDLSTX = new SDKMDLSTX()
                {
                    BUSCOD = ywdm
                }
            };
            var api = new ZhaohangApi().CreateInstance();
            api.zhaoHangApi(cmbc, out data, out result, out response);
            //dal.AddLog("查询用户列表 ", "NTECKUSR", data, result, "admin");
            return response;
        }
        public PinganResponse<YlhModelResponse> yhdm(string shoukuanyh)
        {
            var yqzl = new YqzlDal();
            var yh = yqzl.yhCode(shoukuanyh);//获取收款单位
            PinganApi api = new PinganApi();
            string request, result;
            PinganResponse<YlhModelResponse> response;
            var cityList = yqzl.ecs_regionList(2);//获取所有的市（所有的市都不带市字所以直接对吧就可以了）
            ecs_region thisCity = new ecs_region();
            foreach (var city in cityList)
            {
                if (shoukuanyh.Contains(city.region_name.Replace("中国", "")))
                {
                    thisCity = city;
                    break;
                }
            }
            string key = "";
            if (thisCity == null || thisCity.region_id == 0)
            {//未设置支行信息，无法判断为哪个城市
                key = shoukuanyh.Substring(shoukuanyh.IndexOf("银行") + 2, shoukuanyh.Length - shoukuanyh.IndexOf("银行") - 2).Replace("支行", "").Replace("分行", "").Replace("县", "");
            }
            else
            {
                var cityIndex = shoukuanyh.IndexOf(thisCity.region_name);//所属城市开始位置
                var fenhangIndex = shoukuanyh.IndexOf("分行");//分行开始位置
                var zhihangIndex = shoukuanyh.IndexOf("支行");//支行开始位置
                                                            ///优先读取支行
                if (fenhangIndex != -1 && zhihangIndex != -1)
                {//有支行也有分行那么关键字去支行和分行间的字符
                    key = shoukuanyh.Substring(fenhangIndex + 2, zhihangIndex - fenhangIndex - 2);
                }
                else if (fenhangIndex == -1 && zhihangIndex != -1)
                {//有支行
                    key = shoukuanyh.Substring(cityIndex + thisCity.region_name.Length, zhihangIndex - cityIndex - thisCity.region_name.Length).Replace("市", "");
                }
                else if (fenhangIndex != -1 && zhihangIndex == -1)
                {//有分行
                    key = shoukuanyh.Substring(cityIndex + thisCity.region_name.Length, fenhangIndex - cityIndex - thisCity.region_name.Length).Replace("市", "");
                }
                else
                {

                }
                if (key == "")
                {
                    key = thisCity.region_name;
                }
            }
            key += "支行";
            YlhModel ylhModel = new YlhModel()
            {
                Request = new Request()
                {
                    BankName = yh.name,
                    BankNo = yh.code,
                    KeyWord = key
                }
            };
            api.getYLH(ylhModel, out request, out result, out response);
            if (thisCity != null && thisCity.region_id != 0)
            {
                List<YHHList> list = new List<YHHList>();
                foreach (var item in response.result.list)
                {
                    if (item.NodeName.Contains(thisCity.region_name))
                    {
                        list.Add(item);
                    }
                }
                response.result.list = list;
            }
            return response;
        }
        /// <summary>
        /// 回单下载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public object SdkcsfdfbrtImg(YqzlRequestModel request, userInfo userInfo)
        {
            YqzlResponseModel<PaysModel> response = new YqzlResponseModel<PaysModel>();
            if (userInfo.type != 3 && userInfo.type != 0)
            {
                response.code = "500";
                response.message = "无权操作...";
                return response;
            }
            request.EndDate = request.BeginDate;//银行默认只能查询一天内的，所以开始时间和结束时间设置一样
            response.result = new PaysModel();
            response.result.pays = new List<PayModel>();
            if (request.Reserve == null)
            {
                response.code = "404";
                response.message = "未设置银行，请在U8账户管理设置银行";
                return response;

            }
            if (request.Reserve.Contains("招商") || request.Reserve.Contains("招行"))
            {
                INFO iNFO = new INFO()
                {
                    FUNNAM = "SDKCSFDFBRTIMG",
                    DATTYP = "2",
                    LGNNAM = LGNNAM
                };
                if (userInfo.company == (int)CompanyEnum.yueji)
                {
                    iNFO.LGNNAM = yueJiLGNNAM;
                }
                var CSRRCFDFY0 = new CSRRCFDFY0()
                {
                    EACNBR = request.AcctNo,
                    RRCFLG = "1",
                    BEGDAT = request.BeginDate.ToString("yyyyMMdd"),
                    ENDDAT = request.EndDate.ToString("yyyyMMdd"),
                    // RRCCOD = "HD00014"

                };
                var cmbc = new CMBSDKPGK()
                {
                    INFO = iNFO,
                    CSRRCFDFY0 = CSRRCFDFY0
                };
                var api = new ZhaohangApi();
                string requests;
                string results;
                ResonseClass result;
                api.zhaoHangApi(cmbc, out requests, out results, out result);
                response.code = result.CMBSDKPGK.INFO.RETCOD;
                response.message = result.CMBSDKPGK.INFO.ERRMSG;
                return response;
            }
            else if (request.Reserve.Contains("平安"))
            {
                response.code = "404";
                response.message = "平安银行开发中...";
                return response;
            }
            else
            {
                response.code = "404";
                response.message = "该银行未开发";
                return response;
            }

        }
    }
}
