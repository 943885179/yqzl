using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using EntityFromework;
using Log;
using model;

namespace dal
{
    /// <summary>
    /// 银企直联
    /// </summary>
    public class YqzlDal
    {
        /// <summary>
        /// 读取银企直联的账号
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public yq_userAccount yq_userAccount(Content content)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_userAccount.Where(o => o.accountId == content.BankAcct).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取错误
        /// </summary>
        public yq_error Getyq_error(string resultCode)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                return db.yq_error.Where(o => o.errCode == resultCode).FirstOrDefault();
            }
        }
        /// <summary>
        /// 读取交易记录是否存在
        /// </summary>
        /// <param name="ThirdVoucher"></param>
        /// <returns></returns>
        public yq_paymentRecord yq_paymentRecordByThirdVoucher(string ThirdVoucher)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_paymentRecord.Where(o => o.thirdVoucher == ThirdVoucher && o.isDel==null).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 更新交易记录是否存在
        /// </summary>
        /// <param name="pay"></param>
        /// <returns></returns>
        public Boolean editPay(yq_paymentRecord pay)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var pays = db.yq_paymentRecord.Where(o => o.id == pay.id).FirstOrDefault();
                    pays.frontLogNo = pay.frontLogNo;
                    pays.unionFlag = pay.unionFlag;
                    pays.stt = pay.stt;
                    pays.fee1 = pay.fee1;
                    pays.fee2 = pay.fee2;
                    pays.hostFlowNo = pay.hostFlowNo;
                    pays.hostTxDate = pay.hostTxDate;
                    pays.isApproval = pay.isApproval;
                    pays.approvalMan = pay.approvalMan;
                    pays.approvalDate = pay.approvalDate;
                    pays.requestdata = pay.requestdata;
                    pays.responsedata = pay.responsedata;
                    pays.thirdVoucher = pay.thirdVoucher;
                    pays.hostTxDate = pay.hostTxDate;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// 添加日志记录
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="yqCode">编码</param>
        /// <param name="data">请求参数</param>
        /// <param name="result">返回参数</param>
        /// <param name="name">请求人</param>
        public void AddLog(string message, string yqCode, string data, string result, string name)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                yq_log log = new yq_log();
                log.createTime = DateTime.Now;
                log.ip = GetIP.GetWebClientIp();
                log.message = message;
                log.requestXml = data;
                log.responseXml = result;
                log.yqCode = yqCode;
                log.userName = name;
                db.yq_log.Add(log);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// 添加付款记录
        /// </summary>
        /// <param name="yq_PaymentRecord"></param>
        public Boolean AddYq_paymentRecord(yq_paymentRecord yq_PaymentRecord)
        {
            try
            {
                if (yq_PaymentRecord == null)
                {
                    throw new ArgumentNullException(nameof(yq_PaymentRecord));
                }

                using (var db = new OAtoU8DATAEntities())
                {

                    db.yq_paymentRecord.Add(yq_PaymentRecord);
                    db.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.ToString(), "Yq_paymentRecordErrorLog");
                return false;
            }
        }
        /// <summary>
        /// 读取招行的地区编码
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public yq_cityCode_zhaohang yq_cityCode_zhaohang(string cityName)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_cityCode_zhaohang.Where(o => o.name.Equals(cityName)).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取银行
        /// </summary>
        /// <param name="yh"></param>
        /// <returns></returns>
        public yq_zhaohang_yhCode yhCode(string yh)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var list = db.yq_zhaohang_yhCode.ToList();
                    foreach (var item in list)
                    {
                        if (yh.Contains(item.name))
                        {
                            return item;
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 删除交易记录
        /// </summary>
        /// <returns></returns>
        public yq_paymentRecord delPay(int id)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                   var pay=  db.yq_paymentRecord.Where(o => o.id == id ).FirstOrDefault();
                    // db.yq_paymentRecord.Remove(pay);
                    pay.isDel = true;
                    db.SaveChanges();

                    return pay;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 读取交易记录
        /// </summary>
        /// <returns></returns>
        public List<yq_paymentRecord> yq_paymentRecordList(string isPay, int company)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    if (isPay == null)
                    {//找寻审批后状态未更新的记录
                        return db.yq_paymentRecord.Where(o => o.isApproval == true && (o.stt == "NTE" || o.stt == "40") && o.company == company && o.isDel==null).OrderBy(o => o.thirdVoucher).ToList();
                    }
                    else if (isPay == "Yes")
                    {
                        return db.yq_paymentRecord.Where(o => o.isApproval == true && o.company == company && o.isDel == null).OrderBy(o => o.thirdVoucher).ToList();
                    }
                    else
                    {//状态未更新
                        return db.yq_paymentRecord.Where(o => o.isApproval == false && o.company == company && o.isDel == null).OrderBy(o => o.thirdVoucher).ToList();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过流水号获取交易信息
        /// </summary>
        /// <param name="liushuihao"></param>
        /// <returns></returns>
        public yq_paymentRecord yq_paymentRecordByliushuihao(string liushuihao)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_paymentRecord.Where(o => o.thirdVoucher.Equals(liushuihao) && o.isDel == null).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 撤销交易
        /// </summary>
        /// <param name="liushuihao"></param>
        /// <returns></returns>
        public string RemovePay(string liushuihao)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var pay = db.yq_paymentRecord.Where(o => o.thirdVoucher.Equals(liushuihao) && o.stt == null).FirstOrDefault();
                    if (pay == null)
                    {
                        return "已发生付款无法撤销";
                    }
                    else
                    {
                        db.yq_paymentRecord.Remove(pay);
                        db.SaveChanges();
                        return "撤销成功";
                    }
                }
                catch (Exception ex)
                {
                    return "撤销失败，" + ex.ToString();
                }
            }
        }
        /// <summary>
        /// 通过OA流水号获取交易信息
        /// </summary>
        /// <param name="liushuihao"></param>
        /// <returns></returns>
        public yq_paymentRecord yq_paymentRecordByOAliushuihao(string liushuihao)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_paymentRecord.Where(o => o.cstInnerFlowNo.Equals(liushuihao) && o.isDel == null).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回城市
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public ecs_region ecs_region(string city)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.ecs_region.Where(o => o.region_name.Contains(city)).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过等级返回城市列表
        /// </summary>
        /// <param name="type">国家：0 省：1，市：2 区：3</param>
        /// <returns></returns>
        public List<ecs_region> ecs_regionList(int type)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.ecs_region.Where(o => o.region_type == type).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过父ID返回城市列表
        /// </summary>
        /// <param name="pid">国家：0 省：1，市：2 区：3</param>
        /// <returns></returns>
        public List<ecs_region> ecs_regionListByPid(int pid)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.ecs_region.Where(o => o.parent_id == pid).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回所属省市区
        /// </summary>
        /// <param name="kaihuhan"></param>
        /// <returns></returns>
        public List<ecs_region> ecs_regionPcity_New(string kaihuhan)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var qulist = db.ecs_region.Where(o => o.region_type == 3).OrderByDescending(o => o.region_type).ToList();
                    List<ecs_region> list = new List<ecs_region>();
                    //针对到区的分行比如招商银行广州东山支行
                    foreach (var qu in qulist)
                    {
                        if (kaihuhan.Contains(qu.region_name.Replace("市", "").Replace("县", "").Replace("区", "")))
                        {
                            var city = db.ecs_region.Where(o => o.region_type == 2 && o.region_id == qu.parent_id).OrderByDescending(o => o.region_type).FirstOrDefault();
                            if (kaihuhan.Contains(city.region_name))
                            {
                                var pList = db.ecs_region.Where(o => o.region_type == 1 && o.region_id == city.parent_id).OrderByDescending(o => o.region_type).FirstOrDefault();
                                list.Add(pList);
                                list.Add(city);
                                list.Add(qu);
                                return list;
                            }
                        }
                    }
                    if (list.Count == 0)
                    {//未读取到区，比如 招商银行漳州分行
                        var cityList = db.ecs_region.Where(o => o.region_type == 2).OrderByDescending(o => o.region_type).ToList();
                        foreach (var city in cityList)
                        {
                            if (kaihuhan.Contains(city.region_name))
                            {
                                var pList = db.ecs_region.Where(o => o.region_type == 1 && o.region_id == city.parent_id).OrderByDescending(o => o.region_type).FirstOrDefault();
                                list.Add(pList);
                                list.Add(city);
                                return list;
                            }
                        }
                    }
                    return list;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回城市
        /// </summary>
        /// <param name="kaihuhan"></param>
        /// <returns></returns>
        public string ecs_regionPcity(string kaihuhan)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var list = db.ecs_region.OrderByDescending(o => o.region_type).ToList();
                    foreach (var item in list)
                    {
                        if (kaihuhan.Contains(item.region_name.Replace("市", "").Replace("县", "").Replace("区", "")))
                        {
                            return ecs_regionCity(item);
                        }

                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 返回省市
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public string ecs_regionCity(ecs_region city)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var parent = db.ecs_region.Where(o => o.region_id.Equals(city.parent_id)).FirstOrDefault();
                    if (parent.region_type == 1)
                    {
                        return parent.region_name + city.region_name;
                    }
                    return ecs_regionCity(parent);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
