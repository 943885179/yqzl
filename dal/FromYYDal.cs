using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using common;
using EntityFromework;
using Log;
using model;
using Model;

namespace dal
{
    /// <summary>
    /// u8公共数据获取
    /// </summary>
    public class FromYYDal
    {
        private DateTime date = DateTime.Now;
        private int year = DateTime.Now.Year;
        private  string u8Db= "UFDATA_046_2018Entities";
        /// <summary>
        /// 重新构造，根据登录用户的公司来读取相应的数据库
        /// </summary>
        /// <param name="userInfo"></param>
        public FromYYDal(userInfo  userInfo){
            if (userInfo.company==(int)CompanyEnum.yuemu)
            {
                /* year = 2018;
                  this.u8Db = "UFDATA_114_2018Entities";*/
                this.u8Db = "UFDATA_046_2018Entities";
                year = DateTime.Now.Year;
            }
            if (userInfo.company == (int)CompanyEnum.yueji)
            {
                this.u8Db = "UFDATA_048_2018Entities";
                year = DateTime.Now.Year; ;
                
            }
            if (userInfo.company == (int)CompanyEnum.yuezhuang)
            {
                this.u8Db = "UFDATA_047_2018Entities"; year = DateTime.Now.Year ;
            }
            if (userInfo.company == (int)CompanyEnum.yuehui)
            {
                this.u8Db = "UFDATA_050_2018Entities"; year = DateTime.Now.Year; 
            }
            if (userInfo.company == (int)CompanyEnum.guangzhoufengongsi)
            {
                this.u8Db = "UFDATA_049_2017Entities"; year = 2018;
            }
            if (userInfo.company == (int)CompanyEnum.ceshi)
            {
                year = 2018;
                this.u8Db = "UFDATA_114_2018Entities";
               // this.u8Db = "UFDATA_114_2018Entities"; year = DateTime.Now.Year; 
            }
        }
            // private static string config = ConfigurationManager.AppSettings["U8slqserver"].ToString();
            /// <summary>
            /// 获取付款银行
            /// </summary>
            public List<Content> getAccInfo(int acctType)
           {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                //第一次加载太慢了，增加个暖机操作试试水
                /*  var objectContext = ((IObjectContextAdapter)db).ObjectContext;
                  var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
                  mappingCollection.GenerateViews(new List<EdmSchemaError>());*/
                try
                {
                       var request = (from c in db.CN_AcctInfo
                                       join code in db.code on  c.SubjectCode equals code.ccode
                                       where c.AcctType == acctType && c.lYear == year && c.IsUsed==1 && code.iyear==year
                                       select new Content
                                       {
                                           ID= c.ID,
                                           AcctName= c.AcctName,
                                           SubjectCode=  c.SubjectCode,
                                           ccode_name=code.ccode_name,
                                           BankAcct=c.BankAcct,
                                           BankName=c.BankName,
                                           City=c.City,
                                           Province=c.Province,
                                           UnitName=c.UnitName
                                       }).ToList();
                    if (u8Db == "UFDATA_049_2017Entities")
                    {//049未通知同步
                        request = (from c in db.CN_AcctInfo
                                       join code in db.code on c.SubjectCode equals code.ccode
                                       where c.AcctType == acctType && c.lYear == 2019 && c.IsUsed == 1 && code.iyear == 2018
                                    select new Content
                                       {
                                           ID = c.ID,
                                           AcctName = c.AcctName,
                                           SubjectCode = c.SubjectCode,
                                           ccode_name = code.ccode_name,
                                           BankAcct = c.BankAcct,
                                           BankName = c.BankName
                                       }).ToList();
                    }
                        return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);

                    return null;
                }
            }
        }
        /// <summary>
        /// 获取现金交易还是转账交易
        /// </summary>
        /// <returns></returns>
        public object getSettleStyle()
        {
            using (var db = new UFDATA_048_2017Entities(u8Db)) {

                return (from a in db.SettleStyle
                        orderby a.cSSCode descending
                       select new {
                            a.cSSCode,
                            a.cSSName
                       } ).ToList();
            }
            /*
            string sql = "select * from SettleStyle";
            DataTable dt = SqlHelper.ExecuteDataTable(config, sql);
            List<SettleStyleModel> list = ModelHelper.PutAllVal<SettleStyleModel>(new SettleStyleModel(), dt);
            return list;*/
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <param name="ccode_name">科目描述（根据OA归类大纲来）</param>
        /// <returns></returns>
        public code getCode(string ccode_name)
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                var codes = (from c in db.code where c.iyear == year && c.ccode_name == ccode_name select c).FirstOrDefault();
                return codes;
            }
        }

        /// <summary>
        /// 获取u8单个科目明细
        /// </summary>
        /// <param name="ccode"></param>
        /// <returns></returns>
        public code getCodeInU8Byccode(string ccode)
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                return (from c in db.code where c.iyear == year && c.ccode.Equals(ccode) select c).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取常用的科目便于操作人员修改
        /// </summary>
        /// <returns></returns>
        public List<code> getCodeList()
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                var codes = (from c in db.code where c.cclass.Equals("损益") && c.bend == true select c).ToList();
                return codes;
            }
        }
        /// <summary>
        /// 获取所有正在使用的科目
        /// </summary>
        /// <returns></returns>
        public List<code> getCodeAllList()
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                var codes = (from c in db.code where c.bend == true select c).ToList();
                return codes;
            }
        }
        /// <summary>
        /// 获取流量科目
        /// </summary>
        /// <param name="citemname"></param>
        /// <returns></returns>
        public fitemss98 getProject(string citemname)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                return (from f in db.fitemss98 where f.citemname.Equals(citemname) select f).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取流量科目
        /// </summary>
        /// <param name="citemcode"></param>
        /// <returns></returns>
        public fitemss98 getProjectBycode(string citemcode)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                return (from f in db.fitemss98 where f.citemcode.Equals(citemcode) select f).FirstOrDefault();
            }
        }
        /// <summary>
        /// 获取科目
        /// </summary>
        /// <returns></returns>
        public List<fitemss98> getProject()
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                return db.fitemss98.ToList();
            }
        }
        /// <summary>
        /// 获取部门List
        /// </summary>
        /// <returns></returns>
        public object getDepatementList()
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from d in db.Department
                                   where d.dDepEndDate==null || d.dDepEndDate>DateTime.Now
                                   select new
                                   {
                                       d.cDepName,
                                       d.cDepCode
                                   }).ToList();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }

        }
        /// <summary>
        /// 如何已经生成付款单则不允许再修改付款银行
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public RecordTable getRecordTableByPid(string pid)
        {
            using (var ent = new OAtoU8DATAEntities())
            {
                var list = (from r in ent.RecordTable
                            where r.Pid.Equals(pid)
                            select r).FirstOrDefault();

                return list;
            }
        }
        /// <summary>
        /// 获取U8发起部门的编码 CN_LevelListID（部门和oa有差异导致有些部门取不到，所以要调整公司架构）
        /// </summary>
        /// <param name="departementName"></param>
        /// <returns></returns>
        public int getDepatement(string departementName)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from c in db.CN_LevelListID
                                   join d in db.Department on c.LoadID equals d.cDepCode
                                   where d.cDepName.Contains(departementName) && c.ClassID == 2 && d.dDepEndDate==null
                                   select c).FirstOrDefault();
                    return request.ID;
                }
                catch (Exception)
                {
                   // LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return 0;
                }
            }
        }
        /// <summary>
        /// 获取部门 Department
        /// </summary>
        /// <param name="departementName"></param>
        /// <returns></returns>
        public Department getDepatements(string departementName)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    //部门一般都是xxx-xxx，只取最后一个
                    string[] dirName = departementName.Split('-');
                    departementName = dirName[dirName.Length - 1];
                     var request = (from c in db.CN_LevelListID
                                   join d in db.Department on c.LoadID equals d.cDepCode
                                   where d.cDepName.Contains(departementName) && (d.dDepEndDate==null ||d.dDepEndDate>=DateTime.Now)  
                                    select d).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 根据部门编号获取部门
        /// </summary>
        /// <param name="cDepCode"></param>
        /// <returns></returns>
        public Department getDepatementByDepCode(string cDepCode)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
               
                    return  (from c in db.CN_LevelListID
                                   join d in db.Department on c.LoadID equals d.cDepCode
                                   where d.cDepCode.Contains(cDepCode)
                                   select d).FirstOrDefault();
               
            }
        }
        /// <summary>
        /// 获取供应商的UnitID，但是呢由于OA没有此表，顾后续打算放入部门表中，根据部门名称来作为查询(供应商)
        /// </summary>
        /// <param name="shoukuandanwei"></param>
        /// <returns></returns>
        public CN_UnitID GeVendortUnit(string shoukuandanwei)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from v in db.Vendor
                                   join c in db.CN_UnitID on v.cVenCode equals c.LoadVenID
                                   where v.cVenName.Equals(shoukuandanwei)
                                   select c).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取供应商的UnitID，但是呢由于OA没有此表，顾后续打算放入部门表中，根据部门名称来作为查询(供应商)
        /// </summary>
        /// <param name="shoukuandanwei"></param>
        /// <returns></returns>
        public CustomertModel GeVendortUnit_getDep(string shoukuandanwei)
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from v in db.Vendor
                                   join c in db.CN_UnitID on v.cVenCode equals c.LoadVenID
                                   join d in db.Department on v.cVenDepart equals d.cDepCode
                                   where v.cVenName.Equals(shoukuandanwei)
                                   select new CustomertModel {
                                       cCusName=v.cVenName,
                                       cDepName=d.cDepName
                                   }).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取供应商列表
        /// </summary>
        /// <param name="shoukuandanwei"></param>
        /// <returns></returns>
        public List<CustomertModel> GeVendortUnit_getDepList()
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from v in db.Vendor
                                   join c in db.CN_UnitID on v.cVenCode equals c.LoadVenID
                                   join d in db.Department on v.cVenDepart equals d.cDepCode
                                   select new CustomertModel
                                   {
                                       cCusName = v.cVenName,
                                       cDepName = d.cDepName,
                                       cCusCode=v.cVenCode,
                                       cDepCode=d.cDepCode,
                                       UnitType=2,
                                       UnitTypeName="供应商"
                                   }).ToList();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 根据员工名称获取员工部门信息
        /// </summary>
        /// <param name="personName"></param>
        /// <returns></returns>
        public Department DepartmentByPerson(string personName)
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from d in db.Department
                                   join p in db.Person on d.cDepCode equals p.cDepCode
                                   where p.cPersonName.Equals(personName)
                                   select d).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 员工部门信息
        /// </summary>
        /// <returns></returns>
        public List<CustomertModel> DepartmentByPersonList()
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from d in db.Department
                                   join p in db.Person on d.cDepCode equals p.cDepCode
                                   select new CustomertModel {
                                       cDepCode = d.cDepCode,
                                       cDepName = d.cDepName,
                                       cCusCode = p.cPersonCode,
                                       cCusName = p.cPersonName,
                                       UnitType = 0,
                                       UnitTypeName = ""//员工
                                   }).ToList();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取供应商的UnitID，但是呢由于OA没有此表，顾后续打算放入部门表中，根据部门名称来作为查询（客户）
        /// </summary>
        /// <param name="shoukuandanwei"></param>
        /// <returns></returns>
        public CN_UnitID geCustomertUnit(string shoukuandanwei)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from v in db.Customer
                                   join c in db.CN_UnitID on v.cCusCode equals c.LoadCusID
                                   where v.cCusName.Equals(shoukuandanwei)
                                   select c).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取客户信息，部门+客户
        /// </summary>
        /// <param name="shoukuandanwei"></param>
        /// <returns></returns>
        public CustomertModel geCustomertUnit_getDep(string shoukuandanwei)
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from v in db.Customer
                                   join c in db.CN_UnitID on v.cCusCode equals c.LoadCusID
                                   join d in db.Department on v.cCCCode equals d.cDepCode
                                   where v.cCusName.Equals(shoukuandanwei)
                                   select new CustomertModel {
                                      cCusName= v.cCusName,
                                      cDepName=d.cDepName
                                   }).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取客户信息列表，部门+客户
        /// </summary>
        /// <returns></returns>
        public List<CustomertModel> geCustomertUnit_getDepList()
        {
            using (var db = new UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from v in db.Customer
                                   join c in db.CN_UnitID on v.cCusCode equals c.LoadCusID
                                   join d in db.Department on v.cCCCode equals d.cDepCode
                                   join l in db.CN_LevelListID  on d.cDepCode equals l.LoadID
                                   // where v.cCusName.Equals(shoukuandanwei)
                                   select new CustomertModel
                                   {
                                       cCusName = v.cCusName,
                                       cDepName = d.cDepName,
                                       cCusCode=v.cCusCode,
                                       cDepCode=d.cDepCode,
                                       UnitType = 1,//UnitType = 2;//客户为1，供应商为2
                                       UnitTypeName = "客户"
                                   }).ToList();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取收付出纳编号（收xxxx或者付xxxx）
        /// </summary>
        /// <param name="content"></param>
        /// <param name="shoufu">输入收/付</param>
        /// <returns></returns>
        public string getCashSerialNumber(string content, string shoufu)
        {
            //截取银行的前面数字内容
            //string result = System.Text.RegularExpressions.Regex.Replace(content, @"[^0-9]+", "");
            string result = content.Substring(0, 5);
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from c in db.CN_CashSerialNumber
                                   where c.SNWord.Contains(result) && c.SNWord.Contains(shoufu)
                                   select c).FirstOrDefault();
                    return request.SNWord;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return "010101付";
                }
            }
        }
        /// <summary>
        /// duiqudiyigeshoufukemu
        /// </summary>
        /// <param name="shoufu"></param>
        /// <returns></returns>
        public string getCashSerialNumber(string shoufu)
        {
            //截取银行的前面数字内容
            //string result = System.Text.RegularExpressions.Regex.Replace(content, @"[^0-9]+", "");
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    var request = (from c in db.CN_CashSerialNumber
                                   where c.SNWord.Contains(shoufu)
                                   select c).FirstOrDefault();
                    return request.SNWord;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取U8人员信息
        /// </summary>
        /// <returns></returns>
        public Person getPerson(string userName)
        {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                { 
                    /*var request = (from c in db.hr_hi_person
                                   where c.cPsn_Name.Equals(userName)
                                   select c).FirstOrDefault();*/
                    var request = (from c in db.Person
                                   where c.cPersonName.Equals(userName)
                                   select c).FirstOrDefault();
                    return request;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取收款单位1：客户 2：供应商，3：个人，完全匹配，不支持模糊
        /// </summary>
        /// <param name="shoukuandanwei">收款单位（全称）</param>
        /// <returns></returns>
        public object getUnitType(string shoukuandanwei) {
            var kehu = geCustomertUnit(shoukuandanwei);//客户
            var gonyingshang = GeVendortUnit(shoukuandanwei);//供应商
            var UnitType = 1;//客户为1，供应商为2
            var msg = "";
            if (kehu != null)
            {
                UnitType = 1;
                msg = "客户：" + shoukuandanwei;
            }
            else if (gonyingshang != null)
            {
                UnitType = 2;
                msg = "供应商：" + shoukuandanwei;
            }
            else
            {
                UnitType = 3;
                msg = "个人:" + shoukuandanwei;
            }
            return new { UnitType, msg };
        }
        /// <summary>
        /// 获取付款单位1：客户 2：供应商，3：个人，完全匹配，不支持模糊
        /// </summary>
        /// <param name="fukuandanwei">收款单位（全称）</param>
        /// <returns></returns>
        public string getUnit(string fukuandanwei)
        {
            var kehu = geCustomertUnit_getDep(fukuandanwei);//客户
            if (kehu != null)
            {
                return "客户：" + kehu.cDepName + kehu.cCusName;
            }
            var gonyingshang = GeVendortUnit_getDep(fukuandanwei);//供应商
            if (gonyingshang != null)
            {
                return "供应商：" + kehu.cDepName + kehu.cCusName; ;
            }
            var dep = DepartmentByPerson(fukuandanwei);
            if (dep != null)
            {
                return dep.cDepName+fukuandanwei;
            }
            return  "";
        }
        /// <summary>
        /// 获取付款单位
        /// </summary>
        /// <returns></returns>
        public List<CustomertModel> UnitList()
        {
            var kehu = geCustomertUnit_getDepList();//客户
            var gonyingshang = GeVendortUnit_getDepList();//供应商
            kehu.AddRange(gonyingshang);
            var person = DepartmentByPersonList();
            kehu.AddRange(person);
            return kehu;
        }
        /// <summary>
        /// 读取出纳管理_基础档案类型，目前默认的是现金流量项目
        /// </summary>
        /// <param name="className">项目中文全称，18年好像有三个现金流量项目，项目管理，研发项目</param>
        /// <returns></returns>
        public CN_LevelClass getLervelClass(string className) {
            using (var db = new  UFDATA_048_2017Entities(u8Db))
            {
                try
                {
                    //int dateNowYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    var result = db.CN_LevelClass.Where(c => c.lYear == year && c.ClassName.Equals(className)).FirstOrDefault();
                    return result;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog(GetIP.getIP() + "------------------------" + ex.Message);
                    return null;

                    throw;
                }
            }
        }
        /// <summary>
        /// 通过OA/银行流水号获取是否有日记账记录
        /// </summary>
        /// <param name="liushuihao"></param>
        /// <returns></returns>
        public RecordTable getRecordByLiushui_old(string liushuihao)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {

                    var result = (from r in db.RecordTable
                                  join c in db.yq_paymentRecord on r.liushuihao equals c.frontLogNo
                                  where c.frontLogNo==liushuihao || r.liushuihao==liushuihao
                                  select r).FirstOrDefault();//读取日记账记录
                    return result;
                    //return db.RecordTable.Where(o => o.liushuihao.Equals(liushuihao)).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过OA/银行流水号获取是否有日记账记录
        /// </summary>
        /// <param name="liushuihao">流水号</param>
        /// <param name="type">收/付（如果是公司内转那么流水号是一致的）</param>
        /// <param name="contents">U8银行id</param>
        /// <returns></returns>
        public RecordTable getRecordByLiushui(string liushuihao,string type,string contents)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.RecordTable.Where(o => o.liushuihao.Equals(liushuihao) && o.contents== contents && o.chunabianhao.Contains(type)).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取所有日记账中间表数据
        /// </summary>
        /// <returns></returns>
        public List<RecordTable> GetRecordList(DateTime? yyDate)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.RecordTable.Where(o=>o.isDel==null && o.yyDate==yyDate).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 通过付款返回银行流水号获取OA流水号
        /// </summary>
        /// <param name="frontLogNo">frontLogNo、</param>
        /// <returns></returns>
        public yq_paymentRecord getRecordByPayLiushui(string frontLogNo)
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_paymentRecord.Where(o => o.frontLogNo.Equals(frontLogNo)).FirstOrDefault();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取所有交易列表（未删除且已审批）
        /// </summary>
        /// <returns></returns>
        public List<yq_paymentRecord> GetYq_paymentRecordList() {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.yq_paymentRecord.Where(o=>o.isDel==null && o.isApproval==true).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
