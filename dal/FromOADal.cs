using common;
using Helpers;
using model;
using Model;
using EntityFromework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dal
{
    /// <summary>
    /// OA数据获取
    /// </summary>
    public class FromOADal
    {
        /// <summary>
        /// OA数据库连接字符串
        /// </summary>
        private static string config = ConfigurationManager.AppSettings["OAslqserver"].ToString();
        /// <summary>
        /// u8oa中间表名称，因为要两个数据库联查
        /// </summary>
        private static string IntermediateDb = ConfigurationManager.AppSettings["IntermediateDb"].ToString();
        /// <summary>
        /// 获取管理员列表
        /// </summary>
        /// <returns></returns>
        public List<form_enumvalue> getDepartmentHeadList()
        {
            using (var db = new v3xEntities())
            {
                var code = (from v in db.form_enumvalue join k in db.form_enumlist on v.ref_enumid equals k.id
                            where k.enumname.Equals("部门负责人经理枚举") || k.enumname.Equals("悦肌负责人")
                            orderby v.sortnumber ascending select v).ToList();
                return code;
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        public v3x_org_member getV3xUserByName(string name)
        {
            using (var db = new v3xEntities())
            {
                try
                {

                    var users = db.v3x_org_member.Where(o => o.name.Equals(name) && o.is_deleted == 0).FirstOrDefault();
                    return users;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取科目大纲
        /// </summary>
        /// <returns></returns>
        public List<form_enumvalue> getCode(string type)
        {
            using (var db = new v3xEntities())
            {
                var code = (from v in db.form_enumvalue join k in db.form_enumlist on v.ref_enumid equals k.id where k.enumname.Equals(type) orderby v.sortnumber ascending select v).ToList();
                return code;
            }
        }

        /// <summary>
        /// 获取出纳编号 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object getChunaBianhao(string id)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                var recordTablesList = (from r in ent.RecordTable
                                        where r.Pid.Contains(id)
                                        select r).ToList();
                return recordTablesList;
            }
        }
        /// <summary>
        /// 手工录入u8标记添加 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public object addToYYBymaual(List<ResultListModel> model, userInfo user)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                try
                {
                    foreach (var item in model)
                    {
                        var record = ent.RecordTable.Where(o => o.Pid == item.Id).FirstOrDefault();
                        if (record == null)
                        {//没有添加过的
                            record = new RecordTable
                            {
                                IsIntoBook = 1,
                                piaojuId = 1,
                                Pid = item.Id,
                                Bid = 0,
                                ip = user.name,
                                userName = item.faqiren,
                                updateTime = DateTime.Now,
                                contents = "",
                                chunabianhao = "手工记账"
                            };
                            record.piaojuId = 0;
                            ent.RecordTable.Add(record);
                            ent.SaveChanges();
                        }
                    }
                    return new { start=0, sucess = "状态修改成功" };
                }
                catch (Exception ex)
                {
                    return new { start = 1 , errorMsg = "系统故障"+ex.ToString() };
                }
            }
        }
        /// <summary>
        /// 标记为删除 
        /// </summary>
        /// <param name="Liushuihao"></param>
        /// <returns></returns>
        public RecordTable delRecordTable(string Liushuihao)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                try
                {
                    var record = ent.RecordTable.Where(o => o.liushuihao ==Liushuihao).FirstOrDefault();
                    record.isDel = true;
                    ent.SaveChanges();
                    return record;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 手工录入u8标记删除 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public object delToYYBymaual(List<ResultListModel> model, userInfo user)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                try
                {
                    foreach (var item in model)
                    {
                        var Record = ent.RecordTable.Where(o => o.Pid == item.Id && o.chunabianhao.Equals("手工记账")).FirstOrDefault();
                        if (Record != null)
                        {
                            ent.RecordTable.Remove(Record);
                        }
                        ent.SaveChanges();
                    }
                    return new { start = 0, sucess = "手工录入标记已取消成功" };
                }
                catch (Exception ex)
                {

                    return new { start = 1, errorMsg = "系统故障" + ex.ToString() };
                }
            }
        }
        /// <summary>
        /// 获取审单财务列表
        /// </summary>
        /// <returns></returns>
        public List<form_enumvalue> getCiawuList(){
            using (var db = new v3xEntities())
            {
                return db.form_enumvalue.Where(o=>(o.ref_enumid== -3479054130122240034 || o.ref_enumid == 5578684917746190721 || o.ref_enumid == -6789622439809758250) && o.ifuse.Equals("Y")).ToList();
            }
        }
        /// <summary>
        /// 转移审单财务
        /// </summary>
        /// <param name="type">单据类型</param>
        /// <param name="tabName">转移审单财务表格名称</param>
        /// <param name="liushuiId">转移的字段流水号</param>
        /// <param name="liushuiCol"></param>
        /// <param name="fromFinancial">转移前财务</param>
        /// <param name="toFinancial">转移到的财务</param>
        /// <param name="caiwuCol">财务所在的字段名</param>
        /// <param name="user">操作人员</param>
        /// <returns></returns>
        public object editFinancial(string type,string tabName,string liushuiId,string liushuiCol, string fromFinancial, string toFinancial,string caiwuCol, userInfo user)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                using (var v3x = new v3xEntities())
                {
                    try
                    {
                        if (type.Equals("差旅费报销单")) {
                            return new { start = 1, errorMsg = type+"的指定审单财务为胡姗，暂时无法修改,请联系管理员！" };
                        }
                        if (fromFinancial == toFinancial)
                        {
                            return new { start = 1, errorMsg = "无需变更！" };
                        }
                        var from=v3x.form_enumvalue.Where(o => (o.ref_enumid == -3479054130122240034 || o.ref_enumid == 5578684917746190721 || o.ref_enumid == -6789622439809758250) && o.enumvalue.Equals(fromFinancial)).FirstOrDefault();
                        var to = v3x.form_enumvalue.Where(o => (o.ref_enumid == -3479054130122240034 || o.ref_enumid == 5578684917746190721 || o.ref_enumid == -6789622439809758250) && o.enumvalue.Equals(toFinancial)).FirstOrDefault();
                        var record = new FinancialRecords()
                        {
                            beginFinancial = from.showValue,
                            endFinancial = to.showValue,
                            createDate = DateTime.Now,
                            liushuiId = liushuiId,
                            createMan = user.name

                        };
                        string sql = "update "+ tabName + " set "+ caiwuCol + "='"+to.enumvalue+"' where  "+ liushuiCol+"='"+liushuiId+"';";
                         var result= SqlHelper.ExecuteNonQuery(config, sql);
                        if (result>0)
                        {
                            ent.FinancialRecords.Add(record);
                            ent.SaveChanges();
                        }
                        return new { start = 0, sucess ="从【"+from.showValue+"】成功转移到：【"+to.showValue+"】" };
                    }
                    catch (Exception ex)
                    {
                        return new { start = 1, errorMsg = "系统故障"+ex.ToString() };
                    }
                }
            }
        }
        /// <summary>
        /// 获取转移列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public object getFinancialList(userInfo user)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                try

                {
                    user.name = user.name.Replace("_admin","");
                    return   ent.FinancialRecords.Where(o=>o.beginFinancial.Contains(user.name)|| o.endFinancial.Contains(user.name) || o.createMan.Contains(user.name)).ToList();
                }
                catch (Exception)
                {
                    return null ;
                }
            }
        }
        /// <summary>
        /// 流程签字
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public List<WriteModel> getProcessList(WriteModel pro)
        {
            string sql = "";
          
                sql = "select m.name,d.name as 'depatement',l.name as 'leave',c.attitude,c.create_date,c.opinion_type FROM" +
                   "  [dbo].[col_opinion] c left join v3x_org_member m on c.writer_id = m.id " +
                   "left join v3x_org_department d on d.id = m.org_department_id " +
                   "left join v3x_org_level l on l.id = m.org_level_id    " +
                   " where col_id='" + pro.col_id + "' and opinion_type!=2 and attitude=2 order by c.create_date ";
            /*由于旧有逻辑已经可以通过，所以暂时不做修改
             *if (pro.chuangjinshijin < Convert.ToDateTime("2019-01-23"))
             {  sql = "select m.name,d.name as 'depatement',l.name as 'leave',c.attitude,c.create_date,c.opinion_type FROM" +
                    "  [dbo].[col_opinion] c left join v3x_org_member m on c.writer_id = m.id " +
                    "left join v3x_org_department d on d.id = m.org_department_id " +
                    "left join v3x_org_level l on l.id = m.org_level_id    " +
                    " where col_id='" + pro.col_id + "' and opinion_type!=2 and attitude=2 order by c.create_date ";
             }
             else {
                 sql = "select m.name,d.name as 'depatement',l.name as 'leave',c.attitude,c.create_date,c.opinion_type FROM" +
                   "  [dbo].[col_opinion] c left join v3x_org_member m on c.writer_id = m.id " +
                   "left join v3x_affair  affair on affair.id = c.affair_id " +
                   "left join v3x_org_department d on d.id = m.org_department_id " +
                   "left join v3x_org_level l on l.id = m.org_level_id    " +
                   " where col_id='" + pro.col_id + "' and affair.node_policy='approve'  and  opinion_type!=2 and attitude=2 order by c.create_date ";
             }*/
            //opinion_type:0发起人，1，同意的审核人，2，待办 3回退7 超时
            //以上为旧版签字（下为新版签字，由协同改为审批，需要签字的人，）
            DataTable dt = SqlHelper.ExecuteDataTable(config, sql);
            WriteModel result = new WriteModel();
            string[] field = new string[] { "name", "depatement", "leave", "create_date", "attitude", "opinion_type" };
            List<WriteModel> list = ModelHelper.PutAllVal<WriteModel>(result, dt, field);
            return list;
        }
        /// <summary>
        /// 悦肌流程签字（使用审批作为签字依据，排除协同）
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public List<WriteModel> getProcessListNew(WriteModel pro)
        {
            string sql = "";
                 sql = "select m.name,d.name as 'depatement',l.name as 'leave',c.attitude,c.create_date,c.opinion_type FROM" +
                   "  [dbo].[col_opinion] c left join v3x_org_member m on c.writer_id = m.id " +
                   "left join v3x_affair  affair on affair.id = c.affair_id " +
                   "left join v3x_org_department d on d.id = m.org_department_id " +
                   "left join v3x_org_level l on l.id = m.org_level_id    " +
                   " where col_id='" + pro.col_id + "' and affair.node_policy='approve'  and  opinion_type!=2 and attitude=2 order by c.create_date ";
            //opinion_type:0发起人，1，同意的审核人，2，待办 3回退7 超时
            //以上为旧版签字（下为新版签字，由协同改为审批，需要签字的人，）
            DataTable dt = SqlHelper.ExecuteDataTable(config, sql);
           /* WriteModel result = new WriteModel();
            string[] field = new string[] { "name", "depatement", "leave", "create_date", "attitude", "opinion_type" };
            List<WriteModel> list = ModelHelper.PutAllVal<WriteModel>(result, dt, field);
            return list;*/
            return JsonConvert.DeserializeObject<List<WriteModel>>(JsonConvert.SerializeObject(dt));
        }
        /// <summary>
        /// 获取关联的预支单（用于预支单报销）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<YuzhiAttachment> getSunByattachment(ResultListModel model,Condition condition)
        {
            using (var db = new v3xEntities())
            {
                using (var myent = new OAtoU8DATAEntities())
                {
                    var col_id = Convert.ToInt64(model.colId);
                    var colBody = (from c in db.col_body
                                   join a in db.v3x_affair on c.col_id equals a.object_id
                                   join t in db.v3x_attachment on a.id equals t.genesis_id
                                   where t.reference == col_id
                                   select c).ToList();
                    ResultListModel models = new ResultListModel
                    {
                        type = "预支单",//预支单才能冲账
                        list = new List<Detail>()
                    };
                    var yuzhiAttachmentList = new List<YuzhiAttachment>();
                    foreach (var body in colBody)
                    {
                        var yuzhiAttachment = new YuzhiAttachment
                        {
                            Id = body.content,
                            detailList = new List<Detail>(),
                            col_id = body.col_id
                        };
                        models.Id = body.content;//预支单才能冲账
                        models.colId = body.col_id.ToString();
                        var t = getDetail(models,condition);
                        if (t != null)
                        {
                            //model.list.Concat(t.list);//Union去重
                            foreach (var de in t.list)
                            {
                                yuzhiAttachment.detailList.Add(de);
                            }
                        }
                        yuzhiAttachment.record = myent.RecordTable.Where(o => o.Pid.Contains(models.Id)).ToList();
                        yuzhiAttachmentList.Add(yuzhiAttachment);
                    }
                    return yuzhiAttachmentList;
                }
            }
        }
        /// <summary>
        /// 获取关联的预支单
        /// </summary>
        public List<v3x_attachment> getV3x_attachment(ResultListModel model){
             using (var db = new v3xEntities())
            {
                var col_id = Convert.ToInt64(model.colId);
                return (from a in db.v3x_attachment where a.reference == col_id select a).ToList();
            }
        }
        /// <summary>
         /// 获取每个单据的详细金额和说明
         /// </summary>
         /// <param name="model"></param>
         /// <param name="condition"></param>
         /// <returns></returns>
        public ResultListModel getDetail(ResultListModel model,Condition condition)
        {
            ResultListModel result = new ResultListModel();
            string sql = "";
            if (model.type.Equals("差旅费报销单"))
            {
                if (condition.userInfo.company == (int)CompanyEnum.yuemu)
                {
                    condition.formsonName = "formson_0560";//上海悦目
                    condition.formmainId = "formmain_0559Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yueji)
                {
                    condition.formsonName = "formson_0790";//悦肌
                    condition.formmainId = "formmain_0789Id";
                   
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
                {
                    condition.formsonName = "formson_0722";//悦荟
                    condition.formmainId = "formmain_0721Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
                {
                    condition.formsonName = "formson_0740";//广州分公司
                    condition.formmainId = "formmain_0739Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
                {
                    condition.formsonName = "formson_0763";//广州悦妆
                    condition.formmainId = "formmain_0762Id";
                }
                else
                {
                    condition.formsonName = "formson_0732";//bate2
                    condition.formmainId = "formmain_0731Id";
                }
                sql = string.Format("SELECT  '' as kemu,'' as shouyibumen, '' as miaoshu ,'' as xianqing ,field0030 as jine ,field0047 as shijin,field0030 as jinexiaoji ,"+
                    "field0047 as riqi,field0047 as kaishiriqi,field0044 jiezhiriqi ,field0032 as qita ,field0033 as jintie ,field0034 as shineijiaotong , field0035 as chuchaijiaotong ,"+
                   " field0036 as zhusu , field0037 as chechuan , field0038 as jipiao , field0039 as tianshu , field0040 as qizhididian ,field0030 shuihoujine,6 shuidian,field0045 shuie"+
                   "   FROM {0}  where  {1} = {2}  order by sort", condition.formsonName,condition.formmainId, model.Id);         }
            else if (model.type.Equals("预支单"))
            {
                if (condition.userInfo.company == (int)CompanyEnum.yuemu)
                {
                    condition.formsonName = "formson_0594";//上海悦目
                    condition.formmainId = "formmain_0593Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yueji)
                {
                    condition.formsonName = "formson_0788";//悦肌
                    condition.formmainId = "formmain_0787Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
                {
                    condition.formsonName = "formson_0718";//悦荟
                    condition.formmainId = "formmain_0717Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
                {
                    condition.formsonName = "formson_0744";//广州分公司
                    condition.formmainId = "formmain_0743Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
                {
                    condition.formsonName = "formson_0761";//广州悦妆
                    condition.formmainId = "formmain_0760Id";
                }
                else
                {
                    condition.formsonName = "formson_0730";//bate2
                    condition.formmainId = "formmain_0729Id";
                }
                sql = string.Format("SELECT  '' kaishiriqi,'' jiezhiriqi , field0028 as kemu,field0027 as shouyibumen, field0026 as miaoshu "+
                    ",field0029 as xianqing ,field0031 as jine ,'' as shijin,'' as jinexiaoji ,'' as riqi ,'' as qita ,'' as jintie ,'' as shineijiaotong , "+
                    "'' as chuchaijiaotong , '' as zhusu , '' as chechuan , '' as jipiao , '' as tianshu , '' as qizhididian,field0031 shuihoujine, 0 shuidian,"+
                    " 0 shuie FROM {0} f where  {1} = {2}  order by sort", condition.formsonName,condition.formmainId, model.Id);

            }
            else if (model.type.Equals("付款审批单"))
            {
                if (condition.userInfo.company == (int)CompanyEnum.yuemu)
                {
                    condition.formsonName = "formson_0616";//上海悦目
                    condition.formmainId = "formmain_0615Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yueji )
                {
                    condition.formsonName = "formson_0786";//悦肌
                    condition.formmainId = "formmain_0785Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
                {
                    condition.formsonName = "formson_0720";//悦荟
                    condition.formmainId = "formmain_0719Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
                {
                    condition.formsonName = "formson_0742";//广州分公司
                    condition.formmainId = "formmain_0741Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
                {
                    condition.formsonName = "formson_0758";//广州悦妆
                    condition.formmainId = "formmain_0757Id";
                }
                else
                {

                    condition.formsonName = "formson_0728";//bate2
                    condition.formmainId = "formmain_0727Id";
                }
                sql = string.Format("SELECT  '' kaishiriqi,'' jiezhiriqi , field0030 as kemu,field0029 as shouyibumen, field0028 as miaoshu ,field0031 as xianqing ,field0036 as jine ,field0033 as shijin,'' as jinexiaoji ,field0033 as riqi ,'' as qita ," +
                    "'' as jintie ,'' as shineijiaotong , '' as chuchaijiaotong , '' as zhusu , '' as chechuan , '' as jipiao , '' as tianshu , '' as qizhididian,field0032 shuihoujine," +
                    "showValue shuidian, field0034  shuie  FROM {0} left join form_enumvalue on enumvalue=field0035  where   ref_enumid=4866995226575588265 and {1} = {2}  order by sort", condition.formsonName,condition.formmainId, model.Id);
            }
            else if (model.type.Equals("费用报销单"))
            {
                if (condition.userInfo.company == (int)CompanyEnum.yuemu)
                {
                    condition.formsonName = "formson_0564";//上海悦目
                    condition.formmainId = "formmain_0563Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yueji)
                {
                    condition.formsonName = "formson_0564";//悦肌
                    condition.formmainId = "formmain_0563Id";
                    return null;
                }
                else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
                {
                    condition.formsonName = "formson_0724";//悦荟
                    condition.formmainId = "formmain_0723Id";
                }
                else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
                {
                    condition.formsonName = "formson_0738";//广州分公司
                    condition.formmainId = "formmain_0737Id";
                }
                else
                {
                    condition.formsonName = "formson_0734";//bate2
                    condition.formmainId = "formmain_0733Id";
                }
                sql = string.Format("SELECT   '' kaishiriqi,'' jiezhiriqi ,field0030 as kemu,field0029 as shouyibumen, field0028 as miaoshu ,field0031 as xianqing ,field0036 as jine ,field0033 as shijin,'' as jinexiaoji ,field0033 as riqi ,'' as qita ," +
                    "'' as jintie ,'' as shineijiaotong , '' as chuchaijiaotong , '' as zhusu , '' as chechuan , '' as jipiao , '' as tianshu , '' as qizhididian,field0032 shuihoujine," +
                    "showValue shuidian, field0034  shuie  FROM {0} left join form_enumvalue on enumvalue=field0035  where   ref_enumid=4866995226575588265 and {1} = {2}  order by sort", condition.formsonName,condition.formmainId, model.Id);
            }
            else
            {
            }
            if (!string.IsNullOrEmpty(sql))
            {
                DataTable dt = SqlHelper.ExecuteDataTable(config, sql);
                /* Detail detail = new Detail();
                 string[] field = new string[] { "kaishiriqi", "jiezhiriqi", "kemu", "shouyibumen", "jine", "miaoshu", "xianqing", "jinexiaoji", "riqi", "qita", "jintie", "shineijiaotong", "chuchaijiaotong", "zhusu", "chechuan", "jipiao", "tianshu", "qizhididian", "shuihoujine", "shuidian", "shuie" };
                 List<Detail> list = ModelHelper.PutAllVal<Detail>(detail, dt, field);
                 result.list = list;*/
                result.list= JsonConvert.DeserializeObject<List<Detail>>(JsonConvert.SerializeObject(dt));
            }
            return result;
        }
        /// <summary>
        /// 获取付款审批单sql
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        /// <summary>
        /// 获取审批单的sql语句
        /// </summary>
        /// <returns></returns>
        private string getShenpiSqlByCompany(Condition condition) {
            var bumenvalues = "U8部门枚举";
            if (condition.userInfo.company == (int)CompanyEnum.yuemu)
            {
                condition.formmainName = "formmain_0615";//上海悦目
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yueji)
            {
                condition.formmainName = "formmain_0785";//悦肌
                bumenvalues = "广东悦肌U8部门";
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
            {
                condition.formmainName = "formmain_0719";//悦荟
            }
            else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
            {
                    condition.formmainName = "formmain_0741";//广州分公司
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
            {//广州悦妆
                condition.formmainName = "formmain_0757";
                bumenvalues = "悦妆U8部门";
            }
            else
            {//默认测试
                condition.formmainName = "formmain_0727";//bate2
            }
            StringBuilder sb = new StringBuilder();

            sb.Append("select * from (");
            sb.Append("select  '' shouyiren, '" + condition.formmainName + "' tabName,yqpay.thirdVoucher,  field0013 caiwuValue, 'field0013' caiwuCol,chunabianhao, danjubianhao,pingzhenhao,level.name zhiwu, IsIntoBook shifouPingzhen,IsIntoCloseBill shifouShengdan,IsIntoAccvouch shifouZhidan ,outo.contents contentId, CAST(body.content as varchar(100))bodycontent,CAST( formmain.id as varchar(100)) as Id, '付款审批单'[type], col.finish_date finish_date, col.id colId,");
            sb.Append("col.subject title, col.create_date chuangjinshijin, m.id mid, m.name faqiren, d.name faqibumen, enumvalue.showValue shouyibumen,");
            sb.Append("formmain.field0037 beizhu, formmain.field0026 shouyibumenjl,formmain.field0027 shouyibumenzg,");//受益部门负责人和经理只做一个判断，所以就不连表了
            sb.Append("'field0001' liushuiCol,formmain.field0001 liuShui,formmain.field0010  lAmount ,formmain.field0011  cAmount,formmain.field0007 phone,formmain.field0002 company ,");
            sb.Append("'' chuchaishiyou,'' starDate,'' endDate,0 gongjitianshu,0 danjushu, ");
            sb.Append("formmain.field0016 yuzhi,formmain.field0017 yinhuan,");
            sb.Append("formmain.field0018 shoukuandanwei, formmain.field0014 shoukuanyh, formmain.field0015 zhanhao, formmain.field0009 fukuanriqi ");
            sb.Append(",prints.num printNum ");
            sb.Append(",CASE WHEN formmain.field0010 >=100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND (m.name= '张目' or m.name= '黄晓东') ORDER BY create_date DESC )  ");
            sb.Append("WHEN formmain.field0010 < 100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND m.name= '黄山' ORDER BY create_date DESC ) END AS  lastProcessName  ");
            sb.Append("from col_summary col ");
            sb.Append("left join col_body body on col.id = body.col_id ");
            sb.Append("left join v3x_org_member m on m.id = col.start_member_id ");
            sb.Append("left join v3x_org_department d on m.org_department_id = d.id ");
            sb.Append("left join v3x_org_level level on level.id = m.org_level_id  ");
            sb.Append("left join " + condition.formmainName + "  formmain on formmain.start_member_id = col.start_member_id "); //正式formmain_0571 
                                                                                                                                //sb.Append("left join v3x_org_department d1 on d1.id = formmain.field0006 ");
            sb.Append("left join form_enumvalue enumvalue on enumvalue.enumvalue = formmain.field0006 ");
            sb.Append("left join form_enumlist enumlist on enumvalue.ref_enumid = enumlist.id ");
            sb.Append("left join " + IntermediateDb + ".dbo.RecordTable outo on CAST (formmain.id AS VARCHAR ( 100 ))= outo.pid  and outo.isDel is null and outo.chunabianhao like '%付%' ");

            sb.Append("left join " + IntermediateDb + ".dbo.yq_paymentRecord yqpay on formmain.field0001= yqpay.cstInnerFlowNo  and yqpay.isDel is null ");//判断是否添加银行审批记录
            sb.Append("left join " + IntermediateDb + ".dbo.PrintRecord prints on col.id= prints.colid ");
            sb.Append("where "); //正式 col.form_appid = -1166138112544715333
            sb.Append(" col.create_date >= '" + condition.startDate + "' and col.create_date <= '" + condition.endDate + "' ");
            sb.Append("and enumlist.enumname = '"+ bumenvalues + "' ");
            sb.Append("and m.name like '%" + condition.menberName + "%' ");
            sb.Append("and formmain.id like '%" + condition.Id + "%' ");
            //读取银行记录时候查询（通过账户+金额）
            if (!string.IsNullOrEmpty(condition.AcctNo))
            {
                sb.Append("and  replace(replace(replace(formmain.field0015,'    ',''),'-',''),' ','')= '" + condition.AcctNo + "'");
            }
            if (condition.money>0)
            {
                sb.Append("and formmain.field0017 ='" + condition.money + "' ");//金额
            }
            if (condition.userInfo.type == 1)
            {//财务操作
                if (condition.form_enumvalue == null)
                {
                    return null;
                }
                sb.Append("and formmain.field0013 = '" + condition.form_enumvalue.enumvalue + "'");
            }
            sb.Append("and formmain.field0001 like '%" + condition.liuShui + "%'");
            // sb.Append("and formmain.finishedflag = " + condition.start);
            sb.Append(") as a  where a.Id = '' + a.bodycontent ");
           return sb.ToString();
        }
        /// <summary>
        /// 费用报销单Sql
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private string getFeiyongSqlByCompany(Condition condition) {
            if (condition.userInfo.company == (int)CompanyEnum.yuemu)
            {
                condition.formmainName = "formmain_0563";//上海悦目
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yueji)
            {
                return "";
                //condition.formmainName = "formmain_0563";//悦肌
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
            {
                return "";
                //condition.formmainName = "formmain_0723";//悦荟
            }
            else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
            {
                return "";
                //condition.formmainName = "formmain_0737";//广州分公司
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
            {//广州悦妆
                return "";
            }
            else
            {
                condition.formmainName = "formmain_0733";//bate2
                return "";
            }
            StringBuilder sb = new StringBuilder();

            sb.Append("select  * from (");
            sb.Append("select '' shouyiren, '" + condition.formmainName + "' tabName,yqpay.thirdVoucher,  field0013 caiwuValue, 'field0013' caiwuCol, chunabianhao, danjubianhao,pingzhenhao,level.name zhiwu, IsIntoBook shifouPingzhen,IsIntoCloseBill shifouShengdan,IsIntoAccvouch shifouZhidan ,outo.contents contentId, CAST(body.content as varchar(100))bodycontent,CAST( formmain.id as varchar(100)) as Id, '费用报销单'[type], col.finish_date finish_date, col.id colId,");
            sb.Append("col.subject title, col.create_date chuangjinshijin, m.id mid, m.name faqiren, d.name faqibumen, enumvalue.showValue shouyibumen,");
            sb.Append("formmain.field0037 beizhu, formmain.field0026 shouyibumenjl,formmain.field0027 shouyibumenzg,");//受益部门负责人和经理只做一个判断，所以就不连表了
            sb.Append("'field0001' liushuiCol,formmain.field0001 liuShui,formmain.field0010  lAmount ,formmain.field0011  cAmount,formmain.field0007 phone,formmain.field0002 company ,");
            sb.Append("'' chuchaishiyou,'' starDate,'' endDate,0 gongjitianshu,0 danjushu,");
            sb.Append("formmain.field0016 yuzhi,formmain.field0017 yinhuan,");
            sb.Append("formmain.field0018 shoukuandanwei, formmain.field0014 shoukuanyh, formmain.field0015 zhanhao, formmain.field0009 fukuanriqi ");
            sb.Append(",prints.num printNum ");
            sb.Append(",CASE WHEN formmain.field0010 >=100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND (m.name= '张目' or m.name= '黄晓东') ORDER BY create_date DESC )  ");
            sb.Append("WHEN formmain.field0010 < 100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND m.name= '黄山' ORDER BY create_date DESC ) END AS  lastProcessName  ");
            sb.Append("from col_summary col ");
            sb.Append("left join col_body body on col.id = body.col_id ");
            sb.Append("left join v3x_org_member m on m.id = col.start_member_id ");
            sb.Append("left join v3x_org_department d on m.org_department_id = d.id ");
            sb.Append("left join v3x_org_level level on level.id = m.org_level_id  ");
            sb.Append("left join " + condition.formmainName + "  formmain on formmain.start_member_id = col.start_member_id ");
                                                                                                                               //sb.Append("left join v3x_org_department d1 on d1.id = formmain.field0006 ");
            sb.Append("left join form_enumvalue enumvalue on enumvalue.enumvalue = formmain.field0006 ");
            sb.Append("left join form_enumlist enumlist on enumvalue.ref_enumid = enumlist.id ");
            sb.Append("left join " + IntermediateDb + ".dbo.RecordTable outo on CAST (formmain.id AS VARCHAR ( 100 ))= outo.pid and outo.isDel is null and outo.chunabianhao like '%付%' ");

            sb.Append("left join " + IntermediateDb + ".dbo.yq_paymentRecord yqpay on formmain.field0001= yqpay.cstInnerFlowNo and yqpay.isDel is null ");//判断是否添加银行审批记录
            sb.Append("left join " + IntermediateDb + ".dbo.PrintRecord prints on col.id= prints.colid ");
            sb.Append("where"); //正式表单col.form_appid = 6030205033455177463 
            sb.Append("  col.create_date >= '" + condition.startDate + "' and col.create_date <= '" + condition.endDate + "' ");
            sb.Append("and enumlist.enumname = 'U8部门枚举' ");
            sb.Append("and m.name like '%" + condition.menberName + "%' ");
            sb.Append("and formmain.id like '%" + condition.Id + "%' ");
            if (condition.userInfo.type == 1)
            {//财务操作
                if (condition.form_enumvalue == null)
                {
                    return null;
                }
                sb.Append("and formmain.field0013 = '" + condition.form_enumvalue.enumvalue + "'");
            }
            sb.Append("and m.name like '%" + condition.menberName + "%' ");
            sb.Append("and formmain.field0001 like '%" + condition.liuShui + "%'");
            // sb.Append("and formmain.finishedflag = " + condition.start);
            sb.Append(") as a  where a.Id = '' + a.bodycontent ");
            if (!string.IsNullOrEmpty(condition.AcctNo))
            {
                sb.Append("and  replace(replace(replace(formmain.field0015,'    ',''),'-',''),' ','')= '" + condition.AcctNo + "'");
            }
            if (condition.money > 0)
            {
                sb.Append("and formmain.field0017 ='" + condition.money + "' ");//金额
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取预支单Sql
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private string getYuzhiSqlByCompany(Condition condition)
        {
            var bumenvalues = "U8部门枚举";
            if (condition.userInfo.company == (int)CompanyEnum.yuemu)
            {
                condition.formmainName = "formmain_0593";//上海悦目
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yueji)
            {
                condition.formmainName = "formmain_0787";//悦肌 
                bumenvalues = "广东悦肌U8部门";
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
            {
                condition.formmainName = "formmain_0717";//悦荟
            }
            else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
            {
                condition.formmainName = "formmain_0743";//广州分公司
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
            {//广州悦妆
                condition.formmainName = "formmain_0760";
                bumenvalues = "悦妆U8部门";
            }
            else
            {
                condition.formmainName = "formmain_0729";//Bate2
            }
            StringBuilder sb = new StringBuilder();

            sb.Append("select * from (");
            sb.Append("select   '' shouyiren,'" + condition.formmainName + "' tabName,yqpay.thirdVoucher, field0018 caiwuValue, 'field0018' caiwuCol, chunabianhao, danjubianhao,pingzhenhao,level.name zhiwu, IsIntoBook shifouPingzhen,IsIntoCloseBill shifouShengdan,IsIntoAccvouch shifouZhidan ,outo.contents contentId, CAST(body.content as varchar(100)) bodycontent,CAST( formmain.id as varchar(100)) as Id, '预支单'[type], col.finish_date finish_date, col.id colId,");
            sb.Append("col.subject title, col.create_date chuangjinshijin, m.id mid, m.name faqiren, d.name faqibumen, enumvalue.showValue shouyibumen,");
            sb.Append("formmain.field0035 beizhu, formmain.field0022 shouyibumenjl,formmain.field0023 shouyibumenzg,");//受益部门负责人和经理只做一个判断，所以就不连表了
            sb.Append("'field0001' liushuiCol,formmain.field0001 liuShui,formmain.field0010  lAmount ,formmain.field0011  cAmount,formmain.field0007 phone,formmain.field0002 company , ");
            sb.Append("'' chuchaishiyou,'' starDate,'' endDate,0 gongjitianshu,0 danjushu,");
            sb.Append("'0.00' yuzhi,'0.00' yinhuan,");
            sb.Append("formmain.field0021 shoukuandanwei,formmain.field0019 shoukuanyh,formmain.field0020 zhanhao,formmain.field0009 fukuanriqi  ");
            sb.Append(",prints.num printNum ");
            sb.Append(",CASE WHEN formmain.field0010 >=100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND (m.name= '张目' or m.name= '黄晓东') ORDER BY create_date DESC )  ");
            sb.Append("WHEN formmain.field0010 < 100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND m.name= '黄山' ORDER BY create_date DESC ) END AS  lastProcessName  ");
            sb.Append("from col_summary col ");
            sb.Append("left join col_body body on col.id = body.col_id ");
            sb.Append("left join v3x_org_member m on m.id = col.start_member_id ");
            sb.Append("left join v3x_org_department d on m.org_department_id = d.id ");
            sb.Append("left join v3x_org_level level on level.id = m.org_level_id  ");
            sb.Append("left join " + condition.formmainName + "  formmain on formmain.start_member_id = col.start_member_id ");//formmain_0567正式
                                                                                                                               //sb.Append("left join v3x_org_department d1 on d1.id = formmain.field0006 ");
            sb.Append("left join form_enumvalue enumvalue on enumvalue.enumvalue = formmain.field0006 ");
            sb.Append("left join form_enumlist enumlist on enumvalue.ref_enumid = enumlist.id ");
            sb.Append("left join " + IntermediateDb + ".dbo.RecordTable outo on CAST (formmain.id AS VARCHAR ( 100 ))= outo.pid  and outo.isDel is null and outo.chunabianhao like '%付%' ");
            sb.Append("left join " + IntermediateDb + ".dbo.yq_paymentRecord yqpay on formmain.field0001= yqpay.cstInnerFlowNo  and yqpay.isDel is null ");//判断是否添加银行审批记录
            sb.Append("left join " + IntermediateDb + ".dbo.PrintRecord prints on col.id= prints.colid ");
            sb.Append("where ");//正式col.form_appid = 7918552077698879263
            sb.Append(" col.create_date >= '" + condition.startDate + "' and col.create_date <= '" + condition.endDate + "'");
            sb.Append("and enumlist.enumname = '"+ bumenvalues + "' ");
            sb.Append("and m.name like '%" + condition.menberName + "%'");
            sb.Append("and formmain.id like '%" + condition.Id + "%'");
            if (!string.IsNullOrEmpty(condition.AcctNo))
            {
                sb.Append("and  replace(replace(replace(formmain.field0020,'    ',''),'-',''),' ','')= '" + condition.AcctNo + "'");
            }
            if (condition.money > 0)
            {
                sb.Append("and formmain.field0010 ='" + condition.money + "' ");//金额
            }
            if (condition.userInfo.type == 1)
            {//财务操作
                if (condition.form_enumvalue == null)
                {
                    return null;
                }
                sb.Append("and formmain.field0018= '" + condition.form_enumvalue.enumvalue + "'");
            }
            //  sb.Append("and formmain.finishedflag = " + condition.start);
            sb.Append("and formmain.field0001 like '%" + condition.liuShui + "%'");
            sb.Append(") as a  where a.Id = '' + a.bodycontent ");
            return sb.ToString();
        }
        /// <summary>
        /// 差旅费sql
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private string getChailvSqlByCompany(Condition condition)
        {
            var bumenvalues = "U8部门枚举";
            if (condition.userInfo.company == (int)CompanyEnum.yuemu)
            {
                condition.formmainName = "formmain_0559";//上海悦目
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yueji)
            {
                condition.formmainName = "formmain_0789";//悦肌
                bumenvalues = "广东悦肌U8部门";
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuehui)
            {
                condition.formmainName = "formmain_0721";//悦荟
            }
            else if (condition.userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
            {
                condition.formmainName = "formmain_0739";//广州分公司
            }
            else if (condition.userInfo.company == (int)CompanyEnum.yuezhuang)
            {//广州悦妆
                condition.formmainName = "formmain_0762";
                bumenvalues = "悦妆U8部门";
            }
            else
            {
                condition.formmainName = "formmain_0731";//默认测试
            }
            /*if (condition.userInfo.type == 1)
            {//财务操作
                if (condition.form_enumvalue == null || condition.form_enumvalue.enumvalue.IndexOf("费用") == -1)
                {
                    return null;
                }
            }*/
            StringBuilder sb = new StringBuilder();
            sb.Append("select * from (");
            sb.Append(" select   m.name shouyiren,'" + condition.formmainName + "' tabName,yqpay.thirdVoucher,  '7'  caiwuValue, '' caiwuCol ,chunabianhao, danjubianhao,pingzhenhao,level.name zhiwu,IsIntoBook shifouPingzhen,outo.contents contentId,IsIntoCloseBill shifouShengdan,IsIntoAccvouch shifouZhidan , CAST(body.content as varchar(100))bodycontent,CAST( formmain.id as varchar(100)) as Id, '差旅费报销单'[type], col.finish_date finish_date, col.id colId,");
            sb.Append("col.subject title, col.create_date chuangjinshijin, m.id mid, mf.name faqiren, d.name faqibumen, enumvalue.showValue shouyibumen,");

            sb.Append("formmain.field0043 beizhu, formmain.field0026 shouyibumenjl,formmain.field0027 shouyibumenzg,");//受益部门负责人和经理只做一个判断，所以就不连表了
            sb.Append("'field0021' liushuiCol,formmain.field0021 liuShui,formmain.field0012  lAmount, formmain.field0013  cAmount, '' phone, formmain.field0001 company,");
            sb.Append("formmain.field0006 chuchaishiyou, formmain.field0014 starDate, formmain.field0015 endDate, formmain.field0016 gongjitianshu,");
            sb.Append("formmain.field0018 yuzhi, formmain.field0019 yinhuan,formmain.field0017 danjushu,");
            sb.Append("formmain.field0022 shoukuandanwei, formmain.field0023 shoukuanyh, formmain.field0024 zhanhao, formmain.field0025 fukuanriqi  ");
            sb.Append(",prints.num printNum ");
            sb.Append(",CASE WHEN formmain.field0012 >=100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND (m.name= '张目' or m.name= '黄晓东') ORDER BY create_date DESC )  ");
            sb.Append("WHEN formmain.field0012 < 100000 THEN(SELECT TOP 1 m.name  FROM col_opinion c LEFT JOIN v3x_org_member m ON c.writer_id = m.id WHERE c.col_id = col.id   AND m.name= '黄山' ORDER BY create_date DESC ) END AS  lastProcessName  ");
            sb.Append("from col_summary col ");
            sb.Append("left join col_body body on col.id = body.col_id ");
            sb.Append("left join " + condition.formmainName + " formmain on formmain.start_member_id = col.start_member_id ");//正式的资源表formmain_0559 
            sb.Append("left join v3x_org_member m on m.id = formmain.field0002 ");//归属人
            sb.Append("left join v3x_org_member mf on mf.id = col.start_member_id ");//发起人
            sb.Append("left join v3x_org_department d on m.org_department_id = d.id ");
            sb.Append("left join v3x_org_level level on level.id = m.org_level_id  ");
            sb.Append("left join form_enumvalue enumvalue on enumvalue.enumvalue = formmain.field0005 ");
            sb.Append("left join form_enumlist enumlist on enumvalue.ref_enumid = enumlist.id ");
            sb.Append("left join " + IntermediateDb + ".dbo.RecordTable outo on CAST (formmain.id AS VARCHAR ( 100 ))= outo.pid  and outo.isDel is null and outo.chunabianhao like '%付%' ");
            sb.Append("left join " + IntermediateDb + ".dbo.yq_paymentRecord yqpay on formmain.field0021= yqpay.cstInnerFlowNo  and yqpay.isDel is null ");//判断是否添加银行审批记录
            sb.Append("left join " + IntermediateDb + ".dbo.PrintRecord prints on col.id= prints.colid ");
            //  sb.Append("left join v3x_org_department d1 on d1.id = formmain.field0005 ");
            sb.Append("where ");//col.form_appid = -2287409120319815238 （正式的模板id）
            sb.Append(" col.create_date >= '" + condition.startDate + "' and col.create_date <= '" + condition.endDate + "' ");
            sb.Append("and enumlist.enumname = '"+ bumenvalues + "' ");
            if (!string.IsNullOrEmpty(condition.menberName))
            {
                sb.Append("and (m.name like '%" + condition.menberName + "%' ");
                sb.Append("or mf.name like '%" + condition.menberName + "%') ");
            }
            if (!string.IsNullOrEmpty(condition.AcctNo))
            {
                sb.Append("and  replace(replace(replace(formmain.field0024,'    ',''),'-',''),' ','')= '" + condition.AcctNo + "'");
            }
            if (condition.money > 0)
            {
                sb.Append("and formmain.field0019 ='" + condition.money + "' ");//金额
            }
            sb.Append("and formmain.id like '%" + condition.Id + "%' ");
            sb.Append("and formmain.field0021 like '%" + condition.liuShui + "%'");
            // sb.Append("and formmain.finishedflag = " + condition.start);由于流程出现变化，当流程跑到黄山或者金额大于1000000时候跑到总经理的时候就能打印，再考虑到有些流程比较紧急所以等后续补签提前打印，所以不再设置在完成时候才能打印
            sb.Append(") as a  where a.Id = '' + a.bodycontent ");
             if (condition.userInfo.type == 1)
              {//财务操作
               /*if (condition.form_enumvalue == null || condition.form_enumvalue.showValue.IndexOf("费用") == -1)
               {
                   sb.Clear();
               }*/
                if (!condition.form_enumvalue.enumvalue.Equals("7") && condition.userInfo.company != (int)CompanyEnum.yueji)
                {//7为胡姗的财务枚举编号，此类单据只过胡姗
                    sb.Clear();
                }
              
             }
            return sb.ToString();
        }
        /// <summary>
        /// 费用报销单主表
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<ResultListModel> getList(Condition condition)
        {
            try
            {
                if (condition.startDate == null)
                {
                    condition.startDate = new DateTime(1990, 01, 01);
                }
                if (condition.endDate == null)
                {
                    condition.endDate = DateTime.Now.AddDays(1);
                }
                else
                {
                    DateTime dt = Convert.ToDateTime(condition.endDate);
                    condition.endDate = dt.AddDays(1);
                }
                if (string.IsNullOrEmpty(condition.menberName))
                {
                    condition.menberName = "";
                }
                if (string.IsNullOrEmpty(condition.Id))
                {
                    condition.Id = "";
                }
                if (string.IsNullOrEmpty(condition.liuShui))
                {//根据流水号查询
                    condition.liuShui = "";
                }
                if (condition.userInfo.type == 2)
                {//普通员工
                    condition.menberName = condition.userInfo.name;
                }
                //只查询Form内容
                string sql = "";
                if(string.IsNullOrEmpty(condition.type))
                {
                    sql = getShenpiSqlByCompany(condition);
                    /* sql += " union all ";
                     sql += getFeiyongSqlByCompany(condition);*/
                    sql += " union all ";
                    sql += getYuzhiSqlByCompany(condition);
                    sql += " union all ";
                    sql += getChailvSqlByCompany(condition);
                }
                else if (condition.type.IndexOf("差旅费报销单") > -1)
                {
                    sql = getChailvSqlByCompany(condition);
                }
                else if (condition.type.IndexOf("预支单") > -1)
                {
                    sql = getYuzhiSqlByCompany(condition);
                }
                else if (condition.type.IndexOf("费用报销单") > -1)
                {
                    sql = getFeiyongSqlByCompany(condition);
                }
                else if (condition.type.IndexOf("付款审批单") > -1)
                {
                    sql = getShenpiSqlByCompany(condition);
                }
                else
                {
                    sql = getShenpiSqlByCompany(condition);
                   /* sql += " union all ";
                    sql += getFeiyongSqlByCompany(condition);*/
                    sql += " union all ";
                    sql += getYuzhiSqlByCompany(condition);
                    sql += " union all ";
                    sql += getChailvSqlByCompany(condition);
                }
                if (!string.IsNullOrEmpty(sql))
                {
                    DataTable dt = SqlHelper.ExecuteDataTable(config,sql);
                    var json = JsonConvert.SerializeObject(dt);
                    return JsonConvert.DeserializeObject<List<ResultListModel>>(json) ;
                    /* DataSet ds = SqlHelper.ExecuteDataSet(config, sql);
                     ResultListModel s = new ResultListModel();
                     string[] field = new string[] { "shouyiren", "danjushu", "liushuiCol", "caiwuValue", "caiwuCol", "tabName","chunabianhao", "danjubianhao", "pingzhenhao", "zhiwu", "liuShui", "shifouPingzhen", "contentId", "shifouShengdan", "shifouZhidan", "feiyongleixing", "chuchaishiyou", "gongjitianshu", "starDate", "endDate", "yuzhi", "yinhuan", "shoukuandanwei", "shoukuanyh", "zhanhao", "fukuanriqi", "title", "chuangjinshijin", "faqiren", "faqibumen", "colId", "finish_date", "Id", "company", "phone", "lAmount", "cAmount", "shouyibumen", "type", "beizhu", "shouyibumenjl", "shouyibumenzg", "printNum", "lastProcessName" };

                     var resultListModel = ModelHelper.PutAllVal<ResultListModel>(s, ds, field);

                     if (condition.userInfo.company ==(int) CompanyEnum.ceshi)
                     {//测试的啊好烦，配合演出只能模拟 
                         foreach (var item in resultListModel)
                         {
                             var sss = getProcessList(new WriteModel() { col_id = item.colId });
                             if (sss.Count != 0)
                             {
                                 //有签字说明结束了
                                 item.lastProcessName = "黄山";
                             }

                         }
                     }
                     return resultListModel;*/
                }
                else
                {
                    return new List<ResultListModel>();
                }

            }
            catch (Exception ex)
            {

                return new List<ResultListModel>();
            }
        }
    }
}
