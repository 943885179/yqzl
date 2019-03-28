using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using BLL;
using EntityFromework;
using Model;
using PinganYqzl;
using PinganYqzl.model;
using ZhaoshangYqzl.model;

namespace danjukaipiao.Controllers.api
{
    public class YqzlController : ApiController
    { /// <summary>
      /// 查询交易
      /// </summary>
      /// <param name="ThirdVoucher"></param>
      /// <returns></returns>
        [HttpPost]
        [Route("api/Yqzl/qryDtlList")]
        [ActionName("qryDtlList")]
        public YqzlResponseModel<PaysModel> qryDtlList(YqzlRequestModel request)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new YqzlResponseModel<PaysModel>() {code="403", message = "无权操作" };
            }
            var bll = new YqzlBll();
            return bll.qryDtlList(request,user);
        }
        [HttpGet]
        [Route("api/Yqzl/Approval")]
        [ActionName("Approval")]
        public object Approval(string liushuihao)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            var bll = new YqzlBll();
            return bll.Approval(user,liushuihao);
        }
        
        /// <summary>
        /// 更新交易状态
        /// </summary>
        /// <param name="ThirdVoucher"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/qryDtlByOrig")]
        [ActionName("qryDtlByOrig")]
        public object qryDtlByOrig(String ThirdVoucher)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new { errMsg = "无权操作！" };
            }
            var bll = new YqzlBll();
            return bll.qryDtlByOrig(ThirdVoucher,user);
        }
        /// <summary>
        /// 撤销交易
        /// </summary>
        /// <param name="liushuihao"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/RemovePay")]
        [ActionName("RemovePay")]
        public object RemovePay(string liushuihao)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 3 && user.type != 0)
            {
                return new { errMsg = "无权操作！" };
            }
            var bll = new YqzlBll();
            var errMsg = bll.RemovePay(liushuihao);
            return new { errMsg };
        }
        /// <summary>
        /// 读取交易列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/yq_paymentRecordList")]
        [ActionName("yq_paymentRecordList")]
        public object yq_paymentRecordList(string isPay)
        {
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            if (user.type != 0 && user.type != 3)
            {
                return new { start = 1, errMsg = "无权操作！" };
            }
            YqzlBll api = new YqzlBll();
            if (user.company == null)
            {
                return new { start = 1, errMsg = "公司异常！" };
            }
            return api.yq_paymentRecordList(isPay,user);
        }
        /// <summary>
        /// 获取收款单位的省市
        /// </summary>
        /// <param name="shoukuanyinhan"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/ecs_regionPcity")]
        [ActionName("ecs_regionPcity")]
        public List<ecs_region> ecs_regionPcity(string shoukuanyinhan)
        {
            //return new YqzlBll().ecs_regionPcity(shoukuanyinhan);
            //采用新方法（原来区可能存在重复）
            List<ecs_region> ecs= new YqzlBll().ecs_regionPcity_New(shoukuanyinhan);
            if (ecs.Count==0 || ecs.Where(o=>o.region_type==2).FirstOrDefault()==null)
            {
                return null;//未找到所属城市
            }
            return new List<ecs_region>() {
                ecs.Where(o => o.region_type == 1).First(),//省
                ecs.Where(o => o.region_type == 2).First()//市
            };
            //return ecs.Where(o => o.region_type == 1).First().region_name + ecs.Where(o => o.region_type == 2).First().region_name;
        }
        /// <summary>
        /// 删除失败的交易记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/delPay")]
        [ActionName("delPay")]
        public object delPay(int id)
        {
            return new YqzlBll().delPay(id);
        }
        /// <summary>
        /// 获取移动支票授权人
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/qryYidong")]
        [ActionName("qryYidong")]
        public ResonseClass qryYidong()
        {
            return new YqzlBll().qryYidong();
        }
          /// <summary>
          /// 
          /// </summary>
          /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/ywms")]
        [ActionName("ywms")]
        public ResonseClass ywms(string ywdm)
        {
            return new YqzlBll().ywms(ywdm);
        }
        /// <summary>
        ///银联号查询
        /// </summary>
        /// <param name="BankName"></param>
        /// <param name="KeyWord"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/yhdm")]
        [ActionName("yhdm")]
        public PinganResponse<YlhModelResponse> yhdm(string BankName)
        {
            return new YqzlBll().yhdm(BankName);
        }

        /// <summary>
        /// 回单图片下载
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Yqzl/SdkcsfdfbrtImg")]
        [ActionName("yhdm")]
        public object YqzlRequestModel(DateTime BeginDate,string AcctNo,string Reserve)
        {
            var yqzlRequest = new YqzlRequestModel();
            yqzlRequest.BeginDate = BeginDate;
            yqzlRequest.AcctNo = AcctNo;
            yqzlRequest.Reserve = Reserve;//预留字段传银行名
            var user = HttpContext.Current.Session["userInfo"] as userInfo;
            var bll = new YqzlBll();
            return bll.SdkcsfdfbrtImg(yqzlRequest,user);
        }
    }
}
