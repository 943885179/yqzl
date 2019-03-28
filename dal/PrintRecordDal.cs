using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using EntityFromework;

namespace dal
{
    /// <summary>
    /// 打印操作dal
    /// </summary>
   public class PrintRecordDal
    {
        /// <summary>
        /// 添加打印记录
        /// </summary>
        /// <param name="print"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public object editPrintRecord(PrintRecord print,userInfo user) {
            using (var db = new OAtoU8DATAEntities())
            {
                try
                {
                    PrintRecordBody body = new PrintRecordBody();
                    print.num = 1;
                    print.createdate = DateTime.Now;
                    var oldPrint = db.PrintRecord.Where(o => o.colid == print.colid).FirstOrDefault();
                    if (oldPrint!=null)
                    {
                        oldPrint.num++;
                        db.SaveChanges();
                        print = oldPrint;
                    }
                    else
                    {
                        db.PrintRecord.Add(print);
                        db.SaveChanges();
                    }
                    body.userid = user.id;
                    body.ip = GetIP.GetNetAdapterInfos().ip;
                    body.colid = print.colid;
                    body.createdate = DateTime.Now;
                    body.pid = print.id;
                    db.PrintRecordBody.Add(body);
                    db.SaveChanges();
                    return new { start = 0,msg="打印记录成功" };
                }
                catch (Exception ex)
                {
                    return new { start = 1, msg = ex.ToString() };
                }
        }
        }
    }
}
