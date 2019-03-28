using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFromework;

namespace dal
{
    /// <summary>
    /// 消息
    /// </summary>
  public  class MessageShowDal
    {  /// <summary>
       ///获取消息队列
       /// </summary>
       /// <returns></returns>
        public List<MessageShow> getMessageShow()
        {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    return db.MessageShow.Where(o => o.isOut == 0 && !string.IsNullOrEmpty(o.phone)).ToList();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 信息状态修改为已发送
        /// </summary>
        public void editMessageShow(int id) {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    var msg = db.MessageShow.Where(o => o.id == id).First();
                    msg.isOut = 1;
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
