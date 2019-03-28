using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BLL;
using EntityFromework;
using PinganYqzl;
using PinganYqzl.model;

namespace danjukaipiao.Controllers.api
{
    public class PinganApiController : ApiController
    {
        /// <summary>
        /// 付款接口
        /// </summary>
        /// <param name="xferModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PinganApi/xFer")]
        [ActionName("xFer")]
        public object xFer(XferRequestModel xferModel)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type!=0 && user.type != 3)
            {
                return new { errMsg = "无权操作！" };
            }
            PinganApi api = new PinganApi();
            return null;//api.xFer(xferModel);
        }
        /// <summary>
        /// 付款接口
        /// </summary>
        /// <param name="xferModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PinganApi/xFer")]
        [ActionName("xFer")]
        public object xQue(XferRequestModel xferModel)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new { errMsg = "无权操作！" };
            }
            PinganApi api = new PinganApi();
            return null;
           // return api.xFer(xferModel);
        }
        /// <summary>
        /// 读取交易列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/PinganApi/yq_paymentRecordList")]
        [ActionName("yq_paymentRecordList")]
        public object yq_paymentRecordList(string isPay)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new { start=1, errMsg = "无权操作！" };
            }
            YqzlBll api = new YqzlBll();
            if (user.company==null)
            {
                return new { start = 1, errMsg = "公司异常！" };
            }
            return api.yq_paymentRecordList(isPay,user);
        }
        /// <summary>
        /// 更新交易状态
        /// </summary>
        /// <param name="ThirdVoucher"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/PinganApi/qryDtlByOrig")]
        [ActionName("qryDtlByOrig")]
        public object qryDtlByOrig(String ThirdVoucher)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new { errMsg = "无权操作！" };
            }
            PinganApi api = new PinganApi();
            return api.qryDtlByOrig(new XferRequestModel() { ThirdVoucher= ThirdVoucher,createMan=user.name });
        }
        [HttpGet]
        [Route("api/PinganApi/qryDtlXml")]
        [ActionName("qryDtlXml")]
        public string qryDtlXml(DateTime QueryDate, string Account)
        {
            PinganApi api = new PinganApi();
            return api.qryDtlXml(QueryDate, Account);
        }

        /// <summary>
        /// 查询交易
        /// </summary>
        /// <param name="ThirdVoucher"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PinganApi/qryDtl")]
        [ActionName("qryDtl")]
        public PinganResponse<qryDtlResponse> qryDtl(qryDtlRequest request)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new PinganResponse<qryDtlResponse>() {message="无权操作" };
            }
            PinganApi api = new PinganApi();
            return null;
            //return api.qryDtl(request,user);
        }
    }
} 