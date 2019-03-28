using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
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
    public class YYBll
    {
        private static string LGNNAM = ConfigurationManager.AppSettings["Zhaohang"].ToString();//招行登录账号

        private static string yueJiYidong = ConfigurationManager.AppSettings["yueJiYidong"].ToString();//悦肌招行手机登录账号

        private static string yueMuYidong = ConfigurationManager.AppSettings["yueMuYidong"].ToString();//悦目招行手机登录账号

        private static userInfo user;
        private static FromYYDal fromYY;
        private static YYDal yDal;
        public YYBll(userInfo users)
        {
            user = users;
            fromYY = new FromYYDal(user);
            yDal = new YYDal(user);
        }
        /// <summary>
        /// 获取付款账号
        /// </summary>
        /// <returns></returns>
        public List<Content> getAccInfo(int type)
        {
            type = type == 1 ? 0 : 1;//1为现金，2为银行日记账
            return fromYY.getAccInfo(type);//1:银行日记账,0:现金
        }
        /// <summary>
        /// 获取付款类型
        /// </summary>
        /// <returns></returns>
        public object getSettleStyle()
        {
            return fromYY.getSettleStyle();
        }
        /// <summary>
        /// 获取供应商或者是收款人
        /// </summary>
        /// <returns></returns>
        public object getReceivables(ResultListModel model)
        {
            var result = fromYY.geCustomertUnit(model.shoukuandanwei);//客户
            if (result != null)
            {
                return true;
            }
            result = fromYY.GeVendortUnit(model.shoukuandanwei);//供应商
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <returns></returns>
        public object getCodeList()
        {
            return fromYY.getCodeList();
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <returns></returns>
        public List<code> getCodeAllList()
        {
            return fromYY.getCodeAllList();
        }
        /// <summary>
        /// 获取流量
        /// </summary>
        /// <returns></returns>
        public object getProjectBycode(string citemcode)
        {
            return fromYY.getProjectBycode(citemcode);
        }
        /// <summary>
        /// 获取流量列表
        /// </summary>
        /// <returns></returns>
        public List<fitemss98> getProject()
        {
            return fromYY.getProject();
        }
        /// <summary>
        /// 添加收付款到U8(日记账):旧版不审核直接付款（遗弃）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object AddToAcctBook_old(ResultListModel model)
        {
            if (user.type == 0 || user.type == 1 || user.type == 3)
            {
                if (model.isPay == true)
                {

                    model.zhanhao = model.zhanhao.Replace(" ", "").Replace("-", "");//账号自动过滤掉空格，-
                    if (model.content.BankName.Contains("招商") || model.content.BankName.Contains("招行"))
                    {//包含此关键字的为招商银行，调用招行API接口，读取招行付款记录
                        INFO iNFO = new INFO()
                        {
                            FUNNAM = "DCPAYMNT",
                            DATTYP = "2",
                            LGNNAM = LGNNAM
                        };
                        var SDKPAYRQX = new SDKPAYRQX()
                        {
                            BUSCOD = "N02031"
                        };
                        var city = new YqzlDal().yq_cityCode_zhaohang(model.content.City);
                        //招行描述最多64，这里限制为20个字符
                        var message = model.piaoju[0].Yinhan.miaoshu.Replace("?", "").Replace("<", "").Replace(">", "").Replace("/", "").Replace(" ", "").Replace("；", "").Replace("-", "");
                        message = message.Length > 30 ? message.Substring(0, 30) : message.Substring(0, message.Length - 1);
                        var DCOPDPAYX = new DCOPDPAYX()
                        {
                            YURREF = DateTime.Now.ToString("yyyyMMddHHssmm"),
                            DBTACC = model.content.BankAcct,
                            DBTBBK = city.code,
                            TRSAMT = model.piaoju[0].Yinhan.jiefan,
                            CCYNBR = "10",
                            STLCHN = model.SysFlag.Equals("N") ? "N" : "F",
                            NUSAGE = message,
                            BNKFLG = model.UnionFlag.Equals("1") ? "Y" : "N",
                            CRTACC = model.zhanhao,
                            CRTNAM = model.shoukuandanwei,
                            CRTBNK = model.shoukuanyh,
                            CRTADR = model.shoukuanyh,
                            EPTDAT = DateTime.Now.ToString("yyyyMMdd")
                        };
                        //这里测试招行统一用测试的账号
#if DEBUG
                        DCOPDPAYX.DBTBBK = "75";
                        DCOPDPAYX.DBTACC = "755915712110113";//账号  755915712110113 755915712110815
                                                             //SDKTSINFX.BGNDAT = "20180218";
                                                             //SDKTSINFX.ENDDAT = "20180218";
#endif
                        var cmbc = new CMBSDKPGK()
                        {
                            INFO = iNFO,
                            SDKPAYRQX = SDKPAYRQX,
                            DCOPDPAYX = DCOPDPAYX
                        };
                        ZhaohangApi ZhaohangApi =new  ZhaohangApi().CreateInstance();
                        string requests;
                        string results;
                        ResonseClass response;
                        ZhaohangApi.zhaoHangApi(cmbc, out requests, out results, out response);
                        YqzlDal yqzl = new YqzlDal();
                        yqzl.AddLog("招行付款", "add", requests, results, user.name);//添加交易记录
                        if (!string.IsNullOrEmpty(response.CMBSDKPGK.INFO.ERRMSG))
                        {
                            return new { errorMsg = response.CMBSDKPGK.INFO.ERRMSG };
                        }
                        if (response.CMBSDKPGK.NTQPAYRQZ != null && response.CMBSDKPGK.NTQPAYRQZ.ERRTXT != null)
                        {
                            return new { errorMsg = response.CMBSDKPGK.NTQPAYRQZ.ERRTXT };
                        }
                        if (response.CMBSDKPGK.NTQPAYRQZ != null)
                        {//交易成功,银行受理
                            XferRequestModel requestModel = new XferRequestModel();
                            yq_paymentRecord paymentRecord = new yq_paymentRecord();
                            paymentRecord.payCode = "pay";
                            paymentRecord.thirdVoucher = cmbc.DCOPDPAYX.YURREF;
                            paymentRecord.cstInnerFlowNo = model.liuShui;
                            paymentRecord.ccyCode = "RMB";//10
                            paymentRecord.outAcctNo = cmbc.DCOPDPAYX.DBTACC;
                            paymentRecord.outAcctName = model.content.AcctName;
                            paymentRecord.outAcctAddr = model.content.BankName;
                            paymentRecord.inAcctNo = cmbc.DCOPDPAYX.CRTACC;
                            paymentRecord.inAcctName = cmbc.DCOPDPAYX.CRTNAM;
                            paymentRecord.inAcctBankName = cmbc.DCOPDPAYX.CRTBNK;
                            paymentRecord.tranAmount = cmbc.DCOPDPAYX.TRSAMT;
                            paymentRecord.useEx = cmbc.DCOPDPAYX.NUSAGE;
                            paymentRecord.unionFlag = cmbc.DCOPDPAYX.BNKFLG;
                            paymentRecord.sysFlag = cmbc.DCOPDPAYX.STLCHN;
                            paymentRecord.addrFlag = 1;
                            paymentRecord.createMan = user.name;
                            paymentRecord.frontLogNo = response.CMBSDKPGK.NTQPAYRQZ.REQNBR;
                            paymentRecord.unionFlag = model.UnionFlag;
                            paymentRecord.stt = string.IsNullOrEmpty(response.CMBSDKPGK.NTQPAYRQZ.RTNFLG) ? response.CMBSDKPGK.NTQPAYRQZ.REQSTS : response.CMBSDKPGK.NTQPAYRQZ.RTNFLG;
                            paymentRecord.fee1 = 0;
                            paymentRecord.fee2 = 0;
                            paymentRecord.hostFlowNo = response.CMBSDKPGK.NTQPAYRQZ.YURREF;
                            paymentRecord.hostTxDate = cmbc.DCOPDPAYX.EPTDAT;
                            paymentRecord.createTime = DateTime.Now;
                            yqzl.AddYq_paymentRecord(paymentRecord);//添加交易信息
                            return yDal.AddToAcctBook(model, user);
                        }
                        else
                        {
                            return new { errorMsg = "银企直联接口暂时无法使用，请联系管理员..." };
                        }
                    }
                    else if (model.content.BankName.Contains("平安") || model.content.BankName.Contains("中信"))
                    {
                        PinganApi api = new PinganApi();
                        XferRequestModel requestModel = new XferRequestModel()
                        {
                            ThirdVoucher = DateTime.Now.ToString("yyyyMMddHHssmm"),
                            CstInnerFlowNo = model.liuShui,
                            CcyCode = "RMB",
                            OutAcctNo = model.content.BankAcct,
                            OutAcctName = model.content.AcctName,
                            InAcctNo = model.zhanhao,
                            InAcctName = model.shoukuandanwei,
                            InAcctBankName = model.shoukuanyh,
                            TranAmount = model.piaoju[0].Yinhan.jiefan,
                            UseEx = model.piaoju[0].Yinhan.miaoshu.Replace("?", "").Replace("<", "").Replace(">", "").Replace("/", "").Replace(" ", "").Replace("；", "").Replace("-", ""),
                            UnionFlag = model.UnionFlag,
                            SysFlag = model.SysFlag,
                            AddrFlag = model.AddrFlag
                        };
#if DEBUG
                        requestModel.OutAcctNo = "11002923034501";
                        requestModel.OutAcctName = "平安测试六零零零三三八二八五九八";
#endif
                        string requests;
                        string results;
                        PinganResponse<XferResponseModel> response;
                        api.xFer(requestModel, out requests, out results, out response);
                        YqzlDal yqzl = new YqzlDal();
                        yqzl.AddLog("平安付款", "add", requests, results, user.name);//添加交易记录
                        if (response == null)
                        {
                            return new { errorMsg = "银企直联接口暂时无法使用，请联系管理员..." };
                        }
                        if (response.code.Equals("000000"))
                        {//交易成功,银行受理
                            yq_paymentRecord paymentRecord = new yq_paymentRecord();
                            paymentRecord.payCode = "4004";
                            paymentRecord.thirdVoucher = requestModel.ThirdVoucher;
                            paymentRecord.cstInnerFlowNo = requestModel.CstInnerFlowNo;
                            paymentRecord.ccyCode = requestModel.CcyCode;
                            paymentRecord.outAcctNo = requestModel.OutAcctNo;
                            paymentRecord.outAcctName = requestModel.OutAcctName;
                            paymentRecord.outAcctAddr = requestModel.OutAcctAddr;
                            paymentRecord.inAcctNo = requestModel.InAcctNo;
                            paymentRecord.inAcctName = requestModel.InAcctName;
                            paymentRecord.inAcctBankName = requestModel.InAcctBankName;
                            paymentRecord.tranAmount = requestModel.TranAmount;
                            paymentRecord.useEx = requestModel.UseEx;
                            paymentRecord.unionFlag = requestModel.UnionFlag;
                            paymentRecord.sysFlag = requestModel.SysFlag;
                            paymentRecord.addrFlag = requestModel.AddrFlag;
                            paymentRecord.createMan = requestModel.createMan;
                            paymentRecord.frontLogNo = response.result.FrontLogNo;
                            paymentRecord.unionFlag = response.result.UnionFlag;
                            paymentRecord.stt = response.result.stt;
                            paymentRecord.fee1 = response.result.Fee1;
                            paymentRecord.fee2 = response.result.Fee2;
                            paymentRecord.hostFlowNo = response.result.hostFlowNo;
                            paymentRecord.hostTxDate = response.result.hostTxDate;
                            paymentRecord.createTime = DateTime.Now;
                            yqzl.AddYq_paymentRecord(paymentRecord);//添加交易信息
                            return yDal.AddToAcctBook(model, user);
                        }
                        else
                        {
                            var err = yqzl.Getyq_error(response.code);
                            // db.SaveChanges();
                            return new { errorMsg = err.errMessage };
                        }
                    }
                    else
                    {
                        return yDal.AddToAcctBook(model, user);
                    }
                }
                else { return yDal.AddToAcctBook(model, user); }
            }
            else
            {
                return new { errorMsg = "无权操作" };
            }
        }
        /// <summary>
        /// 添加收付款到U8(日记账)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseMessage AddToAcctBook(ResultListModel model)
        {
            ResponseMessage responsMessage = new ResponseMessage();
            if (user.type == 0 || user.type == 1 || user.type == 3)
            {
                if (model.isPay)
                {
                    if (model.isYq== 1)
                    {//不启用银企直联付款
                        //先查询对应的OA号
                        return yDal.AddToAcctBook(model, user);
                    }
                    if (model.contentType==1)
                    {//现金直接记账

                        return yDal.AddToAcctBook(model, user);
                    }
                    model.zhanhao = Regex.Replace(model.zhanhao, @"[^0-9]+", "");//model.zhanhao.Replace(" ", "").Replace("    ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                    model.shoukuanyh = model.shoukuanyh.Replace(" ", "").Replace("-", "").Replace("\n", "").Replace("\t", "");//账号自动过滤掉空格，-
                    var message = model.piaoju[0].Yinhan.miaoshu
                        .Replace("预支", "").Replace("报销", "")
                        .Replace(model.faqibumen, "").Replace(model.faqiren, "").Replace(model.shoukuandanwei, "")
                        .Replace(model.shoukuandanwei,"").Replace("?", "").Replace("<", "")
                        .Replace(">", "").Replace("/", "").Replace("&", "").Replace("[", "")
                        .Replace(" ", "").Replace("；", "").Replace("]", "")
                        .Replace("-", "").Replace("\n", "").Replace("\t", "");
                    var dal = new YqzlDal();
                    var payOld = dal.yq_paymentRecordByOAliushuihao(model.liuShui);//读取代付款的交易
                    if (payOld != null)
                    {
                        responsMessage.errorMsg = "单据已提交审核，审核付款后将自动录入U8";
                        return responsMessage;
                    }
                    if (string.IsNullOrEmpty(model.content.BankName))
                    {
                        responsMessage.errorMsg = "改付款账号未设置开户行";
                        return responsMessage;

                    }
                    if (model.content.BankName.Contains("招商") || model.content.BankName.Contains("招行"))
                    {//包含此关键字的为招商银行，调用招行API接口，读取招行付款记录
                        
                        var city = new YqzlDal().yq_cityCode_zhaohang(model.content.City);
                        message = message.Length > 20 ? message.Substring(0, 20) : message.Substring(0, message.Length - 1);
                        if (model.AddrFlag == 1 && model.UnionFlag == "1" && (model.content.BankAcct== "120910321110801" || user.company!= (int)CompanyEnum.yueji))
                        {//悦肌只有这个120910321110801
                            INFO iNFO = new INFO()
                            {
                                FUNNAM = "DCPAYMNT",
                                DATTYP = "2",
                                LGNNAM = LGNNAM
                            };
                            //iNFO.LGNNAM = "银企直连测试用户45";
                            var SDKPAYRQX = new SDKPAYRQX()
                            {
                                BUSCOD = "N02031"
                            }; 
                            //招行描述最多64，这里限制为20个字符
                            var DCOPDPAYX = new DCOPDPAYX()
                            {
                                YURREF = DateTime.Now.ToString("yyyyMMddHHssmm"),
                                DBTACC = model.content.BankAcct,
                                DBTBBK = city == null ? "20" : city.code,//为空默认广州
                                TRSAMT = model.piaoju[0].Yinhan.jiefan,
                                CCYNBR = "10",
                                STLCHN = model.SysFlag.Equals("N") ? "N" : "F",
                                NUSAGE = message,
                                BNKFLG = model.UnionFlag.Equals("1") ? "Y" : "N",
                                CRTACC = model.zhanhao,
                                CRTNAM = model.shoukuandanwei,
                                CRTBNK = model.shoukuanyh,
                                CRTADR = model.shoukuanyh,
                                EPTDAT = DateTime.Now.ToString("yyyyMMdd")
                            };
                            if (model.shoukuanyh.Contains("招行") || model.shoukuanyh.Contains("招商"))
                            {
                                DCOPDPAYX.BNKFLG = "Y";
                            }
                            else
                            {
                                //DCOPDPAYX.BNKFLG = "N";

                                responsMessage.errorMsg = "非同行支付将收取手续费,请选择移动支票模式（改为跨行转账）";
                                return responsMessage;
                            }
                            //这里测试招行统一用测试的账号
#if DEBUG
                            DCOPDPAYX.DBTBBK = "20";
                            DCOPDPAYX.DBTACC = "120907722510701";//账号  755915712110113 755915712110815
                                                                 //SDKTSINFX.BGNDAT = "20180218";
                                                                 //SDKTSINFX.ENDDAT = "20180218";
                                                                 //   DCOPDPAYX.DBTBBK = "75";
                                                                 //   DCOPDPAYX.DBTACC = "755915712110113";
#endif
                            var cmbc = new CMBSDKPGK()
                            {
                                INFO = iNFO,
                                SDKPAYRQX = SDKPAYRQX,
                                DCOPDPAYX = DCOPDPAYX
                            };
                            // ZhaohangApi ZhaohangApi = new ZhaohangApi();
                            YqzlDal yqzl = new YqzlDal();
                            //交易成功,银行受理
                            yq_paymentRecord paymentRecord = new yq_paymentRecord
                            {
                                payCode = "pay",
                                thirdVoucher = cmbc.DCOPDPAYX.YURREF,
                                cstInnerFlowNo = model.liuShui,
                                ccyCode = "RMB",//10
                                outAcctNo = cmbc.DCOPDPAYX.DBTACC,
                                outAcctName = model.content.AcctName,
                                outAcctAddr = model.content.BankName,
                                inAcctNo = cmbc.DCOPDPAYX.CRTACC,
                                inAcctName = cmbc.DCOPDPAYX.CRTNAM,
                                inAcctBankName = cmbc.DCOPDPAYX.CRTBNK,
                                tranAmount = cmbc.DCOPDPAYX.TRSAMT,
                                useEx = cmbc.DCOPDPAYX.NUSAGE,
                                unionFlag = cmbc.DCOPDPAYX.BNKFLG,
                                sysFlag = cmbc.DCOPDPAYX.STLCHN,
                                addrFlag = model.AddrFlag,
                                createMan = user.name,
                                fee1 = 0,
                                fee2 = 0,
                                hostTxDate = cmbc.DCOPDPAYX.EPTDAT,
                                createTime = DateTime.Now,
                                isApproval = false,
                                requestModelYY = JsonConvert.SerializeObject(model),
                                requestModelYQ = JsonConvert.SerializeObject(cmbc),
                                company=user.company
                                //requestdata = ZhaohangApi.zhaoHangApi_requestData(cmbc)
                            };
                            if (yqzl.AddYq_paymentRecord(paymentRecord))
                            {
                                responsMessage.sucess = "提交审核成功，付款成功后将自动日记账";
                            }
                            else
                            {
                                responsMessage.errorMsg = "提交审核失败，请联系管理员查询错误日志";
                            }
                            return responsMessage;
                            //paymentRecord.stt = string.IsNullOrEmpty(response.CMBSDKPGK.NTQPAYRQZ.RTNFLG) ? response.CMBSDKPGK.NTQPAYRQZ.REQSTS : response.CMBSDKPGK.NTQPAYRQZ.RTNFLG;

                            // paymentRecord.hostFlowNo = response.CMBSDKPGK.NTQPAYRQZ.YURREF;
                            // paymentRecord.frontLogNo = response.CMBSDKPGK.NTQPAYRQZ.REQNBR;
                            // return yDal.AddToAcctBook(model, user);
                        }
                        else {//非同城行内的统一使用移动开票
                            INFO iNFO = new INFO()
                            {
                                FUNNAM = "NTECKOPR",
                                DATTYP = "2",
                                LGNNAM = LGNNAM
                            };
                            NTECKOPRX NTECKOPRX = new NTECKOPRX()
                            {
                                YURREF = DateTime.Now.ToString("yyyyMMddHHssmm"),
                                EACNBR = model.content.BankAcct,
                                BBKNBR = city == null ? "20" : city.code,//为空默认广州
                                CCYNBR = "10",
                                MAXAMT = model.piaoju[0].Yinhan.jiefan,
                                EFTDAT = DateTime.Now.ToString("yyyyMMdd"),
                                EXPDAT = DateTime.Now.AddDays(2).ToString("yyyyMMdd"),
                                ADDDAT = DateTime.Now.ToString("yyyyMMdd"),
                                AUTUSR = "N028570937",
                            };
                            NTECKOPRX.ECKNBR= DateTime.ParseExact(NTECKOPRX.YURREF, "yyyyMMddHHssmm", System.Globalization.CultureInfo.CurrentCulture).ToString("MMddHHssmm") ;
                            NTOPRMODX NTOPRMODX = new NTOPRMODX()
                            {
                                BUSMOD= "00002"
                            };
                            if (user.company == (int)CompanyEnum.yueji)
                            {
                                NTECKOPRX.AUTUSR = yueJiYidong;//黄晓东yueji2这个账号去审批
                            }
                            else
                            {
                                NTECKOPRX.AUTUSR = yueMuYidong;//否则使用悦目黄晓东yueji2这个账号去审批
                            }
                            if (string.IsNullOrEmpty(model.Dizhi))
                            {
                               // var citys = dal.ecs_regionPcity(model.shoukuanyh);
                                responsMessage.errorMsg = "开户行无法区分属于哪个省市,招行异地或者跨行需要填写城市";
                                return responsMessage;
                            }
                            NTECKRCVX NTECKRCVX = new NTECKRCVX()
                            {
                                SQRNBR = DateTime.Now.ToString("MMddHHssmm"),
                                RCVACC = model.zhanhao,
                                RCVNAM = model.shoukuandanwei,
                                SYSFLG = model.UnionFlag.Equals("1") ? "Y" : "N",
                                PAYCHN = "CPS",
                                // CDTBRD= model.UnionFlag.Equals("1") ? "Y" : "N",//汇路由网银去查
                                BNKADR = model.Dizhi,
                                RCVBBK = model.shoukuanyh,
                                STLCHN = "N",
                                NUSAGE = message,
                                BUSNAR=message,
                                //RCVNTF=""收款人手机
                            };
                            var cmbc = new CMBSDKPGK()
                            {
                                INFO = iNFO,
                                NTECKOPRX = NTECKOPRX,
                                NTECKRCVX = NTECKRCVX,
                                NTOPRMODX= NTOPRMODX
                            };
                            // ZhaohangApi ZhaohangApi = new ZhaohangApi();
                            YqzlDal yqzl = new YqzlDal();
                            yq_paymentRecord paymentRecord = new yq_paymentRecord
                            {
                                payCode = "pay",
                                thirdVoucher = cmbc.NTECKOPRX.YURREF,
                                cstInnerFlowNo = model.liuShui,
                                ccyCode = "RMB",//10
                                outAcctNo = model.content.BankAcct,
                                outAcctName = model.content.AcctName,
                                outAcctAddr = model.content.BankName,
                                inAcctNo = model.zhanhao,
                                inAcctName = model.shoukuandanwei,
                                inAcctBankName = model.shoukuanyh,
                                tranAmount = model.piaoju[0].Yinhan.jiefan,
                                useEx =message,
                                unionFlag = model.UnionFlag,
                                sysFlag =model.SysFlag,
                                addrFlag = model.AddrFlag,
                                createMan = user.name,
                                fee1 = 0,
                                fee2 = 0,
                                hostTxDate = DateTime.Now.ToString("yyyyMMdd"),
                                createTime = DateTime.Now,
                                isApproval = false,
                                requestModelYY = JsonConvert.SerializeObject(model),
                                requestModelYQ = JsonConvert.SerializeObject(cmbc),
                                company=user.company
                                //requestdata = ZhaohangApi.zhaoHangApi_requestData(cmbc)
                            };
                            if (yqzl.AddYq_paymentRecord(paymentRecord))
                            {
                                responsMessage.sucess = "提交审核成功，付款成功后将自动日记账";
                            }
                            else
                            {
                                responsMessage.errorMsg = "提交审核失败，请联系管理员查询错误日志";
                            }
                            return responsMessage;
                        }
                    }
                    else if (model.content.BankName.Contains("平安") || model.content.BankName.Contains("中信"))
                    {
                        YqzlDal yqzl = new YqzlDal();

                        /*var yh = yqzl.yhCode(model.shoukuanyh);//获取收款单位
                        if (yh==null)
                        {
                            responsMessage.errorMsg = "找不到付款银行无法付款，请核对收款银行";
                            return responsMessage;
                        }
                        PinganApi api = new PinganApi();
                        string request, result;
                        PinganResponse<YlhModelResponse> response;
                        //获取所在的省
                        var cityList = yqzl.ecs_regionList(2);//获取所有的市（所有的市都不带市字所以直接对吧就可以了）
                        ecs_region thisCity=new ecs_region();
                        foreach (var city in cityList)
                        {
                            if (model.shoukuanyh.Contains(city.region_name))
                            {
                                thisCity = city;
                                break;
                            }
                        }
                        if (thisCity == null)
                        {//未设置支行信息，无法判断为哪个城市
                            responsMessage.errorMsg = "找不到所属城市，请联系管理员！";
                            return responsMessage;
                        }
                        var quList = yqzl.ecs_regionListByPid(thisCity.region_id);//获取所有的区
                        ecs_region thisQu = new ecs_region();
                        foreach (var qu in quList)
                        {
                            if (model.shoukuanyh.Contains(qu.region_name.Replace("市", "").Replace("县", "").Replace("区", "")))
                            {
                                thisQu = qu;
                                break;
                            }
                        }
                        if (thisQu==null)
                        {
                            thisQu = thisCity;//强制传城市过去，然后读取第一个城市作为开户行
                        }
                        YlhModel ylhModel = new YlhModel() {
                            Request=new Request() {
                                BankName=yh.name,
                                BankNo=yh.code,
                                KeyWord= thisQu.region_name.Replace("市", "").Replace("县", "").Replace("区", "")
                            }
                        };
                        api.getYLH(ylhModel, out request, out result, out response);
                        if (response.result.size==0)
                        {
                            responsMessage.errorMsg = "找不到付款银行银联号";
                            return responsMessage;
                        }*/

                        XferRequestModel requestModel = new XferRequestModel()
                        {
                            ThirdVoucher = DateTime.Now.ToString("yyyyMMddHHssmm"),
                            CstInnerFlowNo = model.liuShui,
                            CcyCode = "RMB",
                            OutAcctNo = model.content.BankAcct,
                            OutAcctName = model.content.UnitName,
                            OutAcctAddr=model.content.BankName,
                            InAcctNo = model.zhanhao,
                            InAcctName = model.shoukuandanwei,
                            InAcctBankName = model.shoukuanyh,
                            TranAmount = model.piaoju[0].Yinhan.jiefan,
                            UseEx =message,
                            //InAcctRecCode=model.yqCode,//跨行需要输入，或者输入省市
                            UnionFlag = model.UnionFlag,
                            SysFlag = model.SysFlag,
                            AddrFlag = model.AddrFlag
                        };
                        if (!string.IsNullOrEmpty(model.yqCode))
                        {//跨行需要输入银联号，否则无法实时落地打款
                            requestModel.InAcctRecCode = model.yqCode;
                        }
                        if (model.shoukuanyh.Contains("平安") || model.shoukuanyh.Contains("中信"))
                        {
                            requestModel.UnionFlag = "1";
                        }
                        else
                        {
                            requestModel.UnionFlag = "0";
                        }
#if DEBUG
                       // requestModel.OutAcctNo = "11014901041000";
                       // requestModel.OutAcctName = "广东悦肌化妆品有限公司";
#endif

                       

                        yq_paymentRecord paymentRecord = new yq_paymentRecord
                        {
                            payCode = "4004",
                            thirdVoucher = requestModel.ThirdVoucher,
                            cstInnerFlowNo = requestModel.CstInnerFlowNo,
                            ccyCode = requestModel.CcyCode,
                            outAcctNo = requestModel.OutAcctNo,
                            outAcctName = requestModel.OutAcctName,
                            outAcctAddr = requestModel.OutAcctAddr,
                            inAcctNo = requestModel.InAcctNo,
                            inAcctName = requestModel.InAcctName,
                            inAcctBankName = requestModel.InAcctBankName,
                            tranAmount = requestModel.TranAmount,
                            useEx = requestModel.UseEx,
                            unionFlag = requestModel.UnionFlag,
                            sysFlag = requestModel.SysFlag,
                            addrFlag = requestModel.AddrFlag,
                            createMan = requestModel.createMan,
                            requestModelYY = JsonConvert.SerializeObject(model),
                            requestModelYQ = JsonConvert.SerializeObject(requestModel),
                            //requestdata =api.xFer_requestdata(requestModel),
                            isApproval=false,
                            //  paymentRecord.frontLogNo = response.result.FrontLogNo;
                            //  paymentRecord.unionFlag = response.result.UnionFlag;
                            // paymentRecord.stt = response.result.stt;
                            // paymentRecord.fee1 = response.result.Fee1;
                            // paymentRecord.fee2 = response.result.Fee2;
                            // paymentRecord.hostFlowNo = response.result.hostFlowNo;
                            //paymentRecord.hostTxDate = response.result.hostTxDate;
                            createTime = DateTime.Now,
                            company=user.company
                        };
                        if (yqzl.AddYq_paymentRecord(paymentRecord))
                        {
                            responsMessage.sucess = "提交审核成功，付款成功后将自动日记账";
                        }
                        else
                        {
                            responsMessage.errorMsg = "提交审核失败，请联系管理员查询错误日志";
                        }
                        return responsMessage;
                    }
                    else
                    {//公司其他银行的付款还未创建银企直联，第三方付款后直接录入U8
                        return yDal.AddToAcctBook(model, user);
                    }
                }
                else {//收款操作，读取到后直接录入U8
                    var s= yDal.AddToAcctBook(model, user);//日记账
                    var isShendan = model.piaoju[0].Detail;
                    foreach (var item in model.piaoju)
                    {
                        foreach (var de in item.Detail)
                        {
                           /* if (de.kemu.Equals())
                            {

                            }*/
                        }
                    }
                    return s;
                }
            }
            else
            {
                responsMessage.errorMsg="无权操作";
                return responsMessage;
            }
        }

        /// <summary>
        /// 生单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object addToCloseBill(ResultListModel model)
        {

            if (user.type == 1 || user.type == 3)
            {
                return yDal.addToCloseBill(model, user);
            }
            else
            {
                return new { errorMsg = "无权操作" };
            }
        }
        /// <summary>
        /// 制单（不带签字）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object addToCashSignRelate(ResultListModel model)
        {

            if (user.type == 1 || user.type == 3)
            {
                return yDal.addToCashSignRelate(model, user);
            }
            else
            {
                return new { errorMsg = "无权操作" };
            }
        }
        /// <summary>
        /// 签字
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object addToCcashier(ResultListModel model)
        {
            return yDal.addToCcashier(model);
        }
        /// <summary>
        /// 获取付款的单位（已生成付款单）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object getRecordTableByPid(ResultListModel model)
        {
            FromYYDal fromYY = new FromYYDal(user);
            return fromYY.getRecordTableByPid(model.Id);
        }
        /// <summary>
        /// 获取U8部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object getDepatementList()
        {
            return fromYY.getDepatementList();
        }
        /// <summary>
        /// 根据收款人、单位获取客户、供应商
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object getUnitType(string shoukuandanwei)
        {
            return fromYY.getUnitType(shoukuandanwei);
        }

    }
}
