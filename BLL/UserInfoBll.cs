using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using dal;
using EntityFromework;
using Log;
using Model;

namespace BLL
{
    public class UserInfoBll
    {

        private UserInfoDal dal;
        private FromOADal oA;
        public UserInfoBll()
        {
            dal = new UserInfoDal();
            oA = new FromOADal();
        }
        public object getMac()
        {
            string ip = GetIP.GetWebClientIp();
            var mac = GetIP.GetMacAddress(ip);
            // string ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            return "Ip:" + ip + "\nMAC:" + mac;
            // return GetIP.GetNetAdapterInfo();
        }
        public UserModel loginUserInfo(UserModel user)
        {
            user.userinfo = dal.getUserInfo(user);
            if (user.userinfo != null)
            {//由于一个人可能是多个部门的，所以只能改用前端选择公司来判断员工归属公司
                user.userinfo.company = user.company;
            }
            if (user.userinfo != null && user.userinfo.type != 2)
            {//为财务出纳人员登录
             //需要短信验证
             /* if (string.IsNullOrEmpty(user.code)) {
                    return new { start =1, user = new userInfo(), msg = "请输入验证码" };
                }
               var code= HttpContext.Current.Session["code"].ToString();
                if (!code.Equals(user.code))
                {
                    return new { start = 1, user = new userInfo(), msg = "验证码输入错误" };
                }*/
                var ip = GetIP.GetWebClientIp();
                Log4Helper.Info(typeof(UserInfoBll), "账号:" + user.userinfo.name + "在：IP为" + ip + "登录");

                //mac地址不再做验证了，不然只能服务器放在同一个局域网里，如果放到OA服务器则会相同
                /*
                var mac = GetIP.GetMacAddress(ip);
                if (!userInfo.mac.Equals(mac))
                {
                    return new { start = 1, user = new userInfo(), msg = "操作地址有误，如果跟换了电脑，请联系管理员" };
                }*/
                //这个是将公司强制替换成前端传过来的公司，让这个人又这个公司的操作权限
            }
            //如果type为1则表示是财务审单
            if (user.userinfo != null && user.userinfo.type == 1)
            {//财务权限的话需要判断下拉是否有财务这个下拉选项
                if (user.userinfo.name == "胡珊_admin")
                {
                    user.userinfo.name = "胡姗_admin";
                }
                user.caiwu = dal.getCaiwu(user.userinfo);
            }
            if (user.userinfo != null)
            {
                user.userinfo.password = "";
            }
            return user;
        }
        public object noteMsg(string name, string msg)
        {
            var msgUser = oA.getV3xUserByName(name);
            if (msgUser != null && !string.IsNullOrEmpty(msgUser.tel_number))
            {
                bool msgnote = NoteHelper.SendSMSSingleByYunYongForCoder(msgUser.tel_number, "【膜法世家】" + msg);
                if (msgnote)
                {
                    LogHelper.WriteLog(msgUser.tel_number + "发送消息: 【膜法世家】" + msg + "，请确保本人操作,---" + DateTime.Now, "noteLog");
                    return new { start = 0, msg = "催办成功！" };
                }
                else { return new { start = 1, msg = "短信发送失败，请重试！多次重试无法发送可能是审批人保留电话有误！请联系管理员！" }; }
            }

            return new { start = 1, msg = "找不到审批人或审批人未设置号码！" };
        }
        public ResponseMessage getCode(UserModel user)
        {
            var message = new ResponseMessage();
            message.start = 1;
            //需要短信验证
            if (string.IsNullOrEmpty(user.userName))
            {
                message.msg= "请输入用户名";
            }
            if (string.IsNullOrEmpty(user.password))
            {
                message.msg = "请输入密码";
            }
            var userInfo = dal.getUserInfo(user);
            if (userInfo != null && userInfo.type != 2)
            {//为财务出纳人员登录

                Random random = new Random();
                var code = random.Next(100000, 1000000);
                bool msg = NoteHelper.SendSMSSingleByYunYongForCoder(userInfo.phnoe, "【膜法世家】本次验证码" + code + "，请确保本人操作");
                if (msg)
                {
                    LogHelper.WriteLog(userInfo.phnoe + "发送消息:【膜法世家】本次验证码" + code + "，请确保本人操作,---" + DateTime.Now, "noteLog");
                    message.msg = "短信发送成功";
                    message.start = 0;
                }
                message.msg = "发送失败";
            }
            message.msg = "普通账号无需验证码，请直接点登录即可";
            return message;
        }
        public object updateUserInfo(UserModel user)
        {
            return dal.updateUserInfo(user);
        }
        public object editPrintRecord(PrintRecord print,userInfo user)
        {
            PrintRecordDal printRecordDal = new PrintRecordDal();
            return printRecordDal.editPrintRecord(print, user);
        }

    }
}
