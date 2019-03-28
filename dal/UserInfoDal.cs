using Log;
using Model;
using EntityFromework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dal
{
    /// <summary>
    /// 员工登录信息获取
    /// </summary>
     public  class UserInfoDal
    {
        /// <summary>
        /// 获取用户信息，登录
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public userInfo getUserInfo(UserModel user) {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {

                    var  users= db.userInfo.Where(o => o.name.Equals(user.userName) && o.password.Equals(user.password)).FirstOrDefault();
                    if (users!=null)
                    {
                        Log4Helper.Info(typeof(YYDal), "用户登录记录:"+user.userName);
                    }
                    return users;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取审单财务
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public form_enumvalue getCaiwu(userInfo user) {
            using (var db = new v3xEntities())
            {
                return (from e in db.form_enumvalue join el in db.form_enumlist on e.ref_enumid equals el.id where e.showValue.Contains(user.name.Replace("_admin","")) select e).FirstOrDefault();
            }

            }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public userInfo updateUserInfo(UserModel user) {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var oldUser= db.userInfo.Where(o => o.name.Equals(user.userName) && o.password.Equals(user.password)).FirstOrDefault();
                    oldUser.password = user.newpassword;
                    db.SaveChanges();
                    return oldUser;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}
