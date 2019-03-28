using common;
using danjukaipiao.App_Start;
using danjukaipiao.Authorize;
using model;
using EntityFromework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BLL;

namespace danjukaipiao.Controllers.api
{
    //[ApiAuthorize]
    public class ListController : ApiController
    {
        private static FromOABll f = new FromOABll();
        /// <summary>
        /// 获取基本信息列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [Route("api/List/getList")]
        [ActionName("getList")]
        public object getList(Condition condition) {
            condition.form_enumvalue = (form_enumvalue) HttpContext.Current.Session["caiwu"];
            condition.userInfo =(userInfo)HttpContext.Current.Session["userInfo"];
            return f.getList(condition);
        }
        [HttpGet]
        [Route("api/List/getDepartmentHeadList")]
        [ActionName("getDepartmentHeadList")]
        public object getDepartmentHeadList()
        {
            return f.getDepartmentHeadList();
        }
        
        /// <summary>
        /// 获取关联数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/getSunByattachment")]
        [ActionName("getSunByattachment")]
        public object getSunByattachment(ResultListModel model)
        {
            Condition condition = new Condition
            {
                userInfo = (userInfo)HttpContext.Current.Session["userInfo"]
            };
            return f.getSunByattachment(model, condition);
        }
        /// <summary>
        /// 打印时候获取详细信息，包含没个申请的明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/getDetail")]
        [ActionName("getDetail")]
        public object getDetail(ResultListModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.type))
            {
                ResultListModel re = new ResultListModel
                {
                    list = new List<Detail>()
                };
                Detail d = new Detail();
                re.lAmount =0;
                d.jine = 0;
                re.list.Add(d);
                return re;
            }
            Condition condition = new Condition
            {
                userInfo = (userInfo)HttpContext.Current.Session["userInfo"]
            };
            return f.getDetail(model, condition);
        }/// <summary>
        /// 获取流程签字
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/getProcess")]
        [ActionName("getProcess")]
        public object getProcess(WriteModel pro)
        { 
            var user = (userInfo)HttpContext.Current.Session["userInfo"];
            return f.getProcess(pro, user);
        }
        /// <summary>
        /// 获取出纳日记账信息
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/getChunaBianhao")]
        [ActionName("getChunaBianhao")]
        public object getChunaBianhao([FromUri] string pid)
        {
           return f.getChunaBianhao(pid);
        }
        /// <summary>
        /// 获取预支单科目大纲
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/getCode")]
        [ActionName("getCode")]
        public object getCode(WriteModel pro)
        {
            return f.getCode(pro);
        }
        /// <summary>
        /// 获取费用科目大纲
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/getCodes")]
        [ActionName("getCodes")]
        public object getCodes(WriteModel pro)
        {
            var user = (userInfo)HttpContext.Current.Session["userInfo"];
            return f.getCodes(pro, user);
        }
        /// <summary>
        /// 添加手工做单
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/List/addToYYBymaual")]
        [ActionName("addToYYBymaual")]
        public object addToYYBymaual(List<ResultListModel> model)
        {
            var user = (userInfo)HttpContext.Current.Session["userInfo"];
            return f.addToYYBymaual(model, user);
        }
        /// <summary>
        /// 获取财务列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/List/getCiawuList")]
        [ActionName("getCiawuList")]
        public object getCiawuList()
        {
            return f.getCiawuList();
        }
        /// <summary>
        /// 获取转移列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/List/getFinancialList")]
        [ActionName("getFinancialList")]
        public object getFinancialList()
        {
            var user = (userInfo)HttpContext.Current.Session["userInfo"];
            return f.getFinancialList(user);
        }
        /// <summary>
        /// 修改审单财务
        /// </summary>
        /// <param name="type">单据类型</param>
        /// <param name="tabName">表名</param>
        /// <param name="liushuiId">流水号</param>
        /// <param name="liushuiCol">流水列名</param>
        /// <param name="fromFinancial">来源财务ID</param>
        /// <param name="toFinancial">交接财务ID</param>
        /// <param name="caiwuCol">财务所在表字段列名</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/List/editFinancial")]
        [ActionName("editFinancial")]
        public object editFinancial(string type, string tabName, string liushuiId, string liushuiCol, string fromFinancial, string toFinancial, string caiwuCol)
        {
            var user = (userInfo)HttpContext.Current.Session["userInfo"];
            return f.editFinancial(type, tabName, liushuiId, liushuiCol, fromFinancial, toFinancial, caiwuCol, user);
        }

    }
}
