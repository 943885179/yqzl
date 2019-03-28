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
using PinganYqzl;
using Model;
using PinganYqzl.model;
using BLL;

namespace danjukaipiao.Controllers.api
{
    //[ApiAuthorize]
    public class U8Controller : ApiController
    {

        private static userInfo user;

        public static YYBll Bll { get; set; }

        public U8Controller() {
            user = (userInfo)HttpContext.Current.Session["userInfo"];
            Bll = new YYBll(user);
        }
        /// <summary>
        /// 获取付款账号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/getAccInfo")]
        [ActionName("getAccInfo")]
        public List<Content> getAccInfo([FromUri]int type) {
            return Bll.getAccInfo(type);//1:银行日记账,0:现金
        }
        /// <summary>
        /// 获取付款类型
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/getSettleStyle")]
        [ActionName("getSettleStyle")]
        public object getSettleStyle()
        {
            return Bll.getSettleStyle();
        }
        /// <summary>
        /// 获取供应商或者是收款人
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/getReceivables")]
        [ActionName("getReceivables")]
        public object getReceivables(ResultListModel model)
        {
            return Bll.getReceivables(model);
        }
        /// <summary>
        /// 获取付款账号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/getCodeList")]
        [ActionName("getCodeList")]
        public object getCodeList()
        {
            return Bll.getCodeList();
        }
        [HttpPost]
        [Route("api/U8/getCodeAllList")]
        [ActionName("getCodeAllList")]
        public List<code> getCodeAllList()
        {
            return Bll.getCodeAllList();
        }
        /// <summary>
        /// 获取流量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/U8/getProjectBycode")]
        [ActionName("getProjectBycode")]
        public object getProjectBycode([FromUri] string citemcode)
        {
            return Bll.getProjectBycode(citemcode);
        }
        /// <summary>
        /// 获取流量列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/U8/getProject")]
        [ActionName("getProject")]
        public List<fitemss98> getProject()
        {
            return Bll.getProject();
        }
        /// <summary>
        /// 添加收付款到U8(日记账)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/addToAcctBook")]
        [ActionName("addToAcctBook")]
        public object AddToAcctBook(ResultListModel model)
        {
            return Bll.AddToAcctBook(model);
        }
        /// <summary>
        /// 生单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/addToCloseBill")]
        [ActionName("addToCloseBill")]
        public object addToCloseBill(ResultListModel model)
        {
                return Bll.addToCloseBill(model);
        }
        /// <summary>
        /// 制单（不带签字）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/addToCashSignRelate")]
        [ActionName("addToCashSignRelate")]
        public object addToCashSignRelate(ResultListModel model)
        {
            return Bll.addToCashSignRelate(model);
        }
        /// <summary>
        /// 签字
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/addToCcashier")]
        [ActionName("addToCcashier")]
        public object addToCcashier(ResultListModel model)
        {
            return Bll.addToCcashier(model);
        }
        /// <summary>
        /// 获取付款的单位（已生成付款单）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/U8/getRecordTableByPid")]
        [ActionName("getRecordTableByPid")]
        public object getRecordTableByPid(ResultListModel model)
        {
            return Bll.getRecordTableByPid(model);
        }
         /// <summary>
           /// 获取U8部门
           /// </summary>
           /// <param name="model"></param>
           /// <returns></returns>
        [HttpPost]
        [Route("api/U8/getDepatementList")]
        [ActionName("getDepatementList")]
        public object getDepatementList()
        {
            return Bll.getDepatementList();
        }
        /// <summary>
        /// 根据收款人、单位获取客户、供应商
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/U8/getUnitType")]
        [ActionName("getUnitType")]
        public object getUnitType([FromUri] string shoukuandanwei)
        {
            return Bll.getUnitType(shoukuandanwei);
        }
    }
}
