using common;
using Log;
using Model;
using EntityFromework;
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
    public class UserInfoController : ApiController
    {
        public static UserInfoBll bll = new UserInfoBll();
        /// <summary>
        /// 获取客户机mac地址（web无法获取非同一局域网的mac）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UserInfo/getMac")]
        public object getMac()
        {
            return bll.getMac();
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserInfo/loginUserInfo")]
        [ActionName("loginUserInfo")]
        public object loginUserInfo(UserModel user)
        {
            user= bll.loginUserInfo(user);
            HttpContext.Current.Session["userInfo"] = null;
            HttpContext.Current.Session["caiwu"] = null;
            //如果type为1则表示是财务审单
            if (user.userinfo != null && user.userinfo.type == 1 ) {//财务权限的话需要判断下拉是否有财务这个下拉选项
                if (user.caiwu == null) {
                    return new { start = 1, user = user.userinfo, msg = "权限不足，请联系管理员" };
                }
                HttpContext.Current.Session["caiwu"] = user.caiwu;
            }
            HttpContext.Current.Session["userInfo"] = user.userinfo;
            HttpContext.Current.Session.Timeout = 6000;
            return user.userinfo == null? new { start = 1, user = new userInfo(), msg = "账号或密码错误" } : new { start=0,user = user.userinfo, msg="登录成功"};
        }
        [HttpPost]
        [Route("api/UserInfo/getUserInfo")]
        [ActionName("getUserInfo")]
        public object getUserInfo()
        {
            return HttpContext.Current.Session["userInfo"];
        }
        /// <summary>
        /// 短信催办
        /// </summary>
        /// <param name="name"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/UserInfo/noteMsg")]
        [ActionName("noteMsg")]
        public object noteMsg(string name,string msg) {
            return  bll.noteMsg( name, msg);
        }
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserInfo/getCode")]
        [ActionName("getCode")]
        public ResponseMessage getCode(UserModel user)
        {
            var message = bll.getCode(user);
            if (message.start==1 && string.IsNullOrEmpty(message.other))
            {
                HttpContext.Current.Session["code"] = null;
                HttpContext.Current.Session["code"] = message.other;
            }
            return message;
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserInfo/updateUserInfo")]
        [ActionName("updateUserInfo")]
        public object updateUserInfo(UserModel user)
        {
            var userInfo= bll.updateUserInfo(user);
            HttpContext.Current.Session["userInfo"]=null;
            return userInfo;
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/UserInfo/editPrintRecord")]
        [ActionName("editPrintRecord")]
        public object editPrintRecord(PrintRecord print)
        {
            var user= HttpContext.Current.Session["userInfo"] as userInfo;
            return bll.editPrintRecord(print,user);
        }

    }
}
