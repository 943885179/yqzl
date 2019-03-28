using common;
using EntityFromework;
using Helpers;
using Log;
using model;
using Model;
using MOFA_U8API_API.dingdan_input;
using MOFA_U8API_API.model.shoukuandan;
using MOFA_U8API_DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace dal
{
    /// <summary>
    /// 用友
    /// </summary>
    public class YYDal
    {
        private string config = ConfigurationManager.AppSettings["UFDATA_046_2018Entities"].ToString();

        private static string oaToU8config = ConfigurationManager.AppSettings["OAtoU8DATAEntities"].ToString();
        private FromYYDal u8;
        private string u8Db = "UFDATA_046_2018Entities";
        private string coutaccset = "046";
        /// <summary>
        /// 重新构造，根据登录用户的公司来读取相应的数据库
        /// </summary>
        /// <param name="userInfo"></param>
        public YYDal(userInfo userInfo)
        {
            if (userInfo.company == (int)CompanyEnum.yuemu)
            {
                /*this.u8Db = "UFDATA_114_2018Entities";
                this.config = ConfigurationManager.AppSettings["UFDATA_114_2018Entities"].ToString();
                this.coutaccset = "114";*/
                this.coutaccset = "046";
                this.u8Db = "UFDATA_046_2018Entities";
                this.config = ConfigurationManager.AppSettings["UFDATA_046_2018Entities"].ToString();

            }
            if (userInfo.company == (int)CompanyEnum.yueji)
            {
                coutaccset = "048";
                this.u8Db = "UFDATA_048_2018Entities";
                config = ConfigurationManager.AppSettings["UFDATA_048_2018Entities"].ToString();
            }
            if (userInfo.company == (int)CompanyEnum.yuehui)
            {
                coutaccset = "050";
                this.u8Db = "UFDATA_050_2018Entities";
                config = ConfigurationManager.AppSettings["UFDATA_050_2018Entities"].ToString();
            }
            if (userInfo.company == (int)CompanyEnum.yuezhuang)
            {
                coutaccset = "047";
                this.u8Db = "UFDATA_047_2018Entities";
                config = ConfigurationManager.AppSettings["UFDATA_047_2018Entities"].ToString();
            }
            if (userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
            {
                coutaccset = "049";
                this.u8Db = "UFDATA_049_2017Entities";
                config = ConfigurationManager.AppSettings["UFDATA_049_2017Entities"].ToString();
            }
            if (userInfo.company == (int)CompanyEnum.ceshi)
            {
                this.u8Db = "UFDATA_114_2018Entities";
                this.config = ConfigurationManager.AppSettings["UFDATA_114_2018Entities"].ToString();
                this.coutaccset = "114";
            }
            u8 = new FromYYDal(userInfo);
        }
        #region 生成日记账
        /// <summary>
        /// 单据只做付款操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public ResponseMessage AddToAcctBook(ResultListModel model, userInfo userInfo)
        {
            ResponseMessage responseMessage = new ResponseMessage();
            if (userInfo.name == "胡姗_admin")
            {
                userInfo.name = "胡珊_admin";
            }
            //日记账首先要保证产生流量，产生流量就要选择银行账户
            if (model.content == null || model.content.ID == 0)
            {
                responseMessage.errorMsg = "请选择账户";
                return responseMessage;
            }
            using (var ent = new OAtoU8DATAEntities())
            {
                //读取中间记录表，判读是否生成了日记账
                var oldJournal = (from r in ent.RecordTable
                                  where r.Pid.Contains(model.Id)
                                  select r).FirstOrDefault();
                if (oldJournal != null)
                {
                    responseMessage.errorMsg = "请勿重复提交！";
                    return responseMessage;
                }
            }
            //收款单位是供应商还是客户，用收款单位来确定
            CN_UnitID customerOrSupplier = u8.geCustomertUnit(model.shoukuandanwei);//客户或者供应商
            CN_AcctBook acctBook = new CN_AcctBook
            {
                UnitType = 1//客户为1，供应商为2,根据u8上默认是选择客户的
            };
            if (customerOrSupplier != null)
            {
                acctBook.UnitID = customerOrSupplier.ID;
                acctBook.CustomerID = customerOrSupplier.ID;
            }
            else
            {
                customerOrSupplier = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                if (customerOrSupplier != null)
                {
                    acctBook.UnitType = 2;//客户为1，供应商为2
                    acctBook.UnitID = customerOrSupplier.ID;
                    acctBook.VendorID = customerOrSupplier.ID;
                }
            }
            var user = u8.getPerson(model.shoukuandanwei);//个人支付
            acctBook.AcctID = model.content.ID;//getAccInfo传过来的Id,账户
            acctBook.AcctType = 1;//日记账类型
            acctBook.AcctDate = Convert.ToDateTime(model.acctDate.ToString("yyyy-MM-dd"));//添加时间
            acctBook.Period = model.acctDate.Month;//期数
            acctBook.ExchangeRate = 1;//汇率 
            acctBook.SettleTypeID = model.contentType;//转账或者现金（统一设置为转账）
            acctBook.Cashier = userInfo.name.Replace("_admin", "");//日记账人员
            acctBook.ProjectClass = u8.getLervelClass("现金流量项目").ClassID_N;//TODO:下一步将会前端展示这个下拉，而非在这固定死
            acctBook.lYear = model.acctDate.Year;//年份设为当前年
            acctBook.lParentID = model.content.ID;//出纳选择银行
                                                  //CN_AcctInfo cont = u8.getAccInfo(model.content,2);
            string left = "";//付前面的数字
            if (userInfo.company == (int)CompanyEnum.yueji)
            {//悦肌的需要截取前面六位做标识
                left = model.content.AcctName.Substring(0, 6);
            }
            acctBook.CSNCashSign = model.isPay ? u8.getCashSerialNumber(left + "付") : u8.getCashSerialNumber(left + "收");//收付款标记，TODO:下一步将会前端展示，我们将从前端取值
            //暂时只做支付的，收款的不做，付款的编号确定，如果需要更新，请查询getCashSerialNumber（context）
            acctBook.DeptID = u8.UnitList().Where(o => o.cCusName == model.shoukuandanwei).FirstOrDefault()?.DeptID;// 
            u8.getDepatement(model.faqibumen);//发起部门为录入者部门，但是OA上的部门可能在OA上搜索不到，TODO:下一步会将U8的部门前端展示选择
            //悦目不用管部门，悦肌要，根据收款单位找到对应的部门和客户，
            /*if (acctBook.DeptID == 0) {//报销部门在U8上找不到
                return new { errorMsg = model.faqibumen + "在U8上找不到" };
            }*/
            //一些选项没有值得话默认为0，不然不会显示出来
            acctBook.Debit = acctBook.Debit == null ? 0.0000M : acctBook.Debit;
            acctBook.FDebit = acctBook.FDebit == null ? 0.0000M : acctBook.FDebit;
            acctBook.PersonID = acctBook.PersonID == null ? 0 : acctBook.PersonID;
            acctBook.SourceType = acctBook.SourceType == null ? 0 : acctBook.SourceType;
            acctBook.IsAudited = acctBook.IsAudited == null ? 0 : acctBook.IsAudited;
            acctBook.IsDelete = acctBook.IsDelete == null ? 0 : acctBook.IsDelete;
            acctBook.IsRegGLVouch = acctBook.IsRegGLVouch == null ? 0 : acctBook.IsRegGLVouch;
            acctBook.RectifyID = acctBook.RectifyID == null ? 0 : acctBook.RectifyID;
            acctBook.RectifyType = acctBook.RectifyType ?? "0";
            acctBook.CACheckFlag = acctBook.CACheckFlag == null ? 0 : acctBook.CACheckFlag;
            acctBook.CBCheckFlag = acctBook.CBCheckFlag == null ? 0 : acctBook.CBCheckFlag;
            acctBook.CustomCol1 = acctBook.CustomCol1 == null ? 0 : acctBook.CustomCol1;
            acctBook.CustomCol2 = acctBook.CustomCol2 == null ? 0 : acctBook.CustomCol2;
            acctBook.CustomCol3 = acctBook.CustomCol3 == null ? 0 : acctBook.CustomCol3;
            acctBook.CashNum = acctBook.CashNum == null ? 0 : acctBook.CashNum;
            acctBook.CustomerID = acctBook.CustomerID == null ? 0 : acctBook.CustomerID;
            acctBook.VendorID = acctBook.VendorID == null ? 0 : acctBook.VendorID;
            acctBook.cDefine7 = acctBook.cDefine7 == null ? 0 : acctBook.cDefine7;
            acctBook.cDefine15 = acctBook.cDefine15 == null ? 0 : acctBook.cDefine15;
            acctBook.cDefine16 = acctBook.cDefine16 == null ? 0 : acctBook.cDefine16;
            acctBook.RateCalType = acctBook.RateCalType == null ? 0 : acctBook.RateCalType;
            acctBook.VoucherNum = 0;
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        for (int i = 0; i < model.piaoju.Count; i++)
                        {
                            Piaoju piaoju = model.piaoju[i];
                            if (piaoju.Yinhan.jiefan == 0)
                            {//票据无借支，未产生流量不用日记账，跳过
                                continue;
                            }
                            #region 验证单据合法性
                            //读取票据明细，如果存在受控科目等需要先判断，不合法无法提交
                            //录入日记账时候添加科目控制，在当科目符合的时候才能插入，不能的话说明财务审核不认真科目没有核实正确
                            //出纳日记账不再限制供应商，强制录入
                            /*  foreach (Detail list in piaoju.Detail)
                              {
                                  if (string.IsNullOrEmpty(list.kemu)) {//过来掉空科目
                                      continue;
                                  }
                                  //OA上的科目定义为U8的科目和它所对应的流量，如果不确定流量编码则统统为00
                                  var kemu = list.kemu.Substring(0, list.kemu.Length - 2);//获取科目
                                  var thisCode = u8.getCodeInU8Byccode(kemu);
                                  if (thisCode == null || piaoju.Yinhan.jiefan==0)
                                  {//没找到这个科目
                                      continue;
                                  }
                                  if (thisCode.bcus && customerOrSupplier == null)
                                  {//客户往来核算 
                                      return new { errorMsg = thisCode.ccode + "是客户受控科目，但是在客户列表中无法查询到" + model.shoukuandanwei + "这个客户" };
                                  }
                                  if (thisCode.bsup && customerOrSupplier == null)
                                  {//供应商往来核算项目 
                                      return new { errorMsg = thisCode.ccode + "是供应商受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "这个供应商" };
                                  }
                                  if (thisCode.bperson && user == null)
                                  {//个人往来核算项目 
                                      return new { errorMsg = thisCode.ccode + "是个人往来受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "" };
                                  }
                                  if (thisCode.bdept && string.IsNullOrEmpty(list.shouyibumen))
                                  {//部门往来核算项目 
                                      return new { errorMsg = thisCode.ccode + "部门受控,请选择部门" };
                                  }
                              }*/
                            #endregion
                            acctBook.Credit = model.isPay ? piaoju.Yinhan.jiefan : 0.0000M;//需要银行支付的支付金额
                            acctBook.FCredit = model.isPay ? piaoju.Yinhan.jiefan : 0.0000M;//需要银行支付的支付金额
                            acctBook.Debit = !model.isPay ? piaoju.Yinhan.jiefan : 0.0000M;//账号收到的金额
                            acctBook.FDebit = !model.isPay ? piaoju.Yinhan.jiefan : 0.0000M;//账号收到的金额
                            acctBook.Summary = piaoju.Yinhan.miaoshu.Replace("/", "").Replace(" ", "").Replace("；", "").Replace("-", "");//描述
                            //获取最后一个录入的编号
                            var dateFirst = new DateTime(model.acctDate.Year, model.acctDate.Month, 1);//本月第一天
                            var lastAcctacctBook = (from b in db.CN_AcctBook
                                                    where b.AcctID == model.content.ID && b.CSNCashSign == acctBook.CSNCashSign && b.AcctDate >= dateFirst
                                                    orderby b.CSNCashID descending
                                                    select b
                                       ).FirstOrDefault();
                            acctBook.CSNCashID = lastAcctacctBook == null ? 1 : lastAcctacctBook.CSNCashID + 1;//编号自动加一
                            db.CN_AcctBook.Add(acctBook);
                            db.SaveChanges();
                            var newacctBook = (from b in db.CN_AcctBook
                                               where b.ID == acctBook.ID
                                               select b).FirstOrDefault();
                            acctBook.ID_Old = acctBook.ID;
                            db.SaveChanges();
                            var chunabianhao = acctBook.CSNCashSign + "-" + acctBook.CSNCashID.ToString().PadLeft(5, '0');
                            model.piaoju[i].chunabianhao = chunabianhao;
                            if (chunabianhao == null || chunabianhao == "")
                            {
                                responseMessage.errorMsg = "操作太频繁！";
                                return responseMessage;
                            }
                            using (var ent = new OAtoU8DATAEntities())
                            {//插入到自己的数据库表进行记录
                                RecordTable record = new RecordTable
                                {
                                    IsIntoBook = 1,
                                    piaojuId = i,
                                    Pid = model.Id,
                                    Bid = acctBook.ID,
                                    ip = userInfo.name,
                                    updateTime = model.acctDate,
                                    contents = model.content.ID.ToString(),
                                    chunabianhao = chunabianhao,
                                    liushuihao = model.liuShui,
                                    yyDate = Convert.ToDateTime(model.acctDate.ToString("yyyy-MM-dd"))
                                };
                                record.piaojuId = piaoju.tabClass;
                                ent.RecordTable.Add(record);
                                ent.SaveChanges();
                            }
                        }
                        tran.Commit();
                        foreach (var piaoju in model.piaoju)
                        {
                            Log4Helper.Info(typeof(YYDal), "日记账成功,出纳编号为" + piaoju.chunabianhao);
                        }
                        responseMessage.sucess = "成功生成日记账！";
                        responseMessage.model = model;
                        return responseMessage;
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                        tran.Rollback();
                        responseMessage.errorMsg = "U8生成日记账错误！";
                        return responseMessage;
                    }
                }

            }
        }
        #endregion
        #region 生成日记账
        /// <summary>
        /// 单据只做收款操作
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public object addToAcctBookByIn(ResultListModel model, userInfo userInfo)
        {
            if (userInfo.name == "胡姗_admin")
            {
                userInfo.name = "胡珊_admin";
            }
            //日记账首先要保证产生流量，产生流量就要选择银行账户
            if (model.content == null || model.content.ID == 0)
            {
                return new { errorMsg = "请选择账户" };
            }
            using (var ent = new OAtoU8DATAEntities())
            {
                //读取中间记录表，判读是否生成了日记账
                var oldJournal = (from r in ent.RecordTable
                                  where r.Pid.Contains(model.Id)
                                  select r).FirstOrDefault();
                if (oldJournal != null)
                {
                    return new { errorMsg = "数据已经通过本系统提交到U8，请勿重复提交！" };
                }
            }
            //收款单位是供应商还是客户，用收款单位来确定
            CN_UnitID customerOrSupplier = u8.geCustomertUnit(model.shoukuandanwei);//客户或者供应商
            CN_AcctBook acctBook = new CN_AcctBook
            {
                UnitType = 1//客户为1，供应商为2,根据u8上默认是选择客户的
            };
            if (customerOrSupplier != null)
            {
                acctBook.UnitID = customerOrSupplier.ID;
                acctBook.CustomerID = customerOrSupplier.ID;
            }
            else
            {
                customerOrSupplier = u8.GeVendortUnit(model.shoukuandanwei);//赌气供应商
                if (customerOrSupplier != null)
                {
                    acctBook.UnitType = 2;//客户为1，供应商为2
                    acctBook.UnitID = customerOrSupplier.ID;
                    acctBook.VendorID = customerOrSupplier.ID;
                }
            }
            var user = u8.getPerson(model.shoukuandanwei);//个人支付
            acctBook.AcctID = model.content.ID;//getAccInfo传过来的Id,账户
            acctBook.AcctType = 1;//日记账类型
            acctBook.AcctDate = Convert.ToDateTime(model.acctDate.ToString("yyyy-MM-dd"));//添加时间
            acctBook.Period = model.acctDate.Month;//期数
            acctBook.ExchangeRate = 1;//汇率 
            acctBook.SettleTypeID = 2;//转账或者现金（统一设置为转账）
            acctBook.Cashier = userInfo.name.Replace("_admin", "");//日记账人员
            acctBook.ProjectClass = u8.getLervelClass("现金流量项目").ClassID_N;//TODO:下一步将会前端展示这个下拉，而非在这固定死
            acctBook.lYear = model.acctDate.Year;//年份设为当前年
            acctBook.lParentID = model.content.ID;//出纳选择银行
                                                  //   CN_AcctInfo cont = getAccInfo(model.content,2);
            acctBook.CSNCashSign = u8.getCashSerialNumber("收");//收付款标记，TODO:下一步将会前端展示，我们将从前端取值
            //暂时只做支付的，收款的不做，付款的编号确定，如果需要更新，请查询getCashSerialNumber（context）
            acctBook.DeptID = null;// u8.getDepatement(model.faqibumen);//发起部门为录入者部门，但是OA上的部门可能在OA上五福搜索到，TODO:下一步会将U8的部门前端展示选择
            /*if (acctBook.DeptID == 0) {//报销部门在U8上找不到
                return new { errorMsg = model.faqibumen + "在U8上找不到" };
            }*/
            //一些选项没有值得话默认为0，不然不会显示出来
            acctBook.Debit = acctBook.Debit == null ? 0.0000M : acctBook.Debit;
            acctBook.FDebit = acctBook.FDebit == null ? 0.0000M : acctBook.FDebit;
            acctBook.PersonID = acctBook.PersonID == null ? 0 : acctBook.PersonID;
            acctBook.SourceType = acctBook.SourceType == null ? 0 : acctBook.SourceType;
            acctBook.IsAudited = acctBook.IsAudited == null ? 0 : acctBook.IsAudited;
            acctBook.IsDelete = acctBook.IsDelete == null ? 0 : acctBook.IsDelete;
            acctBook.IsRegGLVouch = acctBook.IsRegGLVouch == null ? 0 : acctBook.IsRegGLVouch;
            acctBook.RectifyID = acctBook.RectifyID == null ? 0 : acctBook.RectifyID;
            acctBook.RectifyType = acctBook.RectifyType ?? "0";
            acctBook.CACheckFlag = acctBook.CACheckFlag == null ? 0 : acctBook.CACheckFlag;
            acctBook.CBCheckFlag = acctBook.CBCheckFlag == null ? 0 : acctBook.CBCheckFlag;
            acctBook.CustomCol1 = acctBook.CustomCol1 == null ? 0 : acctBook.CustomCol1;
            acctBook.CustomCol2 = acctBook.CustomCol2 == null ? 0 : acctBook.CustomCol2;
            acctBook.CustomCol3 = acctBook.CustomCol3 == null ? 0 : acctBook.CustomCol3;
            acctBook.CashNum = acctBook.CashNum == null ? 0 : acctBook.CashNum;
            acctBook.CustomerID = acctBook.CustomerID == null ? 0 : acctBook.CustomerID;
            acctBook.VendorID = acctBook.VendorID == null ? 0 : acctBook.VendorID;
            acctBook.cDefine7 = acctBook.cDefine7 == null ? 0 : acctBook.cDefine7;
            acctBook.cDefine15 = acctBook.cDefine15 == null ? 0 : acctBook.cDefine15;
            acctBook.cDefine16 = acctBook.cDefine16 == null ? 0 : acctBook.cDefine16;
            acctBook.RateCalType = acctBook.RateCalType == null ? 0 : acctBook.RateCalType;
            acctBook.VoucherNum = 0;
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {

                        for (int i = 0; i < model.piaoju.Count; i++)
                        {
                            Piaoju piaoju = model.piaoju[i];
                            acctBook.Summary = piaoju.Yinhan.miaoshu.Replace("/", "").Replace(" ", "").Replace("；", "").Replace("-", "");//描述
                            //获取最后一个录入的编号
                            var lastAcctacctBook = (from b in db.CN_AcctBook
                                                    where b.AcctID == model.content.ID
                                                    orderby b.CSNCashID descending
                                                    select b
                                       ).FirstOrDefault();
                            acctBook.CSNCashID = lastAcctacctBook == null ? 1 : lastAcctacctBook.CSNCashID + 1;//编号自动加一
                            db.CN_AcctBook.Add(acctBook);
                            db.SaveChanges();
                            var newacctBook = (from b in db.CN_AcctBook
                                               where b.ID == acctBook.ID
                                               select b).FirstOrDefault();
                            acctBook.ID_Old = acctBook.ID;
                            db.SaveChanges();
                            var chunabianhao = acctBook.CSNCashSign + "-" + acctBook.CSNCashID.ToString().PadLeft(5, '0');
                            model.piaoju[i].chunabianhao = chunabianhao;
                            if (chunabianhao == null || chunabianhao == "")
                            {
                                return new { errorMsg = "你操作太快了，请重试！" };
                            }
                            using (var ent = new OAtoU8DATAEntities())
                            {//插入到自己的数据库表进行记录
                                RecordTable record = new RecordTable
                                {
                                    IsIntoBook = 1,
                                    piaojuId = i,
                                    Pid = model.Id,
                                    Bid = acctBook.ID,
                                    ip = userInfo.name,
                                    updateTime = model.acctDate,
                                    contents = model.content.ID.ToString(),
                                    chunabianhao = chunabianhao
                                };
                                record.piaojuId = piaoju.tabClass;
                                ent.RecordTable.Add(record);
                                ent.SaveChanges();
                            }
                        }
                        tran.Commit();
                        foreach (var piaoju in model.piaoju)
                        {
                            Log4Helper.Info(typeof(YYDal), "日记账成功,出纳编号为" + piaoju.chunabianhao);
                        }
                        return new { model, sucess = "成功生成日记账" };
                    }
                    catch (Exception ex)
                    {
                        LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                        tran.Rollback();
                        return new { errorMsg = "数据更新错误！" };
                    }
                }

            }
        }
        #endregion
        #region 生单（拥有收款单位为供应商或者个人时才有）
        /// <summary>
        /// 生单（生收付款单）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public object addToCloseBill(ResultListModel model, userInfo userInfo)
        {
            if (userInfo.name == "胡姗_admin")
            {
                userInfo.name = "胡珊_admin";
            }
            if (model.content == null && model.lAmount >= 0)
            {
                return new { errorMsg = "请选择账户" };
            }
            using (var ent = new OAtoU8DATAEntities())
            {
                var isIntoJournal = (from r in ent.RecordTable
                                     where r.Pid.Contains(model.Id)
                                     select r).FirstOrDefault();
                if (isIntoJournal == null)
                {
                    return new { errorMsg = "还未生成日记账，无法进行生单操作" };
                }
                var RecordTable = (from r in ent.RecordTable
                                   where r.Pid.Contains(model.Id) && r.IsIntoCloseBill != null
                                   select r).FirstOrDefault();
                if (RecordTable != null)
                {
                    return new { errorMsg = "已经生单到U8，禁止重复提交" };
                }

            }
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                       // var settleStyle = (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First();
                        for (int i = 0; i < model.piaoju.Count; i++)
                        {
                            var piaoju = model.piaoju[i];
                            if (piaoju.Yinhan.jiefan == 0)
                            {
                                continue;
                            }
                            RecordTable oatab = new RecordTable();
                            using (var ent = new OAtoU8DATAEntities())
                            {//读取日记账数据
                                oatab = (from r in ent.RecordTable
                                         where r.Pid.Contains(model.Id) && r.piaojuId == piaoju.tabClass
                                         select r).FirstOrDefault();
                            }
                            if (oatab == null)
                            {
                                return new { errorMsg = "未查询到单据，无法生单" };
                            }
                            var acctBook = (from b in db.CN_AcctBook
                                            where b.ID == oatab.Bid
                                            select b
                                       ).First();
                            if (model.content.ID != acctBook.AcctID)
                            {
                                var acct = db.CN_AcctInfo.Where(o => o.ID == acctBook.AcctID).FirstOrDefault();
                                return new { errorMsg = "生单账户和日记账账户不一致，日记账账户为" + acct.AcctName };
                            }
                            if ((acctBook.CustomerID == null || acctBook.CustomerID == 0) && (acctBook.VendorID == null || acctBook.VendorID == 0))
                            {
                                return new { errorMsg = "无法搜索到客户或者供应商，无法生单" };
                            }
                            var closeBills = (from c in db.Ap_CloseBills orderby c.iID descending select c).FirstOrDefault();//获取最后一个数据
                            DateTime dates = acctBook != null ? Convert.ToDateTime(acctBook.AcctDate) : model.acctDate;
                            DateTime dateLast = Convert.ToDateTime(dates.AddMonths(1).AddDays(-(dates.Day + 1)).ToString("yyyy-MM-dd"));//获取这个月的最后一天
                            if (dates.Date.Month == DateTime.Now.Month)
                            {
                                dateLast = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                            }
                            DateTime dateFirst = Convert.ToDateTime(dates.AddDays(1 - dates.Day).ToString("yyyy-MM-dd"));//获取这个月的第一天
                            Ap_CloseBills clo = new Ap_CloseBills
                            {
                                iID = closeBills == null ? 10001674 : closeBills.iID + 1,
                                ID = closeBills == null ? 10001739 : closeBills.ID + 1,
                                iType = 0,
                                bPrePay = false
                            };
                            var cCusVen = "";//
                            var VT_ID = 8055;// 8051收供应商 8055 付客户
                            foreach (Detail list in piaoju.Detail)
                            {
                                var detailjine = Convert.ToDecimal(list.jine);
                                if (!string.IsNullOrEmpty(list.miaoshu))
                                {//获取流量集合
                                    var oldKemu = list.kemu;
                                    var trueKemu = list.kemu.Substring(0, list.kemu.Length - 2);
                                    var thisCode = u8.getCodeInU8Byccode(trueKemu);
                                    if (thisCode == null)
                                    {//没找到这个科目
                                        continue;
                                    }
                                    if (thisCode.bcus)
                                    {//客户往来核算 
                                        var kehu = (from v in db.Customer
                                                    join c in db.CN_UnitID on v.cCusCode equals c.LoadCusID
                                                    where v.cCusName.Equals(model.shoukuandanwei)
                                                    select v).FirstOrDefault();//获取客户类型的受益部门 
                                        if (kehu != null)
                                        {
                                            cCusVen = kehu.cCusCode;
                                            VT_ID = 8055;
                                            break;
                                        }
                                    }
                                    if (thisCode.bsup)
                                    {//供应商往来核算项目 
                                        var gonyingshang = (from v in db.Vendor
                                                            join c in db.CN_UnitID on v.cVenCode equals c.LoadVenID
                                                            where v.cVenName.Equals(model.shoukuandanwei)
                                                            select v).FirstOrDefault();
                                        if (gonyingshang != null)
                                        {
                                            cCusVen = gonyingshang.cVenCode;
                                            VT_ID = 8053;
                                            break;
                                        }
                                    }
                                }
                            }
                            clo.cCusVen = cCusVen;//客户或供应商编码
                            clo.iAmt = Convert.ToDecimal(acctBook.Credit);
                            clo.iAmt_f = Convert.ToDecimal(acctBook.Credit);
                            clo.iRAmt = Convert.ToDecimal(acctBook.Credit);
                            clo.iRAmt_f = Convert.ToDecimal(acctBook.Credit);
                            clo.cKm = null;
                            clo.cXmClass = null;
                            var cdep = u8.getDepatements(model.shouyibumen);
                            clo.cDepCode = cdep?.cDepCode;
                            clo.cPersonCode = null;
                            clo.cItemName = null;
                            clo.iAmt_s = 0;
                            clo.iRAmt_s = 0;
                            clo.iOrderType = null;
                            clo.ifaresettled_f = 0.00M;
                            clo.iOrderType = null;
                            clo.ccItemCode = null;
                            clo.RegisterFlag = 0;
                            clo.iSrcClosesID = 0;
                            Ap_CloseBill closeBill = new Ap_CloseBill
                            {
                                cVouchType = model.isPay ? "49" : "48"
                            }; //暂时U8数据库设计的是48收供应商，49付客户
                            var cvouchId = dateLast.ToString("yyMM") + "00000001";
                            var oldLasts = (from c in db.Ap_CloseBill
                                            orderby c.cVouchID descending
                                            select c).FirstOrDefault();
                            if (oldLasts != null)
                            {
                                var date = oldLasts.cVouchID.Substring(0, 4);
                                if (date.Equals(Convert.ToDateTime(acctBook.AcctDate).ToString("yyMM")))
                                {
                                    closeBill.cVouchID = (Convert.ToInt64(oldLasts.cVouchID) + 1).ToString();
                                }
                                else
                                {
                                    closeBill.cVouchID = cvouchId;
                                }
                            }
                            else
                            {
                                closeBill.cVouchID = cvouchId;
                            }

                            closeBill.dVouchDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd"));
                            closeBill.iPeriod = Convert.ToByte(dateLast.Month);
                            closeBill.cDwCode = cCusVen;//"010001"
                            closeBill.cDeptCode = cdep?.cDepCode;//获取部门 //外键Department表的cDepCode
                            closeBill.cPerson = null;
                            //var fitem = (from f in db.fitem where f.citem_name.Equals("现金流量项目") select f).First();//获取现金流量项目的CLass
                            closeBill.cItem_Class = null; //fitem.citem_class;// 获取项目名称 外键fitem表的 citem_class
                            closeBill.cSSCode = acctBook.SettleTypeID.ToString().PadLeft(2,'0');//结算方式编码 01现金 02 转账 //外键SettleStyle表的cSSCode
                            closeBill.cNoteNo = null;
                            closeBill.cCoVouchType = "SC";//对应单据类型 
                            closeBill.cDigest = acctBook.Summary;//摘要
                            closeBill.cexch_name = "人民币";//币种名称
                            closeBill.iExchRate = 1;//汇率 
                            closeBill.iAmount = acctBook.Credit;//本币金额 
                            closeBill.iAmount_f = acctBook.Credit;//原币金额 
                            closeBill.iRAmount = acctBook.Credit;//本币余额 
                            closeBill.iRAmount_f = acctBook.Credit;//原币金额
                            closeBill.cOperator = userInfo.name.Replace("_admin", "");//录入人 
                            closeBill.bStartFlag = false;//期初标志 
                            closeBill.cCode = model.content.SubjectCode;//结算科目编码 
                            closeBill.iPayForOther = false;//代付标志
                            closeBill.cFlag =model.isPay?"AP":"AR";//AR 应收款管理 AP:应付款管理 
                            closeBill.iID = clo.iID;
                            closeBill.bFromBank = false;//银行导入标志 
                            closeBill.bToBank = false;//导出银行标志 
                            closeBill.bSure = false;//银行确认标志 
                            closeBill.VT_ID = VT_ID;//单据模版号 8051收供应商 8055 付客户
                            closeBill.iAmount_s = 0;//数量 
                            closeBill.dverifydate = dateLast;
                            closeBill.IsWfControlled = false;//是否工作流控制 
                            closeBill.iSource = null;
                            //closeBill.dmodifysystime = DateTime.Now;
                            //closeBill.cmodifier = userInfo.name;
                            // closeBill.dmoddate = DateTime.Now;
                            closeBill.iPayType = 0;
                            closeBill.csysbarcode = "||ap" + closeBill.cVouchType + "|" + closeBill.cVouchID;
                            closeBill.ibg_ctrl = false;//是否预算控制 
                            closeBill.ibg_overflag = 0;//预算审批状态 
                            closeBill.dcreatesystime = dateLast;
                            closeBill.RegisterFlag = 0;//登记标志
                                                       /* closeBill.Ufts = TimeHelp.DateToByte(DateTime.Now);//时间转byte*/
                            closeBill.bReceived = false;//是否接收 
                            closeBill.bSend = false;//是否发送标志                         
                            closeBill.bPrePay = false;//预收预付标志 
                            var lastClosebill = db.Ap_CloseBill.OrderByDescending(o => o.iID).FirstOrDefault();
                            CN_PayedRecord pay = new CN_PayedRecord
                            {
                                iMainID = clo.iID,
                                iSubID = clo.ID,
                                iAcctBookID = acctBook.ID,
                                fMoney = Convert.ToDouble(acctBook.Credit),
                                fMoneyF = Convert.ToDouble(acctBook.Credit),
                                lMakeVouch = 11//设置成11后就表示已经生单无法修改
                            };//支付金额记录,生单中间表
                            db.Ap_CloseBill.Add(closeBill);//添加主表
                            db.SaveChanges();
                            db.Ap_CloseBills.Add(clo);//添加子表
                            db.SaveChanges();
                            db.CN_PayedRecord.Add(pay);//添加中间表
                            db.SaveChanges();
                            //添加成功一个更新自定义表数据
                            using (var ent = new OAtoU8DATAEntities())
                            {
                                var request = (from re in ent.RecordTable
                                               where re.Bid == acctBook.ID
                                               select re).FirstOrDefault();
                                request.ip2 = userInfo.name;
                                request.updateTime2 = dateLast;
                                request.IsIntoCloseBill = 1;
                                request.danjubianhao = closeBill.cVouchID;
                                ent.SaveChanges();
                            }
                            model.piaoju[i].danjubianhao = closeBill.cVouchID;
                        }
                        tran.Commit();
                        foreach (var piaoju in model.piaoju)
                        {
                            Log4Helper.Info(typeof(YYDal), "生单成功,单据编号为" + piaoju.danjubianhao);
                        }
                        return new { model, sucess = "成功生单" };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                        return new { errorMsg = "系统错误，生单失败" };
                    }
                }
            }

        }
        #endregion
        /// <summary>
        /// 添加收付款单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public object addToCloseBill_U8API(ResultListModel model, userInfo userInfo)
        {
            if (userInfo.name == "胡姗_admin")
            {
                userInfo.name = "胡珊_admin";
            }
            if (model.content == null && model.lAmount >= 0)
            {
                return new { errorMsg = "请选择账户" };
            }
            using (var ent = new OAtoU8DATAEntities())
            {
                var isIntoJournal = (from r in ent.RecordTable
                                     where r.Pid.Contains(model.Id)
                                     select r).FirstOrDefault();
                if (isIntoJournal == null)
                {
                    return new { errorMsg = "还未生成日记账，无法进行生单操作" };
                }
                var RecordTable = (from r in ent.RecordTable
                                   where r.Pid.Contains(model.Id) && r.IsIntoCloseBill != null
                                   select r).FirstOrDefault();
                if (RecordTable != null)
                {
                    return new { errorMsg = "已经生单到U8，禁止重复提交" };
                }

            }
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        //var settleStyle = (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First();
                        for (int i = 0; i < model.piaoju.Count; i++)
                        {
                            var piaoju = model.piaoju[i];
                            if (piaoju.Yinhan.jiefan == 0)
                            {
                                continue;
                            }
                            RecordTable oatab = new RecordTable();
                            using (var ent = new OAtoU8DATAEntities())
                            {//读取到自己设计表的数据
                                oatab = (from r in ent.RecordTable
                                         where r.Pid.Contains(model.Id) && r.piaojuId == piaoju.tabClass
                                         select r).FirstOrDefault();
                            }
                            if (oatab == null)
                            {
                                return new { errorMsg = "未查询到单据，无法生单" };
                            }
                            var acctBook = (from b in db.CN_AcctBook
                                            where b.ID == oatab.Bid
                                            select b
                                       ).First();
                            if (model.content.ID != acctBook.AcctID)
                            {
                                var acct = db.CN_AcctInfo.Where(o => o.ID == acctBook.AcctID).FirstOrDefault();
                                return new { errorMsg = "生单账户和日记账账户不一致，日记账账户为" + acct.AcctName };
                            }
                            if ((acctBook.CustomerID == null || acctBook.CustomerID == 0) && (acctBook.VendorID == null || acctBook.VendorID == 0))
                            {
                                return new { errorMsg = "无法搜索到客户或者供应商，无法生单" };
                            }
                            var closeBills = (from c in db.Ap_CloseBills orderby c.iID descending select c).FirstOrDefault();//获取最后一个数据
                            DateTime dates = acctBook != null ? Convert.ToDateTime(acctBook.AcctDate) : model.acctDate;
                            DateTime dateLast = dates.AddMonths(1).AddDays(-(dates.Day + 1));//获取这个月的最后一天
                            if (dates.Date.Month == DateTime.Now.Month)
                            {
                                dateLast = DateTime.Now;
                            }
                            DateTime dateFirst = Convert.ToDateTime(dates.AddDays(1 - dates.Day).ToString("yyyy-MM-dd"));//获取这个月的第一天
                            var cCusVen = "";//
                            var VT_ID = 8055;// 8053收供应商 8055 付客户

                            foreach (Detail list in piaoju.Detail)
                            {
                                var detailjine = Convert.ToDecimal(list.jine);
                                if (!string.IsNullOrEmpty(list.miaoshu))
                                {//获取流量集合
                                    var oldKemu = list.kemu;
                                    var trueKemu = list.kemu.Substring(0, list.kemu.Length - 2);
                                    var thisCode = u8.getCodeInU8Byccode(trueKemu);
                                    if (thisCode == null)
                                    {//没找到这个科目
                                        return new { errorMsg = "科目选择有误！" };
                                    }
                                    if (thisCode.bcus)
                                    {//客户往来核算 
                                        var kehu = (from v in db.Customer
                                                    join c in db.CN_UnitID on v.cCusCode equals c.LoadCusID
                                                    where v.cCusName.Equals(model.shoukuandanwei)
                                                    select v).FirstOrDefault();//获取客户类型的受益部门 
                                        if (kehu != null)
                                        {
                                            cCusVen = kehu.cCusCode;
                                            VT_ID = 8055;
                                            break;
                                        }
                                    }
                                    if (thisCode.bsup)
                                    {//供应商往来核算项目 
                                        var gonyingshang = (from v in db.Vendor
                                                            join c in db.CN_UnitID on v.cVenCode equals c.LoadVenID
                                                            where v.cVenName.Equals(model.shoukuandanwei)
                                                            select v).FirstOrDefault();
                                        if (gonyingshang != null)
                                        {
                                            cCusVen = gonyingshang.cVenCode;
                                            VT_ID = 8053;
                                            break;
                                        }
                                    }
                                }
                            }
                            ////*******************调用永鹏他们的接口实现付款单生成***********************************/////////////////////////////
                            Ap_CloseBill_head head = new Ap_CloseBill_head
                            {
                                cVouchType = "49",
                                dVouchDate = dateLast.ToString("yyyy-MM-dd"),
                                iPeriod = dateLast.Month.ToString(),//单据日期的月份取整数
                                cDwCode = cCusVen,
                                cDeptCode = u8.getDepatements(model.shouyibumen) == null ? "" : u8.getDepatements(model.shouyibumen).cDepCode,
                                cPerson = "",
                                cItem_Class = "",
                                cSSCode = acctBook.SettleTypeID.ToString().PadLeft(2,'0'), //settleStyle.cSSCode,//结算方式编码 01现金 02 转账 //外键SettleStyle表的cSSCode
                                cNoteNo = "",
                                cCoVouchType = "SC",//对应单据类型 
                                cCoVouchID = "",
                                cDigest = acctBook.Summary,//摘要
                                cexch_name = "人民币",
                                iExchRate = "1",
                                iAmount = acctBook.Credit.ToString(),
                                iAmount_f = acctBook.Credit.ToString(),
                                iRAmount = acctBook.Credit.ToString(),
                                iRAmount_f = acctBook.Credit.ToString(),
                                cOperator = userInfo.name.Replace("_admin", ""),//录入人 
                                cCancelMan = "",
                                bStartFlag = "0",
                                cCode = model.content.SubjectCode,//结算科目编码 
                                iPayForOther = "0",
                                cFlag =model.isPay?"AP":"AP",//AR 应收款管理 AP:应付款管理 
                                cCancelNo = "",
                                bFromBank = "0",
                                bToBank = "0",
                                bSure = "0",
                                VT_ID = VT_ID.ToString(),//单据模版号 8051收供应商 8055 付客户
                                iAmount_s = "0",
                                iPayType = "0"
                            };
                            List<Ap_CloseBills_body> body_list = new List<Ap_CloseBills_body>();

                            Ap_CloseBills_body body = new Ap_CloseBills_body
                            {
                                iType = "0",
                                bPrePay = "0",
                                cCusVen = cCusVen,
                                iAmt_f = acctBook.Credit.ToString(),
                                iAmt = acctBook.Credit.ToString(),
                                iRAmt_f = acctBook.Credit.ToString(),
                                iRAmt = acctBook.Credit.ToString(),
                                cKm = piaoju.Detail[0].kemu.Substring(0, piaoju.Detail[0].kemu.Length - 2),
                                cXmClass = "",
                                cDepCode = u8.getDepatements(model.shouyibumen)?.cDepCode,
                                iAmt_s = "0",
                                iRAmt_s = "0",
                                iOrderType = "",
                                ccItemCode = ""
                            };

                            string zhangtao = coutaccset;
                            string codeType = model.isPay ? "付款单":"收款单";
                            string sYear = acctBook.lYear == 0 ? DateTime.Now.Year.ToString() : acctBook.lYear.ToString();
                            string Ap_CloseBill_ID = string.Empty;
                            string error_msg = string.Empty;
                            body_list.Add(body);

                            LogHelper.WriteLog("进入"+ codeType);
                            //打开数据库连接
                            DBSqlServersHelper.ConnectionString = oaToU8config;

                            sql_shoukuandan_input.all_shoukuandan_index(head, body_list, zhangtao, codeType, sYear, "0", out Ap_CloseBill_ID, out error_msg);
                            LogHelper.WriteLog(codeType+"iid:" + Ap_CloseBill_ID);
                            if (!string.IsNullOrEmpty(Ap_CloseBill_ID))
                            {
                                var Ap_CloseBill_IDs = Convert.ToInt32(Ap_CloseBill_ID);
                                var clos = db.Ap_CloseBills.Where(o => o.iID == Ap_CloseBill_IDs).FirstOrDefault();
                                var closeBill = db.Ap_CloseBill.Where(o => o.iID == Ap_CloseBill_IDs).FirstOrDefault();
                                CN_PayedRecord pay = new CN_PayedRecord
                                {
                                    iMainID = clos.iID,
                                    iSubID = clos.ID,
                                    iAcctBookID = acctBook.ID,
                                    fMoney = Convert.ToDouble(acctBook.Credit),
                                    fMoneyF = Convert.ToDouble(acctBook.Credit),
                                    lMakeVouch = 11//设置成11后就表示已经生单无法修改
                                };//支付金额记录,生单中间表
                                LogHelper.WriteLog("添加中间表");
                                db.CN_PayedRecord.Add(pay);//添加中间表
                                db.SaveChanges();
                                LogHelper.WriteLog("添加中间结束");
                                //添加成功一个更新自定义表数据
                                using (var ent = new OAtoU8DATAEntities())
                                {
                                    var request = (from re in ent.RecordTable
                                                   where re.Bid == acctBook.ID
                                                   select re).FirstOrDefault();
                                    request.ip2 = userInfo.name;
                                    request.updateTime2 = dateLast;
                                    request.IsIntoCloseBill = 1;
                                    request.danjubianhao = closeBill.cVouchID;
                                    ent.SaveChanges();
                                }
                                LogHelper.WriteLog("添加中间库");
                                model.piaoju[i].danjubianhao = closeBill.cVouchID;
                            }
                            else
                            {
                                return new { errorMsg = error_msg };
                            }
                        }
                        tran.Commit();
                        foreach (var piaoju in model.piaoju)
                        {
                            Log4Helper.Info(typeof(YYDal), "生单成功,单据编号为" + piaoju.danjubianhao);
                        }
                        return new { model, sucess = "成功生单" };
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                        return new { errorMsg = "系统错误，生单失败" };
                    }
                }
            }
        }
        #region 制单
        /// <summary>
        /// 制单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public object addToCashSignRelate(ResultListModel model, userInfo userInfo)
        {
            if (userInfo.name == "胡姗_admin")
            {
                userInfo.name = "胡珊_admin";
            }
            if (model.content == null)
            {
                return new { errorMsg = "请选择账户" };
            }
            var NullDetail = model.piaoju[0].Detail.Where(o => string.IsNullOrEmpty(o.miaoshu) == true).FirstOrDefault();
            if (NullDetail == null)
            {
                return new { errorMsg = "请填写所有摘要信息" };
            }
            Boolean shifouShendan = false;
            using (var ent = new OAtoU8DATAEntities())
            {
                var RecordTables = (from r in ent.RecordTable
                                    where r.Pid.Contains(model.Id) && r.IsIntoAccvouch != null
                                    select r).FirstOrDefault();
                if (RecordTables != null)
                {
                    //存在数据，返回值
                    return new { errorMsg = "已经制单到U8，禁止重复提交" };
                }

            }
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    using (var ent = new OAtoU8DATAEntities())
                    {
                        using (var trans = ent.Database.BeginTransaction())
                        {
                            try
                            {
                                FromOADal oa = new FromOADal();
                                //var guanlingYuzhi = oa.getSunByattachment(model,);//获取关联的预支单
                                for (int i = 0; i < model.piaoju.Count; i++)
                                {//读取票据
                                    var piaoju = model.piaoju[i];
                                    var RecordTable = (from r in ent.RecordTable
                                                       where r.Pid.Contains(model.Id) && r.piaojuId == piaoju.tabClass
                                                       select r).FirstOrDefault();
                                    shifouShendan = string.IsNullOrEmpty(RecordTable.danjubianhao) ? false : true;
                                    CN_AcctBook acctBook = new CN_AcctBook();
                                    if (RecordTable == null && piaoju.Yinhan.jiefan != 0)
                                    {
                                        return new { errorMsg = "单据错误，未找到" + piaoju.tabClass + "请先日记账" };
                                    }
                                    if (RecordTable != null)
                                    {
                                        acctBook = (from b in db.CN_AcctBook where b.ID == RecordTable.Bid select b).FirstOrDefault();
                                    }
                                    DateTime date = acctBook != null ? Convert.ToDateTime(acctBook.AcctDate) : model.acctDate;
                                    DateTime dateLast = Convert.ToDateTime(date.AddMonths(1).AddDays(-(date.Day + 1)).ToString("yyyy-MM-dd"));//获取这个月的最后一天
                                    if (date.Date.Month == DateTime.Now.Month)
                                    {
                                        dateLast = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                                    }
                                    DateTime dateFirst = Convert.ToDateTime(date.AddDays(1 - date.Day).ToString("yyyy-MM-dd"));//获取这个月的第一天
                                    var LastGl = (from g in db.GL_accvouch where g.dbill_date >= dateFirst orderby g.ino_id descending select g).FirstOrDefault();
                                    short ino_id = LastGl == null ? Convert.ToInt16(1) : Convert.ToInt16(LastGl.ino_id + 1);
                                    var yinhankemu = "";
                                    foreach (Detail list in piaoju.Detail)
                                    {
                                        if (Convert.ToDecimal(list.jiefan) == 0)
                                        {
                                            var test = yinhankemu + list.kemu.Substring(0, list.kemu.Length - 2) + ",";
                                            if (test.Length < 50)
                                            {
                                                yinhankemu = yinhankemu + list.kemu.Substring(0, list.kemu.Length - 2) + ",";
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    yinhankemu = yinhankemu.Substring(0, yinhankemu.Length - 2);
                                    if (piaoju.Yinhan.jiefan == 0)
                                    {//说明无流量操作，直接进总表（冲借支）
                                        short inid = 0;
                                        var snuord = u8.getCashSerialNumber("付");
                                        var dsign = (from d in db.CN_CashSerialNumber where d.SNWord.Equals(snuord) select d).FirstOrDefault();//这里保存了记账凭证号
                                        var time = TimeHelp.ConvertDateTimeToInt(dateLast);
                                        while (true)
                                        {
                                            var x = db.GL_accvouch.Where(g => g.coutno_id.Contains(time.ToString())).FirstOrDefault();
                                            if (x == null)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                time++;
                                            }
                                        }
                                        string ccodeList = "";

                                        #region 科目明细录入

                                        var LastGls = (from g in db.GL_accvouch where g.coutno_id.Contains("GL") orderby g.coutno_id descending select g).FirstOrDefault();
                                        var Gl = "GL0000000000001";
                                        if (LastGls != null)
                                        {
                                            string result = System.Text.RegularExpressions.Regex.Replace(LastGls.coutno_id, @"[^0-9]+", "");
                                            Gl = "GL" + (Convert.ToInt64(result) + 1).ToString();
                                        }
                                        var UnitID = 0; //客户/供应商id
                                                        // var UnitType = 2;//客户为1，供应商为2
                                        Customer kehus = new Customer();
                                        Vendor gonyingshangs = new Vendor();
                                        Person user = new Person();
                                        foreach (Detail list in piaoju.Detail)
                                        {
                                            var detailjine = Convert.ToDecimal(list.jine);
                                            if (!string.IsNullOrEmpty(list.miaoshu))
                                            {//获取流量集合

                                                if (string.IsNullOrEmpty(list.kemu))
                                                {
                                                    return "请确认每一条明细都选择了对应科目";
                                                }
                                                var oldKemu = list.kemu;
                                                var trueKemu = list.kemu.Substring(0, list.kemu.Length - 2);

                                                var thisCode = u8.getCodeInU8Byccode(trueKemu);
                                                string bitem = null;
                                                if (thisCode == null)
                                                {//没找到这个科目
                                                    continue;
                                                }
                                                if (thisCode.bcus)
                                                {//客户往来核算 
                                                    var kehu = u8.geCustomertUnit(model.shoukuandanwei);//客户
                                                    if (kehu != null)
                                                    {
                                                        UnitID = kehu.ID;
                                                        acctBook.UnitType = 1;//客户为1，供应商为2,根据u8上默认是选择客户的
                                                        acctBook.UnitID = UnitID;
                                                        acctBook.CustomerID = UnitID;
                                                        kehus = (from cu in db.Customer
                                                                 join c in db.CN_UnitID on cu.cCusCode equals c.LoadCusID
                                                                 where c.ID == UnitID
                                                                 select cu).FirstOrDefault();//获取客户类型的受益部门
                                                    }
                                                    else
                                                    {
                                                        return new { errorMsg = thisCode.ccode + "是客户受控科目，但是在客户列表中无法查询到" + model.shoukuandanwei + "这个客户" };
                                                    }
                                                }
                                                if (thisCode.bsup)
                                                {//供应商往来核算项目 

                                                    var gonyingshang = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                                                    if (gonyingshang != null)
                                                    {
                                                        UnitID = gonyingshang.ID;
                                                        acctBook.UnitType = 2;//客户为1，供应商为2
                                                        acctBook.UnitID = UnitID;
                                                        acctBook.VendorID = UnitID;
                                                        gonyingshangs = (from ve in db.Vendor
                                                                         join c in db.CN_UnitID on ve.cVenCode equals c.LoadVenID
                                                                         where c.ID == UnitID
                                                                         select ve).FirstOrDefault();
                                                    }
                                                    else
                                                    {
                                                        return new { errorMsg = thisCode.ccode + "是供应商受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "这个供应商" };
                                                    }
                                                }
                                                if (thisCode.bperson)
                                                {//个人往来核算项目 
                                                    user = u8.getPerson(model.shoukuandanwei);
                                                    if (user == null)
                                                    {
                                                        return new { errorMsg = thisCode.ccode + "是个人往来受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "" };
                                                    }
                                                }
                                                if (thisCode.bitem)
                                                {// bitem(U861)  是否项目核算
                                                    bitem = "1001";
                                                }

                                                if (thisCode.bdept && string.IsNullOrEmpty(list.shouyibumen))
                                                {//部门往来核算项目 
                                                    return new { errorMsg = thisCode.ccode + "部门受控,请选择部门" };
                                                }
                                                if (!thisCode.bdept)
                                                {//不是受控部门的去掉受控部门，不然在U8上回报错非受控部门
                                                    list.shouyibumen = null;
                                                }
                                                if (thisCode.ccode == "220201" && RecordTable.danjubianhao == null)
                                                {
                                                    return new { errorMsg = "本单需要生成应付单等操作，暂不支持，请自行到U8生成" };
                                                }
                                                if (Convert.ToDecimal(list.jiefan) == 0)
                                                {
                                                    gonyingshangs = null;
                                                }
                                                if (dsign == null)
                                                {
                                                    return new { errorMsg = "无记账凭证" };
                                                }
                                                inid++;

                                                ccodeList = trueKemu + ",";
                                                if (list.shouyibumen != null)
                                                {
                                                    var depatement = (from d in db.Department where d.cDepCode.Equals(list.shouyibumen) select d).FirstOrDefault();
                                                    if (depatement == null)
                                                    {
                                                        list.shouyibumen = null;
                                                    }
                                                }
                                                if (list.miaoshu.Length >= 120)
                                                {
                                                    list.miaoshu = list.miaoshu.Substring(0, 120);
                                                }
                                                GL_accvouch acc = new GL_accvouch()
                                                {
                                                    iperiod = Convert.ToByte(dateLast.Month),
                                                    csign = dsign.U8VouchSign,//凭证类别字
                                                    isignseq = 1,//凭证类别排序号 
                                                    ino_id = ino_id,//凭证编号 
                                                    inid = inid,//在12循环行号 
                                                    dbill_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//制单日期
                                                    idoc = !shifouShendan ? (short)0 : (short)1,//附单据数 (不生单为0生单为1)
                                                    cbill = userInfo.name.Replace("_admin", ""),//制单人
                                                    ccheck = null,
                                                    cbook = null,
                                                    ibook = 0,
                                                    ccashier = !shifouShendan ? userInfo.name.Replace("_admin", "") : null,//出纳签字人(不生单的签字了，生单的没有)
                                                    iflag = null,
                                                    ctext1 = null,
                                                    ctext2 = null,
                                                    cdigest = list.miaoshu,//摘要 
                                                    ccode = trueKemu,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                    cexch_name = null,
                                                    md = Convert.ToDecimal(list.jine),//借方金额 
                                                    mc = Convert.ToDecimal(list.jiefan), //  mc = i == 1 ? Convert.ToDecimal(acctBook.Credit) : 0.00M,//和上面一个有值它就为0.00M 贷方金额 
                                                    mc_f = 0.00M,//外币贷方金额  
                                                    md_f = 0.00M,//外币借方金额 
                                                    nfrat = 0,//汇率 
                                                    nc_s = 0,//数量贷方 
                                                    nd_s = 0,//数量借方 
                                                    csettle = null,// (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode,//结算方式编码 
                                                    cn_id = null,
                                                    dt_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                    cdept_id = list.shouyibumen,//==null?null: u8.getDepatements(list.shouyibumen).cDepCode,//user == null ? list.shouyibumen : user.cDepCode, //getDepatements(r.shouyibumen).cDepCode,
                                                    cperson_id = user?.cPersonCode,
                                                    ccus_id = kehus?.cCusCode,
                                                    csup_id = gonyingshangs?.cVenCode,
                                                    citem_id = bitem,
                                                    citem_class = null,
                                                    cname = !shifouShendan ? null : "-",
                                                    ccode_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                    iflagbank = null,
                                                    iflagPerson = null,
                                                    coutaccset = null,//上海悦目046，广东悦肌048
                                                    ioutyear = (short)dateLast.Year,//外部凭证会计年度 
                                                    coutsysname = null,//外部凭证系统名称
                                                    coutsysver = null,
                                                    doutbilldate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//外部凭证制单日期
                                                    ioutperiod = Convert.ToByte(dateLast.Month),//外部凭证会计期间 
                                                    coutsign = null,//外部凭证账套号 RP AP
                                                    coutno_id = Gl,//外部凭证业务号 
                                                    doutdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                                                    coutbillsign = !shifouShendan ? null : "49",
                                                    coutid = null,
                                                    bvouchedit = true,//凭证是否可修改 
                                                    bvouchAddordele = true,//凭证分录是否可增删
                                                    bvouchmoneyhold = false,//凭证合计金额是否保值
                                                    bvalueedit = Convert.ToDecimal(list.jiefan) == 0 ? false : true,//分录数值是否可修改
                                                    bcodeedit = true,//分录科目是否可修改 
                                                    ccodecontrol = null,
                                                    bPCSedit = false,//分录往来项是否可修改 
                                                    bDeptedit = true,//分录部门是否可修改 
                                                    bItemedit = true,//分录项目是否可修改 
                                                    bCusSupInput = false,//分录往来项是否必输
                                                    iyear = Convert.ToInt16(dateLast.Year),//凭证的会计年度
                                                    cblueoutno_id = null,
                                                    ccodeexch_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode,//对方科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号) 
                                                    tvouchtime = dateLast,//凭证保存时间 
                                                    iYPeriod = Convert.ToInt32(dateLast.Year + "" + (dateLast.Month >= 10 ? dateLast.Month.ToString() : "0" + dateLast.Month)),
                                                    RowGuid = Guid.NewGuid().ToString(),//包括年度的会计期间 //行标识.规律不知道
                                                    bFlagOut = false,//公司对帐是否导出过对帐单
                                                    bdelete = false,//是否核销 
                                                };
                                                db.GL_accvouch.Add(acc);
                                                db.SaveChanges();
                                            }
                                            else
                                            {
                                                return new { errorMsg = "请填写所有摘要信息" };
                                            }
                                        }
                                        #endregion
                                        RecordTable record = new RecordTable
                                        {
                                            IsIntoBook = 0,
                                            piaojuId = i,
                                            Pid = model.Id,
                                            Bid = 0,
                                            contents = model.content.ID.ToString()
                                        };
                                        record.piaojuId = piaoju.tabClass;

                                        record.ip2 = userInfo.name;
                                        record.updateTime2 = dateLast;
                                        record.IsIntoAccvouch = 1;
                                        record.pingzhenhao = dsign.U8VouchSign + "  " + ino_id;
                                        record.userName = model.faqiren;
                                        ent.RecordTable.Add(record);
                                        ent.SaveChanges();
                                        model.piaoju[i].pingzhenhao = record.pingzhenhao;
                                    }
                                    else
                                    {//必须要生成日记账（有银行流水）
                                        if (model.content.ID != acctBook.AcctID)
                                        {
                                            var acct = db.CN_AcctInfo.Where(o => o.ID == acctBook.AcctID).FirstOrDefault();
                                            if (acct == null)
                                            {
                                                return new { errorMsg = "找不到该账户" };
                                            }
                                            return new { errorMsg = "生成凭证账户和日记账账户不一致，日记账账户为" + acct.AcctName };
                                        }
                                        short inid = 0;
                                        var dsign = (from d in db.CN_CashSerialNumber where d.SNWord.Contains(acctBook.CSNCashSign) select d).FirstOrDefault();//这里保存了记账凭证号
                                        var time = TimeHelp.ConvertDateTimeToInt(DateTime.Now);
                                        while (true)
                                        {
                                            var x = db.GL_accvouch.Where(g => g.coutno_id.Contains(time.ToString())).FirstOrDefault();
                                            if (x == null)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                time++;
                                            }
                                        }
                                        var coutno_id = (from g in db.GL_accvouch
                                                         where g.coutsysname.Equals("AP") && !g.coutno_id.Contains("APHCAP")
                                                         orderby g.coutno_id descending
                                                         select g.coutno_id).FirstOrDefault();
                                        if (coutno_id == null)
                                        {
                                            coutno_id = "01AP00000000001";
                                        }
                                        else
                                        {
                                            if (coutno_id.IndexOf("01AP") != -1)
                                            {//存在
                                                coutno_id = System.Text.RegularExpressions.Regex.Replace(coutno_id, @"[^0-9]+", "");//只保留数字
                                                var last = Convert.ToInt64(coutno_id.Replace("01AP", ""));
                                                last++;
                                                coutno_id = "01AP" + last.ToString().PadLeft(11, '0');
                                            }
                                        }
                                        var clobil = (from cb in db.Ap_CloseBill
                                                      join c in db.Ap_CloseBills on cb.iID equals c.iID
                                                      join cp in db.CN_PayedRecord on c.iID equals cp.iMainID
                                                      where cp.iAcctBookID == acctBook.ID
                                                      select cb).FirstOrDefault();
                                        string ccodeList = "";
                                        #region 科目明细录入
                                        var i_id = 0;
                                        var UnitID = 0; //客户/供应商id
                                        //var UnitType = 2;//客户为1，供应商为2
                                        Customer kehus = new Customer();
                                        Vendor gonyingshangs = new Vendor();
                                        Person user = new Person();
                                        foreach (Detail list in piaoju.Detail)
                                        {
                                            string bitem = null;
                                            var detailjine = Convert.ToDecimal(list.jine);
                                            /* if (!string.IsNullOrEmpty(list.miaoshu))
                                             {//获取流量集合*/
                                            if (list.miaoshu.Length >= 120)
                                            {
                                                list.miaoshu = list.miaoshu.Substring(0, 120);
                                            }
                                            if (string.IsNullOrEmpty(list.kemu))
                                            {
                                                return "请确认每一条明细都选择了对应科目";
                                            }
                                            var oldKemu = list.kemu;
                                            var trueKemu = list.kemu.Substring(0, list.kemu.Length - 2);
                                            var thisCode = u8.getCodeInU8Byccode(trueKemu);
                                            if (thisCode == null)
                                            {//没找到这个科目
                                                continue;
                                            }
                                            if (thisCode.bcus)
                                            {//客户往来核算 
                                                var kehu = u8.geCustomertUnit(model.shoukuandanwei);//客户
                                                if (kehu != null)
                                                {
                                                    UnitID = kehu.ID;
                                                    kehus = (from cu in db.Customer
                                                             join c in db.CN_UnitID on cu.cCusCode equals c.LoadCusID
                                                             where c.ID == UnitID
                                                             select cu).FirstOrDefault();//获取客户类型的受益部门
                                                    acctBook.UnitType = 1;//客户为1，供应商为2,根据u8上默认是选择客户的
                                                    acctBook.UnitID = UnitID;
                                                    acctBook.CustomerID = UnitID;
                                                }
                                                else
                                                {
                                                    return new { errorMsg = thisCode.ccode + "是客户受控科目，但是在客户列表中无法查询到" + model.shoukuandanwei + "这个客户" };
                                                }
                                            }
                                            if (thisCode.bitem)
                                            {// bitem(U861)  是否项目核算
                                                bitem = "1001";
                                            }
                                            if (thisCode.bsup)
                                            {//供应商往来核算项目 
                                                var gonyingshang = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                                                if (gonyingshang != null)
                                                {
                                                    UnitID = gonyingshang.ID;
                                                    gonyingshangs = (from ve in db.Vendor
                                                                     join c in db.CN_UnitID on ve.cVenCode equals c.LoadVenID
                                                                     where c.ID == UnitID
                                                                     select ve).FirstOrDefault();
                                                    acctBook.UnitType = 2;//客户为1，供应商为2
                                                    acctBook.UnitID = UnitID;
                                                    acctBook.VendorID = UnitID;
                                                }
                                                else
                                                {
                                                    return new { errorMsg = thisCode.ccode + "是供应商受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "这个供应商" };
                                                }
                                            }
                                            if (thisCode.bperson)
                                            {//个人往来核算项目 
                                                user = u8.getPerson(model.shoukuandanwei);
                                                if (user == null)
                                                {
                                                    return new { errorMsg = thisCode.ccode + "是个人往来受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "" };
                                                }
                                            }
                                            if (thisCode.bdept && string.IsNullOrEmpty(list.shouyibumen))
                                            {//部门往来核算项目 
                                                return new { errorMsg = thisCode.ccode + "部门受控,请选择部门" };
                                            }

                                            if (!thisCode.bdept)
                                            {//不是受控部门的去掉受控部门，不然在U8上回报错非受控部门
                                                list.shouyibumen = null;
                                            }
                                            if (thisCode.ccode == "220201" && RecordTable.danjubianhao == null)
                                            {
                                                return new { errorMsg = thisCode.ccode + "科目为应付系统科目，请先生成收付款单" };
                                            }
                                            if (dsign == null)
                                            {
                                                return new { errorMsg = "无记账凭证" };
                                            }
                                            inid++;
                                            ccodeList = trueKemu + ",";
                                            if (list.shouyibumen != null)
                                            {
                                                var depatement = (from d in db.Department where d.cDepCode.Equals(list.shouyibumen) select d).FirstOrDefault();
                                                if (depatement == null)
                                                {
                                                    list.shouyibumen = null;
                                                }
                                            }
                                            var ccodecontrol = "***";
                                            if (RecordTable.danjubianhao == null)
                                            {
                                                ccodecontrol = "***";//SC模块的
                                            }
                                            else
                                            {
                                                ccodecontrol = Convert.ToDecimal(list.jiefan) == 0 ? "AP" : "! AR AP #";
                                            }
                                            if (shifouShendan)
                                            {
                                                yinhankemu = model.content.SubjectCode + "|人民币";
                                            }
                                            GL_accvouch acc = new GL_accvouch
                                            {
                                                iperiod = Convert.ToByte(dateLast.Month),
                                                csign = dsign.U8VouchSign,//凭证类别字
                                                isignseq = 1,//凭证类别排序号 
                                                ino_id = ino_id,//凭证编号 
                                                inid = inid,//在12循环行号 
                                                dbill_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//制单日期
                                                idoc = !shifouShendan ? (short)0 : (short)1,//附单据数 (不生单为0生单为1)
                                                cbill = userInfo.name.Replace("_admin", ""),//制单人
                                                ccheck = null,
                                                cbook = null,
                                                ibook = 0,
                                                ccashier = !shifouShendan ? userInfo.name.Replace("_admin", "") : null,//出纳签字人(不生单的签字了，生单的没有)
                                                iflag = null,
                                                ctext1 = null,
                                                ctext2 = null,
                                                cdigest = list.miaoshu,//摘要 
                                                ccode = trueKemu,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                cexch_name = null,
                                                md = Convert.ToDecimal(list.jine),//借方金额 
                                                mc = Convert.ToDecimal(list.jiefan), //  mc = i == 1 ? Convert.ToDecimal(acctBook.Credit) : 0.00M;//和上面一个有值它就为0.00M 贷方金额 
                                                mc_f = 0.00M,//外币贷方金额  
                                                md_f = 0.00M,//外币借方金额 
                                                nfrat = 0,//汇率 
                                                nc_s = 0,//数量贷方 
                                                nd_s = 0,//数量借方 
                                                csettle = Convert.ToDecimal(list.jiefan) == 0 ? null : (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode,// (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode;//结算方式编码 
                                                cn_id = null,
                                                dt_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cdept_id = list.shouyibumen,
                                                //user == null ? list.shouyibumen : user.cDepCode; //getDepatements(r.shouyibumen).cDepCode;
                                                cperson_id = user?.cPersonCode,
                                                ccus_id = kehus?.cCusCode,
                                                csup_id = gonyingshangs?.cVenCode,
                                                citem_id = bitem,
                                                citem_class = null,
                                                cname = !shifouShendan ? null : "-",
                                                ccode_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                iflagbank = null,
                                                iflagPerson = null,
                                                coutaccset = !shifouShendan ? null : coutaccset,//上海悦目046，广东悦肌048
                                                ioutyear = !shifouShendan ? (short)0 : (short)DateTime.Now.Year,//外部凭证会计年度 
                                                coutsysname = !shifouShendan ? "SC" : "AP",//外部凭证系统名称
                                                coutsysver = null,
                                                doutbilldate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//外部凭证制单日期
                                                ioutperiod = Convert.ToByte(dateLast.Month),//外部凭证会计期间 
                                                coutsign = !shifouShendan ? "出纳管理" : "AP",//外部凭证账套号 RP AP
                                                coutno_id = !shifouShendan ? "SC" + time : coutno_id,//外部凭证业务号 
                                                                                                     //doutdate = !shifouShendan ?null : Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd"));
                                                coutbillsign = !shifouShendan ? null : "49",
                                                coutid = clobil?.cVouchID,
                                                bvouchedit = true,//凭证是否可修改 
                                                bvouchAddordele = true,//凭证分录是否可增删
                                                bvouchmoneyhold = false,//凭证合计金额是否保值
                                                bvalueedit = true,//分录数值是否可修改
                                                bcodeedit = true,//分录科目是否可修改 
                                                ccodecontrol = ccodecontrol,
                                                bPCSedit = Convert.ToDecimal(list.jiefan) == 0 ? false : true,//分录往来项是否可修改 
                                                bDeptedit = true,//分录部门是否可修改 
                                                bItemedit = true,//分录项目是否可修改 
                                                bCusSupInput = Convert.ToDecimal(list.jiefan) == 0 ? false : true,//分录往来项是否必输
                                                iyear = Convert.ToInt16(dateLast.Year),//凭证的会计年度
                                                cblueoutno_id = null,
                                                ccodeexch_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode, //Convert.ToDecimal(list.jiefan) == 0 ? model.content.SubjectCode+ "|人民币" : model.content.SubjectCode;//对方科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号) 
                                                tvouchtime = DateTime.Now,//凭证保存时间
                                                iYPeriod = Convert.ToInt32(dateLast.Year + "" + (dateLast.Month >= 10 ? dateLast.Month.ToString() : "0" + dateLast.Month)),
                                                RowGuid = Guid.NewGuid().ToString(),//包括年度的会计期间 //行标识.规律不知道
                                                bFlagOut = false,//公司对帐是否导出过对帐单
                                                bdelete = false//是否核销 
                                            };
                                            if (!shifouShendan)
                                            {
                                                acc.doutdate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd"));
                                            }
                                            db.GL_accvouch.Add(acc);
                                            db.SaveChanges();
                                            i_id = acc.i_id;
                                            /*  }
                                              else
                                              {
                                                  return new { errorMsg = "请填写所有摘要信息" };
                                              }*/
                                        }
                                        #endregion
                                        #region 添加个人记录表
                                        var request = (from re in ent.RecordTable
                                                       where re.Bid == acctBook.ID
                                                       select re).FirstOrDefault();
                                        request.ip2 = userInfo.name;
                                        request.updateTime2 = dateLast;
                                        request.IsIntoAccvouch = 1;
                                        request.pingzhenhao = dsign.U8VouchSign + "  " + ino_id;
                                        request.userName = model.faqiren;
                                        ent.SaveChanges();
                                        #endregion
                                        #region 银行录入
                                        /* inid++;
                                         ccodeList = ccodeList.Substring(0, ccodeList.Length - 1);

                                         GL_accvouch accs = new GL_accvouch()
                                         {//付款方录入
                                             iperiod = Convert.ToByte(DateTime.Now.Month),
                                             csign = dsign.U8VouchSign,//凭证类别字
                                             isignseq = 1,//凭证类别排序号 
                                             ino_id = ino_id,//凭证编号 
                                             inid = Convert.ToInt16(inid),//在12循环行号 
                                             dbill_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),//制单日期
                                             idoc = shifouShendan == false ? (short)0 : (short)1,//附单据数 (不生单为0生单为1)
                                             cbill = userInfo.name,//制单人
                                             ccheck = null,
                                             cacctBook = null,
                                             iacctBook = 0,
                                             ccashier = shifouShendan == false ? acctBook.Cashier : null,//出纳签字人(不生单的签字了，生单的没有)
                                             iflag = null,
                                             ctext1 = null,
                                             ctext2 = null,
                                             cdigest = acctBook.Summary,//摘要 
                                             ccode = model.content.SubjectCode,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                             cexch_name = null,
                                             nc_s = 0,//数量贷方 
                                             nd_s = 0,//数量借方 
                                             nfrat = 0,//汇率 
                                             mc_f = 0.00M,//外币贷方金额  
                                             md_f = 0.00M,//外币借方金额
                                             md = 0.00M, //借方金额 
                                             mc = Convert.ToDecimal(piaoju.Yinhan.jiefan), //Convert.ToDecimal(piaoju.yinhuan),//贷方金额 
                                             csettle = null,// (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode,//结算方式编码 
                                             cn_id = null,
                                             dt_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                                             cdept_id = user == null ? u8.getDepatements(model.faqibumen).cDepCode : user.cDepCode, //getDepatements(r.shouyibumen).cDepCode,
                                             cperson_id = user == null ? null : user.cPersonCode,
                                             ccus_id = kehus == null ? null : kehus.cCusCode,
                                             csup_id = gonyingshangs == null ? null : gonyingshangs.cVenCode,
                                             citem_id = null,
                                             citem_class = null,
                                             cname = shifouShendan == false ? null : "-",
                                             ccode_equal = ccodeList,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                             iflagbank = null,
                                             iflagPerson = null,
                                             coutaccset = "046",//上海悦目046，广东悦肌048
                                             ioutyear = (short)DateTime.Now.Year,//外部凭证会计年度 
                                             coutsysname = acctBook.UnitID == 0 ? "SC" : "AP",//外部凭证系统名称
                                             coutsysver = null,
                                             doutbilldate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),//外部凭证制单日期
                                             ioutperiod = Convert.ToByte(DateTime.Now.Month),//外部凭证会计期间 
                                             coutsign = shifouShendan == false ? "出纳管理" : "RP",//外部凭证账套号 RP AP
                                             coutno_id = shifouShendan == false ? "SC" + time : coutno_id,//外部凭证业务号 
                                             doutdate = DateTime.Now,
                                             coutbillsign = shifouShendan == false ? null : "49",
                                             coutid = clobil == null ? null : clobil.cVouchID + "                  " + inid,
                                             bvouchedit = true,//凭证是否可修改 
                                             bvouchAddordele = true,//凭证分录是否可增删
                                             bvouchmoneyhold = false,//凭证合计金额是否保值
                                             bvalueedit = false,//分录数值是否可修改
                                             bcodeedit = true,//分录科目是否可修改 
                                             ccodecontrol = "AP",
                                             bPCSedit = false,//分录往来项是否可修改 
                                             bDeptedit = true,//分录部门是否可修改 
                                             bItemedit = true,//分录项目是否可修改 
                                             bCusSupInput = false,//分录往来项是否必输
                                             iyear = Convert.ToInt16(DateTime.Now.Year),//凭证的会计年度
                                             cblueoutno_id = null,
                                             ccodeexch_equal = model.content.SubjectCode,//对方科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                                                         // daudit_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),//凭证审核日期 
                                             tvouchtime = DateTime.Now,//凭证保存时间 
                                             iYPeriod = Convert.ToInt32(DateTime.Now.Year + "" + (DateTime.Now.Month >= 10 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month)),
                                             RowGuid = Guid.NewGuid().ToString(),//包括年度的会计期间 
                                             bFlagOut = false,//公司对帐是否导出过对帐单
                                             bdelete = false,//是否核销 
                                         };
                                         db.GL_accvouch.Add(accs);
                                         db.SaveChanges();*/
                                        #endregion
                                        #region 如果生单，则需要修改生单表数据
                                        if (RecordTable.danjubianhao != null)
                                        {//生单过
                                            var kehu = u8.geCustomertUnit(model.shoukuandanwei);//客户
                                            if (kehu != null)
                                            {
                                                UnitID = kehu.ID;
                                                kehus = (from cu in db.Customer
                                                         join c in db.CN_UnitID on cu.cCusCode equals c.LoadCusID
                                                         where c.ID == UnitID
                                                         select cu).FirstOrDefault();//获取客户类型的受益部门
                                            }
                                            var cCusVen = kehus == null ? "" : kehus.cCusCode;
                                            var gonyingshang = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                                            if (gonyingshang != null)
                                            {
                                                UnitID = gonyingshang.ID;
                                                gonyingshangs = (from ve in db.Vendor
                                                                 join c in db.CN_UnitID on ve.cVenCode equals c.LoadVenID
                                                                 where c.ID == UnitID
                                                                 select ve).FirstOrDefault();
                                            }
                                            cCusVen = gonyingshangs == null ? "" : gonyingshangs.cVenCode;
                                            var settleStyle = (from s in db.SettleStyle where s.cSSName.Equals("转账") select s);
                                            Ap_Detail detail = new Ap_Detail()
                                            {
                                                iPeriod = Convert.ToByte(dateLast.Month),
                                                cVouchType = "49",
                                                cVouchSType = null,
                                                cVouchID = dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                dVouchDate = dateLast,
                                                dRegDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cDwCode = cCusVen,
                                                iBVid = 0,
                                                cCode = "100202",
                                                isignseq = Convert.ToByte(dateLast.Month),
                                                ino_id = null,
                                                cDigest = acctBook.Summary,
                                                iPrice = 0,
                                                cexch_name = "人民币",
                                                iExchRate = 1,
                                                iDAmount = 0.00M,
                                                iCAmount = acctBook.Credit,
                                                iDAmount_f = 0.00M,
                                                iCAmount_s = (double)acctBook.Credit,
                                                iDAmount_s = 0,
                                                iCAmount_f = 0,
                                                cSSCode = settleStyle.First().cSSCode,
                                                cProcStyle = "49",
                                                cCancelNo = "AP49" + dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                bPrePay = false,
                                                iFlag = 6,
                                                cCoVouchType = "49",
                                                cCoVouchID = dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                cFlag = "AP",
                                                iClosesID = 0,
                                                iCoClosesID = 0,
                                                cGLSign = dsign.U8VouchSign,
                                                iGLno_id = (short)acctBook.CSNCashID,
                                                dPZDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cOperator = acctBook.Cashier,
                                                cCheckMan = acctBook.Cashier,
                                                cPZid = coutno_id

                                            };
                                            Ap_Detail details = new Ap_Detail()
                                            {
                                                iPeriod = Convert.ToByte(dateLast.Month),
                                                cVouchType = "49",
                                                cVouchSType = null,
                                                cVouchID = dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                dVouchDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                dRegDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cDwCode = cCusVen,
                                                iBVid = 0,
                                                ino_id = ino_id,
                                                cCode = "220201",
                                                isignseq = Convert.ToByte(dateLast.Month),
                                                cDigest = acctBook.Summary,
                                                iPrice = 0,
                                                cexch_name = "人民币",
                                                iExchRate = 1,
                                                iDAmount = acctBook.Credit,
                                                iCAmount = 0.00M,
                                                iDAmount_f = acctBook.Credit,
                                                iCAmount_s = 0.00,
                                                iDAmount_s = 0,
                                                iCAmount_f = 0,
                                                cSSCode = settleStyle.First().cSSCode,
                                                cProcStyle = "49",
                                                cCancelNo = "AP49" + DateTime.Now.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                bPrePay = true,
                                                iFlag = 6,
                                                cCoVouchType = "49",
                                                cCoVouchID = DateTime.Now.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                cFlag = "AP",
                                                iClosesID = 0,
                                                iCoClosesID = 0,
                                                cGLSign = dsign.U8VouchSign,
                                                iGLno_id = (short)acctBook.CSNCashID,
                                                dPZDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cOperator = acctBook.Cashier,
                                                cCheckMan = acctBook.Cashier,
                                                cPZid = coutno_id,
                                                iAmount = acctBook.Credit,
                                                iAmount_f = acctBook.Credit,
                                                iAmount_s = 0,
                                                iVouchAmount = acctBook.Credit,
                                                iVouchAmount_f = acctBook.Credit,
                                                iVouchAmount_s = 0
                                            };
                                            db.Ap_Detail.Add(detail);
                                            db.Ap_Detail.Add(details);
                                            db.SaveChanges();
                                            //修改生单
                                            var col = (from c in db.Ap_CloseBills
                                                       join cp in db.CN_PayedRecord on c.iID equals cp.iMainID
                                                       where cp.iAcctBookID == acctBook.ID
                                                       select c).First();
                                            var trueKemu = piaoju.Detail[0].kemu.Substring(0, piaoju.Detail[0].kemu.Length - 2);
                                            col.cKm = trueKemu;
                                            var colbill = db.Ap_CloseBill.Where(o => o.iID == col.iID).First();
                                            colbill.cPzID = coutno_id;
                                            colbill.cCheckMan = userInfo.name.Replace("_admin", "");
                                            // colbill.dverifysystime = DateTime.Now;
                                            //colbill.dverifydate = DateTime.Now;
                                            colbill.cPZNum = dsign.U8VouchSign + "-" + ino_id.ToString().PadLeft(4, '0');
                                            colbill.doutbilldate = DateTime.Now;
                                            db.SaveChanges();
                                        }
                                        #endregion
                                        //添加流量
                                        foreach (var liu in piaoju.liuliangList)
                                        {

                                            GL_CashTable cash = new GL_CashTable()
                                            {
                                                iPeriod = Convert.ToByte(dateLast.Month),
                                                iSignSeq = 1,
                                                iNo_id = ino_id,
                                                inid = Convert.ToInt16(inid),
                                                cCashItem = liu.citemcode,
                                                md = 0.00M,//收入金额时候才用，现在只做支出
                                                mc = liu.jine,
                                                //  ccode = cashMc.Key,
                                                ccode = model.content.SubjectCode,
                                                md_f = 0.00M,
                                                mc_f = 0.00M,
                                                nd_s = 0,
                                                nc_s = 0,
                                                dbill_date = dateLast,
                                                csign = dsign.U8VouchSign,
                                                iyear = Convert.ToInt16(dateLast.Year),
                                                iYPeriod = Convert.ToInt32(dateLast.ToString("yyyyMM")),
                                                RowGuid = TimeHelp.ConvertDateTimeToInt(DateTime.Now) + "00000000",
                                            };
                                            db.GL_CashTable.Add(cash);
                                            #region 冲账金额统计（取消）
                                            /*
                                            if (liu.citemcode.Equals("04")) {
                                                request.l4 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("05"))
                                            {
                                                request.l5 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("06"))
                                            {
                                                request.l6 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("07"))
                                            {
                                                request.l7 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("13"))
                                            {
                                                request.l13 = liu.jine;
                                            }
                                            ent.SaveChanges();
                                            */
                                            #endregion
                                        }
                                        CN_VARelate v = new CN_VARelate()
                                        {
                                            AcctID = acctBook.AcctID,
                                            VouchID = i_id,
                                            IsPrint = 0
                                        };//凭证开票对应关系
                                        db.CN_VARelate.Add(v);
                                        db.SaveChanges();
                                        var CN_CashSignRelate = (from c in db.CN_CashSignRelate orderby c.ID descending select c).FirstOrDefault();
                                        string sql = string.Format(@"insert into CN_CashSignRelate(AcctBookID,JobID,VouchID,AccountID) values({0},{1},{2},{3})", acctBook.ID, CN_CashSignRelate == null ? 1 : CN_CashSignRelate.ID + 1, i_id, acctBook.AcctID);
                                        int dt = SqlHelper.ExecuteNonQuery(config, sql);
                                        if (dt < 1)
                                        {//这张表未改变
                                            throw new Exception("CN_CashSignRelate未成功添加数据，终止操作");
                                        }
                                        //科目备查表添加数据
                                        GL_CodeRemark coderremark = new GL_CodeRemark
                                        {
                                            iPeriod = Convert.ToByte(DateTime.Now.Month),
                                            iyear = Convert.ToInt16(DateTime.Now.Year),
                                            iYPeriod = Convert.ToInt32(DateTime.Now.ToString("yyyyMM")),
                                            inid = 1,
                                            csign = dsign.U8VouchSign,
                                            iNo_id = Convert.ToInt16(ino_id + "")// Convert.ToInt16(acctBook.CSNCashID);
                                        };
                                        GL_CodeRemark coderremark1 = new GL_CodeRemark
                                        {
                                            iPeriod = Convert.ToByte(DateTime.Now.Month),
                                            iyear = Convert.ToInt16(DateTime.Now.Year),
                                            iYPeriod = Convert.ToInt32(DateTime.Now.ToString("yyyyMM")),
                                            inid = 2,
                                            csign = dsign.U8VouchSign,
                                            iNo_id = Convert.ToInt16(ino_id + "")// Convert.ToInt16(acctBook.CSNCashID);
                                        };
                                        db.GL_CodeRemark.Add(coderremark);
                                        db.GL_CodeRemark.Add(coderremark1);
                                        db.SaveChanges();
                                        acctBook.VouchOutSignNum = shifouShendan == false ? "SC" + time : coutno_id;
                                        acctBook.VoucherStr = dsign.U8VouchSign + "  " + ino_id;
                                        acctBook.VoucherNum = Convert.ToInt16(ino_id + "");
                                        db.SaveChanges();
                                        acctBook.IsRegGLVouch = 1;//制单成功
                                        db.SaveChanges();
                                        model.piaoju[i].pingzhenhao = dsign.U8VouchSign + "  " + ino_id;
                                    }
                                }
                                trans.Commit();
                                tran.Commit();
                                foreach (var piaoju in model.piaoju)
                                {
                                    Log4Helper.Info(typeof(YYDal), "制单成功,凭证编号为" + piaoju.pingzhenhao);
                                }
                                return new { model, sucess = "制单成功" };
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                                return new { errorMsg = "制单失败，系统故障" };
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region 新制单
        /// <summary>
        /// 制单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public object addToCashSignRelateNew(ResultListModel model, userInfo userInfo)
        {
            if (userInfo.name == "胡姗_admin")
            {
                userInfo.name = "胡珊_admin";
            }
            if (model.content == null)
            {
                return new { errorMsg = "请选择账户" };
            }
            foreach (var item in model.piaoju)
            {
                var NullMiaoshu = item.Detail.Where(o => string.IsNullOrEmpty(o.miaoshu) == true).FirstOrDefault();
                if (NullMiaoshu == null)
                {
                    return new { errorMsg = "请填写所有摘要信息" };
                }
                var NullKemu = item.Detail.Where(o => string.IsNullOrEmpty(o.kemu) == true).FirstOrDefault();
                if (NullKemu==null)
                {
                    return "请确认每一条明细都选择了对应科目";
                }
            }
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    using (var ent = new OAtoU8DATAEntities())
                    {
                        using (var trans = ent.Database.BeginTransaction())
                        {
                            try
                            {
                                var RecordTables = (from r in ent.RecordTable
                                                    where r.Pid.Contains(model.Id) && r.IsIntoAccvouch != null
                                                    select r).FirstOrDefault();
                                if (RecordTables != null)
                                {
                                    return new { errorMsg = "已经制单（生成凭证）到U8，禁止重复提交" };
                                }
                                FromOADal oa = new FromOADal();
                                //var guanlingYuzhi = oa.getSunByattachment(model,);//获取关联的预支单
                                for (int i = 0; i < model.piaoju.Count; i++)
                                {//读取票据(原先是有多张单据的改为一张单据，可以省略外部的for)
                                    var piaoju = model.piaoju[i];
                                    var RecordTable = (from r in ent.RecordTable
                                                       where r.Pid == model.Id && r.piaojuId == piaoju.tabClass
                                                       select r).FirstOrDefault();
                                    var shifouShendan = string.IsNullOrEmpty(RecordTable.danjubianhao) ? false : true;//是否生成付款单、收款单
                                    CN_AcctBook acctBook = new CN_AcctBook();
                                    if (RecordTable == null && piaoju.Yinhan.jiefan != 0)
                                    {
                                        return new { errorMsg = "单据错误，未找到" + piaoju.tabClass + "请先日记账" };
                                    }
                                    if (RecordTable != null)
                                    {
                                        acctBook = (from b in db.CN_AcctBook where b.ID == RecordTable.Bid select b).FirstOrDefault();
                                    }
                                    DateTime date = acctBook != null ? Convert.ToDateTime(acctBook.AcctDate) : model.acctDate;
                                    DateTime dateLast = Convert.ToDateTime(date.AddMonths(1).AddDays(-(date.Day + 1)).ToString("yyyy-MM-dd"));//获取这个月的最后一天
                                    if (date.Date.Month == DateTime.Now.Month)
                                    {
                                        dateLast = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                                    }
                                    DateTime dateFirst = Convert.ToDateTime(date.AddDays(1 - date.Day).ToString("yyyy-MM-dd"));//获取这个月的第一天
                                    var LastGl = (from g in db.GL_accvouch where g.dbill_date >= dateFirst orderby g.ino_id descending select g).FirstOrDefault();
                                    short ino_id = LastGl == null ? Convert.ToInt16(1) : Convert.ToInt16(LastGl.ino_id + 1);
                                    var yinhankemu = "";
                                    foreach (Detail list in piaoju.Detail)
                                    {
                                        if (Convert.ToDecimal(list.jiefan) == 0)
                                        {
                                            if (yinhankemu.Length < 50)
                                            {
                                                yinhankemu = yinhankemu + list.kemu.Substring(0, list.kemu.Length - 2) + ",";
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    yinhankemu = yinhankemu.Substring(0, yinhankemu.Length - 2);
                                    if (piaoju.Yinhan.jiefan == 0)
                                    {//说明无流量操作，直接进总表（冲借支）
                                        short inid = 0;
                                        string left = "";//付前面的数字
                                        if (userInfo.company == (int)CompanyEnum.yueji)
                                        {//悦肌的需要截取前面六位做标识
                                            left = model.content.AcctName.Substring(0, 6);
                                        }
                                        var snuord = model.isPay ? u8.getCashSerialNumber(left + "付") : u8.getCashSerialNumber(left + "收");
                                        var dsign = (from d in db.CN_CashSerialNumber where d.SNWord.Equals(snuord) select d).FirstOrDefault();//这里保存了记账凭证号
                                        var time = TimeHelp.ConvertDateTimeToInt(dateLast);
                                        while (true)
                                        {
                                            var x = db.GL_accvouch.Where(g => g.coutno_id.Contains(time.ToString())).FirstOrDefault();
                                            if (x == null)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                time++;
                                            }
                                        }
                                        string ccodeList = "";

                                        #region 科目明细录入

                                        var LastGls = (from g in db.GL_accvouch where g.coutno_id.Contains("GL") orderby g.coutno_id descending select g).FirstOrDefault();
                                        var Gl = "GL0000000000001";
                                        if (LastGls != null)
                                        {
                                            string result = System.Text.RegularExpressions.Regex.Replace(LastGls.coutno_id, @"[^0-9]+", "");
                                            Gl = "GL" + (Convert.ToInt64(result) + 1).ToString();
                                        }
                                        var UnitID = 0; //客户/供应商id
                                                        // var UnitType = 2;//客户为1，供应商为2
                                        Customer kehus = new Customer();
                                        Vendor gonyingshangs = new Vendor();
                                        Person user = new Person();
                                        foreach (Detail list in piaoju.Detail)
                                        {
                                            var detailjine = Convert.ToDecimal(list.jine);
                                            if (!string.IsNullOrEmpty(list.miaoshu))
                                            {//获取流量集合

                                                if (string.IsNullOrEmpty(list.kemu))
                                                {
                                                    return "请确认每一条明细都选择了对应科目";
                                                }
                                                var oldKemu = list.kemu;
                                                var trueKemu = list.kemu.Substring(0, list.kemu.Length - 2);

                                                var thisCode = u8.getCodeInU8Byccode(trueKemu);
                                                string bitem = null;
                                                if (thisCode == null)
                                                {//没找到这个科目
                                                    continue;
                                                }
                                                if (thisCode.bcus)
                                                {//客户往来核算 
                                                    var kehu = u8.geCustomertUnit(model.shoukuandanwei);//客户
                                                    if (kehu != null)
                                                    {
                                                        UnitID = kehu.ID;
                                                        acctBook.UnitType = 1;//客户为1，供应商为2,根据u8上默认是选择客户的
                                                        acctBook.UnitID = UnitID;
                                                        acctBook.CustomerID = UnitID;
                                                        kehus = (from cu in db.Customer
                                                                 join c in db.CN_UnitID on cu.cCusCode equals c.LoadCusID
                                                                 where c.ID == UnitID
                                                                 select cu).FirstOrDefault();//获取客户类型的受益部门
                                                    }
                                                    else
                                                    {
                                                        return new { errorMsg = thisCode.ccode + "是客户受控科目，但是在客户列表中无法查询到" + model.shoukuandanwei + "这个客户" };
                                                    }
                                                }
                                                if (thisCode.bsup)
                                                {//供应商往来核算项目 

                                                    var gonyingshang = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                                                    if (gonyingshang != null)
                                                    {
                                                        UnitID = gonyingshang.ID;
                                                        acctBook.UnitType = 2;//客户为1，供应商为2
                                                        acctBook.UnitID = UnitID;
                                                        acctBook.VendorID = UnitID;
                                                        gonyingshangs = (from ve in db.Vendor
                                                                         join c in db.CN_UnitID on ve.cVenCode equals c.LoadVenID
                                                                         where c.ID == UnitID
                                                                         select ve).FirstOrDefault();
                                                    }
                                                    else
                                                    {
                                                        return new { errorMsg = thisCode.ccode + "是供应商受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "这个供应商" };
                                                    }
                                                }
                                                if (thisCode.bperson)
                                                {//个人往来核算项目 
                                                    user = u8.getPerson(model.shoukuandanwei);
                                                    if (user == null)
                                                    {
                                                        return new { errorMsg = thisCode.ccode + "是个人往来受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "" };
                                                    }
                                                }
                                                if (thisCode.bitem)
                                                {// bitem(U861)  是否项目核算
                                                    bitem = "1001";
                                                }

                                                if (thisCode.bdept && string.IsNullOrEmpty(list.shouyibumen))
                                                {//部门往来核算项目 
                                                    return new { errorMsg = thisCode.ccode + "部门受控,请选择部门" };
                                                }
                                                if (!thisCode.bdept)
                                                {//不是受控部门的去掉受控部门，不然在U8上回报错非受控部门
                                                    list.shouyibumen = null;
                                                }
                                                if (thisCode.ccode == "220201" && RecordTable.danjubianhao == null)
                                                {
                                                    return new { errorMsg = "本单需要生成应付单等操作，暂不支持，请自行到U8生成" };
                                                }
                                                if (Convert.ToDecimal(list.jiefan) == 0)
                                                {
                                                    gonyingshangs = null;
                                                }
                                                if (dsign == null)
                                                {
                                                    return new { errorMsg = "无记账凭证" };
                                                }
                                                inid++;

                                                ccodeList = trueKemu + ",";
                                                if (list.shouyibumen != null)
                                                {
                                                    var depatement = (from d in db.Department where d.cDepCode.Equals(list.shouyibumen) select d).FirstOrDefault();
                                                    if (depatement == null)
                                                    {
                                                        list.shouyibumen = null;
                                                    }
                                                }
                                                if (list.miaoshu.Length >= 120)
                                                {
                                                    list.miaoshu = list.miaoshu.Substring(0, 120);
                                                }
                                                GL_accvouch acc = new GL_accvouch()
                                                {
                                                    iperiod = Convert.ToByte(dateLast.Month),
                                                    csign = dsign.U8VouchSign,//凭证类别字
                                                    isignseq = 1,//凭证类别排序号 
                                                    ino_id = ino_id,//凭证编号 
                                                    inid = inid,//在12循环行号 
                                                    dbill_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//制单日期
                                                    idoc = !shifouShendan ? (short)0 : (short)1,//附单据数 (不生单为0生单为1)
                                                    cbill = userInfo.name.Replace("_admin", ""),//制单人
                                                    ccheck = null,
                                                    cbook = null,
                                                    ibook = 0,
                                                    ccashier = !shifouShendan ? userInfo.name.Replace("_admin", "") : null,//出纳签字人(不生单的签字了，生单的没有)
                                                    iflag = null,
                                                    ctext1 = null,
                                                    ctext2 = null,
                                                    cdigest = list.miaoshu,//摘要 
                                                    ccode = trueKemu,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                    cexch_name = null,
                                                    md = Convert.ToDecimal(list.jine),//借方金额 
                                                    mc = Convert.ToDecimal(list.jiefan), //  mc = i == 1 ? Convert.ToDecimal(acctBook.Credit) : 0.00M,//和上面一个有值它就为0.00M 贷方金额 
                                                    mc_f = 0.00M,//外币贷方金额  
                                                    md_f = 0.00M,//外币借方金额 
                                                    nfrat = 0,//汇率 
                                                    nc_s = 0,//数量贷方 
                                                    nd_s = 0,//数量借方 
                                                    csettle = null,// (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode,//结算方式编码 
                                                    cn_id = null,
                                                    dt_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                    cdept_id = list.shouyibumen,//==null?null: u8.getDepatements(list.shouyibumen).cDepCode,//user == null ? list.shouyibumen : user.cDepCode, //getDepatements(r.shouyibumen).cDepCode,
                                                    cperson_id = user?.cPersonCode,
                                                    ccus_id = kehus?.cCusCode,
                                                    csup_id = gonyingshangs?.cVenCode,
                                                    citem_id = bitem,
                                                    citem_class = null,
                                                    cname = !shifouShendan ? null : "-",
                                                    ccode_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                    iflagbank = null,
                                                    iflagPerson = null,
                                                    coutaccset = null,//上海悦目046，广东悦肌048
                                                    ioutyear = (short)dateLast.Year,//外部凭证会计年度 
                                                    coutsysname = null,//外部凭证系统名称
                                                    coutsysver = null,
                                                    doutbilldate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//外部凭证制单日期
                                                    ioutperiod = Convert.ToByte(dateLast.Month),//外部凭证会计期间 
                                                    coutsign = null,//外部凭证账套号 RP AP
                                                    coutno_id = Gl,//外部凭证业务号 
                                                    doutdate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                                                    coutbillsign = !shifouShendan ? null : "49",
                                                    coutid = null,
                                                    bvouchedit = true,//凭证是否可修改 
                                                    bvouchAddordele = true,//凭证分录是否可增删
                                                    bvouchmoneyhold = false,//凭证合计金额是否保值
                                                    bvalueedit = Convert.ToDecimal(list.jiefan) == 0 ? false : true,//分录数值是否可修改
                                                    bcodeedit = true,//分录科目是否可修改 
                                                    ccodecontrol = null,
                                                    bPCSedit = false,//分录往来项是否可修改 
                                                    bDeptedit = true,//分录部门是否可修改 
                                                    bItemedit = true,//分录项目是否可修改 
                                                    bCusSupInput = false,//分录往来项是否必输
                                                    iyear = Convert.ToInt16(dateLast.Year),//凭证的会计年度
                                                    cblueoutno_id = null,
                                                    ccodeexch_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode,//对方科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号) 
                                                    tvouchtime = dateLast,//凭证保存时间 
                                                    iYPeriod = Convert.ToInt32(dateLast.Year + "" + (dateLast.Month >= 10 ? dateLast.Month.ToString() : "0" + dateLast.Month)),
                                                    RowGuid = Guid.NewGuid().ToString(),//包括年度的会计期间 //行标识.规律不知道
                                                    bFlagOut = false,//公司对帐是否导出过对帐单
                                                    bdelete = false,//是否核销 
                                                };
                                                db.GL_accvouch.Add(acc);
                                                db.SaveChanges();
                                            }
                                            else
                                            {
                                                return new { errorMsg = "请填写所有摘要信息" };
                                            }
                                        }
                                        #endregion
                                        RecordTable record = new RecordTable
                                        {
                                            IsIntoBook = 0,
                                            piaojuId = i,
                                            Pid = model.Id,
                                            Bid = 0,
                                            contents = model.content.ID.ToString()
                                        };
                                        record.piaojuId = piaoju.tabClass;

                                        record.ip2 = userInfo.name;
                                        record.updateTime2 = dateLast;
                                        record.IsIntoAccvouch = 1;
                                        record.pingzhenhao = dsign.U8VouchSign + "  " + ino_id;
                                        record.userName = model.faqiren;
                                        ent.RecordTable.Add(record);
                                        ent.SaveChanges();
                                        model.piaoju[i].pingzhenhao = record.pingzhenhao;
                                    }
                                    else
                                    {//有银行流水

                                        if (model.content.ID != acctBook.AcctID)
                                        {
                                            var acct = db.CN_AcctInfo.Where(o => o.ID == acctBook.AcctID).FirstOrDefault();
                                            if (acct == null)
                                            {
                                                return new { errorMsg = "找不到该账户" };
                                            }
                                            return new { errorMsg = "生成凭证账户和日记账账户不一致，日记账账户为" + acct.AcctName };
                                        }
                                        short inid = 0;
                                        var dsign = (from d in db.CN_CashSerialNumber where d.SNWord.Contains(acctBook.CSNCashSign) select d).FirstOrDefault();//这里保存了记账凭证号

                                        var time = TimeHelp.ConvertDateTimeToInt(DateTime.Now);
                                        while (true)
                                        {
                                            var x = db.GL_accvouch.Where(g => g.coutno_id.Contains(time.ToString())).FirstOrDefault();
                                            if (x == null)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                time++;
                                            }
                                        }
                                        var coutno_id = (from g in db.GL_accvouch
                                                         where g.coutsysname.Equals("AP") && !g.coutno_id.Contains("APHCAP")
                                                         orderby g.coutno_id descending
                                                         select g.coutno_id).FirstOrDefault();
                                        if (coutno_id == null)
                                        {
                                            coutno_id = "01AP00000000001";
                                        }
                                        else
                                        {
                                            if (coutno_id.Contains("01AP"))
                                            {//存在
                                                var last = Convert.ToInt64(coutno_id.Replace("01AP", ""));
                                                last++;
                                                //coutno_id = Regex.Replace(coutno_id, @"[^0-9]+", "");//只保留数字
                                                coutno_id = "01AP" + last.ToString().PadLeft(11, '0');
                                            }
                                        }
                                        var clobil = (from cb in db.Ap_CloseBill
                                                      join c in db.Ap_CloseBills on cb.iID equals c.iID
                                                      join cp in db.CN_PayedRecord on c.iID equals cp.iMainID
                                                      where cp.iAcctBookID == acctBook.ID
                                                      select cb).FirstOrDefault();
                                        string ccodeList = "";
                                        #region 科目明细录入
                                        var i_id = 0;
                                        var UnitID = 0; //客户/供应商id
                                        //var UnitType = 2;//客户为1，供应商为2
                                        Customer kehus = new Customer();
                                        Vendor gonyingshangs = new Vendor();
                                        Person user = new Person();
                                        foreach (Detail list in piaoju.Detail)
                                        {
                                            string bitem = null;
                                            var detailjine = Convert.ToDecimal(list.jine);
                                            /* if (!string.IsNullOrEmpty(list.miaoshu))
                                             {//获取流量集合*/
                                            if (list.miaoshu.Length >= 120)
                                            {
                                                list.miaoshu = list.miaoshu.Substring(0, 120);
                                            }
                                            var oldKemu = list.kemu;
                                            var trueKemu = list.kemu.Substring(0, list.kemu.Length - 2);//旧版OA因为只能有一个枚举值，所以后两位为对应的流量排除掉
                                            var thisCode = u8.getCodeInU8Byccode(trueKemu);
                                            if (thisCode == null)
                                            {//没找到这个科目
                                                return new { errorMsg = "科目异常，你选择的科目不存在" };
                                            }
                                            if (thisCode.bcus)
                                            {//客户往来核算 
                                                var kehu = u8.geCustomertUnit(model.shoukuandanwei);//客户
                                                if (kehu != null)
                                                {
                                                    UnitID = kehu.ID;
                                                    kehus = (from cu in db.Customer
                                                             join c in db.CN_UnitID on cu.cCusCode equals c.LoadCusID
                                                             where c.ID == UnitID
                                                             select cu).FirstOrDefault();//获取客户类型的受益部门
                                                    acctBook.UnitType = 1;//客户为1，供应商为2,根据u8上默认是选择客户的
                                                    acctBook.UnitID = UnitID;
                                                    acctBook.CustomerID = UnitID;
                                                }
                                                else
                                                {
                                                    return new { errorMsg = thisCode.ccode + "是客户受控科目，但是在客户列表中无法查询到" + model.shoukuandanwei + "这个客户" };
                                                }
                                            }
                                            if (thisCode.bitem)
                                            {// bitem(U861)  是否项目核算
                                                bitem = "1001";
                                            }
                                            if (thisCode.bsup)
                                            {//供应商往来核算项目 
                                                var gonyingshang = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                                                if (gonyingshang != null)
                                                {
                                                    UnitID = gonyingshang.ID;
                                                    gonyingshangs = (from ve in db.Vendor
                                                                     join c in db.CN_UnitID on ve.cVenCode equals c.LoadVenID
                                                                     where c.ID == UnitID
                                                                     select ve).FirstOrDefault();
                                                    acctBook.UnitType = 2;//客户为1，供应商为2
                                                    acctBook.UnitID = UnitID;
                                                    acctBook.VendorID = UnitID;
                                                }
                                                else
                                                {
                                                    return new { errorMsg = thisCode.ccode + "是供应商受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "这个供应商" };
                                                }
                                            }
                                            if (thisCode.bperson)
                                            {//个人往来核算项目 
                                                user = u8.getPerson(model.shoukuandanwei);
                                                if (user == null)
                                                {
                                                    return new { errorMsg = thisCode.ccode + "是个人往来受控科目，但是在供应商列表中无法查询到" + model.shoukuandanwei + "" };
                                                }
                                            }
                                            if (thisCode.bdept && string.IsNullOrEmpty(list.shouyibumen))
                                            {//部门往来核算项目 
                                                return new { errorMsg = thisCode.ccode + "部门受控,请选择部门" };
                                            }

                                            if (!thisCode.bdept)
                                            {//不是受控部门的去掉受控部门，不然在U8上回报错非受控部门
                                                list.shouyibumen = null;
                                            }
                                            if ((thisCode.ccode == "220201" || thisCode.ccode == "1122") && RecordTable.danjubianhao == null)
                                            {
                                                return new { errorMsg = thisCode.ccode + "科目为应付系统科目，请先生成收付款单" };
                                            }
                                            if (dsign == null)
                                            {
                                                return new { errorMsg = "无记账凭证" };
                                            }
                                            inid++;
                                            ccodeList = trueKemu + ",";
                                            if (list.shouyibumen != null)
                                            {
                                                var depatement = (from d in db.Department where d.cDepCode.Equals(list.shouyibumen) select d).FirstOrDefault();
                                                if (depatement == null)
                                                {
                                                    list.shouyibumen = null;
                                                }
                                            }
                                            var ccodecontrol = "***";
                                            if (RecordTable.danjubianhao == null)
                                            {
                                                ccodecontrol = "***";//SC模块的
                                            }
                                            else
                                            {
                                                ccodecontrol = Convert.ToDecimal(list.jiefan) == 0 ? "AP" : "! AR AP #";
                                            }
                                            if (shifouShendan)
                                            {
                                                yinhankemu = model.content.SubjectCode + "|人民币";
                                            }
                                            GL_accvouch acc = new GL_accvouch
                                            {
                                                iperiod = Convert.ToByte(dateLast.Month),
                                                csign = dsign.U8VouchSign,//凭证类别字
                                                isignseq = 1,//凭证类别排序号 
                                                ino_id = ino_id,//凭证编号 
                                                inid = inid,//在12循环行号 
                                                dbill_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//制单日期
                                                idoc = !shifouShendan ? (short)0 : (short)1,//附单据数 (不生单为0生单为1)
                                                cbill = userInfo.name.Replace("_admin", ""),//制单人
                                                ccheck = null,
                                                cbook = null,
                                                ibook = 0,
                                                ccashier = !shifouShendan ? userInfo.name.Replace("_admin", "") : null,//出纳签字人(不生单的签字了，生单的没有)
                                                iflag = null,
                                                ctext1 = null,
                                                ctext2 = null,
                                                cdigest = list.miaoshu,//摘要 
                                                ccode = trueKemu,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                cexch_name = null,
                                                md = Convert.ToDecimal(list.jine), 
                                                mc = Convert.ToDecimal(list.jiefan), //  mc = i == 1 ? Convert.ToDecimal(acctBook.Credit) : 0.00M;//和上面一个有值它就为0.00M 贷方金额 
                                                mc_f = 0.00M,//外币贷方金额  
                                                md_f = 0.00M,//外币借方金额 
                                                nfrat = 0,//汇率 
                                                nc_s = 0,//数量贷方 
                                                nd_s = 0,//数量借方 
                                                csettle = Convert.ToDecimal(list.jiefan) == 0 ? null :acctBook.SettleTypeID.ToString().PadLeft(2,'0'), //(from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode,// (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode;//结算方式编码 
                                                cn_id = null,
                                                dt_date = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cdept_id = list.shouyibumen,
                                                //user == null ? list.shouyibumen : user.cDepCode; //getDepatements(r.shouyibumen).cDepCode;
                                                cperson_id = user?.cPersonCode,
                                                ccus_id = kehus?.cCusCode,
                                                csup_id = gonyingshangs?.cVenCode,
                                                citem_id = bitem,
                                                citem_class = null,
                                                cname = !shifouShendan ? null : "-",
                                                ccode_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                iflagbank = null,
                                                iflagPerson = null,
                                                coutaccset = !shifouShendan ? null : coutaccset,//上海悦目046，广东悦肌048
                                                ioutyear = !shifouShendan ? (short)0 : (short)DateTime.Now.Year,//外部凭证会计年度 
                                                coutsysname = !shifouShendan ? "SC" : "AP",//外部凭证系统名称
                                                coutsysver = null,
                                                doutbilldate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),//外部凭证制单日期
                                                ioutperiod = Convert.ToByte(dateLast.Month),//外部凭证会计期间 
                                                coutsign = !shifouShendan ? "出纳管理" : "AP",//外部凭证账套号 RP AP
                                                coutno_id = !shifouShendan ? "SC" + time : coutno_id,//外部凭证业务号 
                                                                                                     //doutdate = !shifouShendan ?null : Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd"));
                                                coutbillsign = !shifouShendan ? null : "49",
                                                coutid = clobil?.cVouchID,
                                                bvouchedit = true,//凭证是否可修改 
                                                bvouchAddordele = true,//凭证分录是否可增删
                                                bvouchmoneyhold = false,//凭证合计金额是否保值
                                                bvalueedit = true,//分录数值是否可修改
                                                bcodeedit = true,//分录科目是否可修改 
                                                ccodecontrol = ccodecontrol,
                                                bPCSedit = Convert.ToDecimal(list.jiefan) == 0 ? false : true,//分录往来项是否可修改 
                                                bDeptedit = true,//分录部门是否可修改 
                                                bItemedit = true,//分录项目是否可修改 
                                                bCusSupInput = Convert.ToDecimal(list.jiefan) == 0 ? false : true,//分录往来项是否必输
                                                iyear = Convert.ToInt16(dateLast.Year),//凭证的会计年度
                                                cblueoutno_id = null,
                                                ccodeexch_equal = Convert.ToDecimal(list.jiefan) == 0 ? yinhankemu : model.content.SubjectCode, //Convert.ToDecimal(list.jiefan) == 0 ? model.content.SubjectCode+ "|人民币" : model.content.SubjectCode;//对方科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号) 
                                                tvouchtime = DateTime.Now,//凭证保存时间
                                                iYPeriod = Convert.ToInt32(dateLast.Year + "" + (dateLast.Month >= 10 ? dateLast.Month.ToString() : "0" + dateLast.Month)),
                                                RowGuid = Guid.NewGuid().ToString(),//包括年度的会计期间 //行标识.规律不知道
                                                bFlagOut = false,//公司对帐是否导出过对帐单
                                                bdelete = false//是否核销 
                                            };
                                            if (!shifouShendan)
                                            {
                                                acc.doutdate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd"));
                                            }
                                            db.GL_accvouch.Add(acc);
                                            db.SaveChanges();
                                            i_id = acc.i_id;
                                            /* }
                                             else
                                             {
                                                 return new { errorMsg = "请填写所有摘要信息" };
                                             }*/
                                        }
                                        #endregion
                                        #region 添加个人记录表
                                        var request = (from re in ent.RecordTable
                                                       where re.Bid == acctBook.ID
                                                       select re).FirstOrDefault();
                                        request.ip2 = userInfo.name;
                                        request.updateTime2 = dateLast;
                                        request.IsIntoAccvouch = 1;
                                        request.pingzhenhao = dsign.U8VouchSign + "  " + ino_id;
                                        request.userName = model.faqiren;
                                        ent.SaveChanges();
                                        #endregion
                                        #region 银行录入
                                        /* inid++;
                                         ccodeList = ccodeList.Substring(0, ccodeList.Length - 1);

                                         GL_accvouch accs = new GL_accvouch()
                                         {//付款方录入
                                             iperiod = Convert.ToByte(DateTime.Now.Month),
                                             csign = dsign.U8VouchSign,//凭证类别字
                                             isignseq = 1,//凭证类别排序号 
                                             ino_id = ino_id,//凭证编号 
                                             inid = Convert.ToInt16(inid),//在12循环行号 
                                             dbill_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),//制单日期
                                             idoc = shifouShendan == false ? (short)0 : (short)1,//附单据数 (不生单为0生单为1)
                                             cbill = userInfo.name,//制单人
                                             ccheck = null,
                                             cacctBook = null,
                                             iacctBook = 0,
                                             ccashier = shifouShendan == false ? acctBook.Cashier : null,//出纳签字人(不生单的签字了，生单的没有)
                                             iflag = null,
                                             ctext1 = null,
                                             ctext2 = null,
                                             cdigest = acctBook.Summary,//摘要 
                                             ccode = model.content.SubjectCode,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                             cexch_name = null,
                                             nc_s = 0,//数量贷方 
                                             nd_s = 0,//数量借方 
                                             nfrat = 0,//汇率 
                                             mc_f = 0.00M,//外币贷方金额  
                                             md_f = 0.00M,//外币借方金额
                                             md = 0.00M, //借方金额 
                                             mc = Convert.ToDecimal(piaoju.Yinhan.jiefan), //Convert.ToDecimal(piaoju.yinhuan),//贷方金额 
                                             csettle = null,// (from s in db.SettleStyle where s.cSSName.Equals("转账") select s).First().cSSCode,//结算方式编码 
                                             cn_id = null,
                                             dt_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),
                                             cdept_id = user == null ? u8.getDepatements(model.faqibumen).cDepCode : user.cDepCode, //getDepatements(r.shouyibumen).cDepCode,
                                             cperson_id = user == null ? null : user.cPersonCode,
                                             ccus_id = kehus == null ? null : kehus.cCusCode,
                                             csup_id = gonyingshangs == null ? null : gonyingshangs.cVenCode,
                                             citem_id = null,
                                             citem_class = null,
                                             cname = shifouShendan == false ? null : "-",
                                             ccode_equal = ccodeList,//科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                             iflagbank = null,
                                             iflagPerson = null,
                                             coutaccset = "046",//上海悦目046，广东悦肌048
                                             ioutyear = (short)DateTime.Now.Year,//外部凭证会计年度 
                                             coutsysname = acctBook.UnitID == 0 ? "SC" : "AP",//外部凭证系统名称
                                             coutsysver = null,
                                             doutbilldate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),//外部凭证制单日期
                                             ioutperiod = Convert.ToByte(DateTime.Now.Month),//外部凭证会计期间 
                                             coutsign = shifouShendan == false ? "出纳管理" : "RP",//外部凭证账套号 RP AP
                                             coutno_id = shifouShendan == false ? "SC" + time : coutno_id,//外部凭证业务号 
                                             doutdate = DateTime.Now,
                                             coutbillsign = shifouShendan == false ? null : "49",
                                             coutid = clobil == null ? null : clobil.cVouchID + "                  " + inid,
                                             bvouchedit = true,//凭证是否可修改 
                                             bvouchAddordele = true,//凭证分录是否可增删
                                             bvouchmoneyhold = false,//凭证合计金额是否保值
                                             bvalueedit = false,//分录数值是否可修改
                                             bcodeedit = true,//分录科目是否可修改 
                                             ccodecontrol = "AP",
                                             bPCSedit = false,//分录往来项是否可修改 
                                             bDeptedit = true,//分录部门是否可修改 
                                             bItemedit = true,//分录项目是否可修改 
                                             bCusSupInput = false,//分录往来项是否必输
                                             iyear = Convert.ToInt16(DateTime.Now.Year),//凭证的会计年度
                                             cblueoutno_id = null,
                                             ccodeexch_equal = model.content.SubjectCode,//对方科目编码 ，付款的用申请表里的付款科目来做查询(付款为你选择的科目编号)
                                                                                         // daudit_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")),//凭证审核日期 
                                             tvouchtime = DateTime.Now,//凭证保存时间 
                                             iYPeriod = Convert.ToInt32(DateTime.Now.Year + "" + (DateTime.Now.Month >= 10 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month)),
                                             RowGuid = Guid.NewGuid().ToString(),//包括年度的会计期间 
                                             bFlagOut = false,//公司对帐是否导出过对帐单
                                             bdelete = false,//是否核销 
                                         };
                                         db.GL_accvouch.Add(accs);
                                         db.SaveChanges();*/
                                        #endregion
                                        #region 如果生单，则需要修改生单表数据
                                        if (RecordTable.danjubianhao != null)
                                        {//生单过
                                            var kehu = u8.geCustomertUnit(model.shoukuandanwei);//客户
                                            if (kehu != null)
                                            {
                                                UnitID = kehu.ID;
                                                kehus = (from cu in db.Customer
                                                         join c in db.CN_UnitID on cu.cCusCode equals c.LoadCusID
                                                         where c.ID == UnitID
                                                         select cu).FirstOrDefault();//获取客户类型的受益部门
                                            }
                                            var cCusVen = kehus == null ? "" : kehus.cCusCode;
                                            var gonyingshang = u8.GeVendortUnit(model.shoukuandanwei);//供应商
                                            if (gonyingshang != null)
                                            {
                                                UnitID = gonyingshang.ID;
                                                gonyingshangs = (from ve in db.Vendor
                                                                 join c in db.CN_UnitID on ve.cVenCode equals c.LoadVenID
                                                                 where c.ID == UnitID
                                                                 select ve).FirstOrDefault();
                                            }
                                            cCusVen = gonyingshangs == null ? "" : gonyingshangs.cVenCode;
                                            Ap_Detail detail = new Ap_Detail()
                                            {
                                                iPeriod = Convert.ToByte(dateLast.Month),
                                                cVouchType = "49",
                                                cVouchSType = null,
                                                cVouchID = dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                dVouchDate = dateLast,
                                                dRegDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cDwCode = cCusVen,
                                                iBVid = 0,
                                                cCode = "100202",
                                                isignseq = Convert.ToByte(dateLast.Month),
                                                ino_id = null,
                                                cDigest = acctBook.Summary,
                                                iPrice = 0,
                                                cexch_name = "人民币",
                                                iExchRate = 1,
                                                iDAmount = 0.00M,
                                                iCAmount = acctBook.Credit,
                                                iDAmount_f = 0.00M,
                                                iCAmount_s = (double)acctBook.Credit,
                                                iDAmount_s = 0,
                                                iCAmount_f = 0,
                                                cSSCode = acctBook.SettleTypeID.ToString().PadLeft(2,'0'),
                                                cProcStyle = "49",
                                                cCancelNo = "AP49" + dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                bPrePay = false,
                                                iFlag = 6,
                                                cCoVouchType = "49",
                                                cCoVouchID = dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                cFlag = "AP",
                                                iClosesID = 0,
                                                iCoClosesID = 0,
                                                cGLSign = dsign.U8VouchSign,
                                                iGLno_id = (short)acctBook.CSNCashID,
                                                dPZDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cOperator = acctBook.Cashier,
                                                cCheckMan = acctBook.Cashier,
                                                cPZid = coutno_id

                                            };
                                            Ap_Detail details = new Ap_Detail()
                                            {
                                                iPeriod = Convert.ToByte(dateLast.Month),
                                                cVouchType = "49",
                                                cVouchSType = null,
                                                cVouchID = dateLast.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                dVouchDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                dRegDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cDwCode = cCusVen,
                                                iBVid = 0,
                                                ino_id = ino_id,
                                                cCode = "220201",
                                                isignseq = Convert.ToByte(dateLast.Month),
                                                cDigest = acctBook.Summary,
                                                iPrice = 0,
                                                cexch_name = "人民币",
                                                iExchRate = 1,
                                                iDAmount = acctBook.Credit,
                                                iCAmount = 0.00M,
                                                iDAmount_f = acctBook.Credit,
                                                iCAmount_s = 0.00,
                                                iDAmount_s = 0,
                                                iCAmount_f = 0,
                                                cSSCode = acctBook.SettleTypeID.ToString().PadLeft(2, '0'),//settleStyle.First().cSSCode,
                                                cProcStyle = "49",
                                                cCancelNo = "AP49" + DateTime.Now.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                bPrePay = true,
                                                iFlag = 6,
                                                cCoVouchType = "49",
                                                cCoVouchID = DateTime.Now.ToString("yyMM") + ino_id.ToString().PadLeft(8, '0'),
                                                cFlag = "AP",
                                                iClosesID = 0,
                                                iCoClosesID = 0,
                                                cGLSign = dsign.U8VouchSign,
                                                iGLno_id = (short)acctBook.CSNCashID,
                                                dPZDate = Convert.ToDateTime(dateLast.ToString("yyyy-MM-dd")),
                                                cOperator = acctBook.Cashier,
                                                cCheckMan = acctBook.Cashier,
                                                cPZid = coutno_id,
                                                iAmount = acctBook.Credit,
                                                iAmount_f = acctBook.Credit,
                                                iAmount_s = 0,
                                                iVouchAmount = acctBook.Credit,
                                                iVouchAmount_f = acctBook.Credit,
                                                iVouchAmount_s = 0
                                            };
                                            db.Ap_Detail.Add(detail);
                                            db.Ap_Detail.Add(details);
                                            db.SaveChanges();
                                            //修改生单
                                            var col = (from c in db.Ap_CloseBills
                                                       join cp in db.CN_PayedRecord on c.iID equals cp.iMainID
                                                       where cp.iAcctBookID == acctBook.ID
                                                       select c).First();
                                            var trueKemu = piaoju.Detail[0].kemu.Substring(0, piaoju.Detail[0].kemu.Length - 2);
                                            col.cKm = trueKemu;
                                            var colbill = db.Ap_CloseBill.Where(o => o.iID == col.iID).First();
                                            colbill.cPzID = coutno_id;
                                            colbill.cCheckMan = userInfo.name.Replace("_admin", "");
                                            // colbill.dverifysystime = DateTime.Now;
                                            //colbill.dverifydate = DateTime.Now;
                                            colbill.cPZNum = dsign.U8VouchSign + "-" + ino_id.ToString().PadLeft(4, '0');
                                            colbill.doutbilldate = DateTime.Now;
                                            db.SaveChanges();
                                        }
                                        #endregion
                                        //添加流量
                                        foreach (var liu in piaoju.liuliangList)
                                        {

                                            GL_CashTable cash = new GL_CashTable()
                                            {
                                                iPeriod = Convert.ToByte(dateLast.Month),
                                                iSignSeq = 1,
                                                iNo_id = ino_id,
                                                inid = Convert.ToInt16(inid),
                                                cCashItem = liu.citemcode,
                                                md = liu.md,// 0.00M,//(收入)
                                                mc = liu.jine,
                                                //  ccode = cashMc.Key,
                                                ccode = model.content.SubjectCode,
                                                md_f = 0.00M,
                                                mc_f = 0.00M,
                                                nd_s = 0,
                                                nc_s = 0,
                                                dbill_date = dateLast,
                                                csign = dsign.U8VouchSign,
                                                iyear = Convert.ToInt16(dateLast.Year),
                                                iYPeriod = Convert.ToInt32(dateLast.ToString("yyyyMM")),
                                                RowGuid = TimeHelp.ConvertDateTimeToInt(DateTime.Now) + "00000000",
                                            };
                                            db.GL_CashTable.Add(cash);
                                            #region 冲账金额统计（取消）
                                            /*
                                            if (liu.citemcode.Equals("04")) {
                                                request.l4 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("05"))
                                            {
                                                request.l5 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("06"))
                                            {
                                                request.l6 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("07"))
                                            {
                                                request.l7 = liu.jine;
                                            }
                                            if (liu.citemcode.Equals("13"))
                                            {
                                                request.l13 = liu.jine;
                                            }
                                            ent.SaveChanges();
                                            */
                                            #endregion
                                        }
                                        CN_VARelate v = new CN_VARelate()
                                        {
                                            AcctID = acctBook.AcctID,
                                            VouchID = i_id,
                                            IsPrint = 0
                                        };//凭证开票对应关系
                                        db.CN_VARelate.Add(v);
                                        db.SaveChanges();
                                        var CN_CashSignRelate = (from c in db.CN_CashSignRelate orderby c.ID descending select c).FirstOrDefault();
                                        string sql = string.Format(@"insert into CN_CashSignRelate(AcctBookID,JobID,VouchID,AccountID) values({0},{1},{2},{3})", acctBook.ID, CN_CashSignRelate == null ? 1 : CN_CashSignRelate.ID + 1, i_id, acctBook.AcctID);
                                        int dt = SqlHelper.ExecuteNonQuery(config, sql);
                                        if (dt < 1)
                                        {//这张表未改变
                                            throw new Exception("CN_CashSignRelate未成功添加数据，终止操作");
                                        }
                                        //科目备查表添加数据
                                        GL_CodeRemark coderremark = new GL_CodeRemark
                                        {
                                            iPeriod = Convert.ToByte(DateTime.Now.Month),
                                            iyear = Convert.ToInt16(DateTime.Now.Year),
                                            iYPeriod = Convert.ToInt32(DateTime.Now.ToString("yyyyMM")),
                                            inid = 1,
                                            csign = dsign.U8VouchSign,
                                            iNo_id = Convert.ToInt16(ino_id + "")// Convert.ToInt16(acctBook.CSNCashID);
                                        };
                                        GL_CodeRemark coderremark1 = new GL_CodeRemark
                                        {
                                            iPeriod = Convert.ToByte(DateTime.Now.Month),
                                            iyear = Convert.ToInt16(DateTime.Now.Year),
                                            iYPeriod = Convert.ToInt32(DateTime.Now.ToString("yyyyMM")),
                                            inid = 2,
                                            csign = dsign.U8VouchSign,
                                            iNo_id = Convert.ToInt16(ino_id + "")// Convert.ToInt16(acctBook.CSNCashID);
                                        };
                                        db.GL_CodeRemark.Add(coderremark);
                                        db.GL_CodeRemark.Add(coderremark1);
                                        db.SaveChanges();
                                        acctBook.VouchOutSignNum = shifouShendan == false ? "SC" + time : coutno_id;
                                        acctBook.VoucherStr = dsign.U8VouchSign + "  " + ino_id;
                                        acctBook.VoucherNum = Convert.ToInt16(ino_id + "");
                                        db.SaveChanges();
                                        acctBook.IsRegGLVouch = 1;//制单成功
                                        db.SaveChanges();
                                        model.piaoju[i].pingzhenhao = dsign.U8VouchSign + "  " + ino_id;
                                    }
                                }
                                trans.Commit();
                                tran.Commit();
                                foreach (var piaoju in model.piaoju)
                                {
                                    Log4Helper.Info(typeof(YYDal), "制单成功,凭证编号为" + piaoju.pingzhenhao);
                                }
                                return new { model, sucess = "制单成功" };
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                                return new { errorMsg = "制单失败，系统故障" };
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region 签字（本来是和制单一起的）
        /// <summary>
        /// 签字
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string addToCcashier(ResultListModel model)
        {
            using (var ent = new OAtoU8DATAEntities())
            {//读取到自己设计表的数据
                var list = (from r in ent.RecordTable
                            where r.Pid.Contains(model.Id)
                            select r).ToList();

                using (var db = new UFDATA_048_2017Entities(u8Db))
                {
                    using (var tran = db.Database.BeginTransaction())

                    {
                        try
                        {
                            var RecordTables = (from r in ent.RecordTable
                                                where r.Pid.Contains(model.Id) && r.IsIntoAccvouch != null
                                                select r).FirstOrDefault();
                            if (RecordTables == null)
                            {
                                return "请先制单";
                            }
                            foreach (var r in list)
                            {
                                var cash = (from c in db.CN_CashSignRelate where c.AcctBookID == r.Bid select c).FirstOrDefault();
                                if (cash == null)
                                {
                                    return "请先制单";
                                }
                                var acc = (from a in db.GL_accvouch
                                           where a.i_id == cash.VouchID
                                           select a
                                       ).FirstOrDefault();
                                acc.ccashier = GetIP.getIP().userName;//签字步骤放到签字去 book.Cashier,//出纳签字人 
                                db.SaveChanges();
                            }
                            tran.Commit();
                            return "签字成功";
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                            return "签字失败";
                        }
                    }
                }
            }
        }
        #endregion
    }
}
