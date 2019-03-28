using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common;
using dal;
using EntityFromework;
using model;

namespace BLL
{
   public class FromOABll
    {
        private static FromOADal f = new FromOADal();
        /// <summary>
        /// 获取基本信息列表
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        public List<ResultListModel> getList(Condition condition)
        {
            return f.getList(condition);
        }
        public object getDepartmentHeadList()
        {
            return f.getDepartmentHeadList();
        }

        /// <summary>
        /// 获取关联数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public object getSunByattachment(ResultListModel model, Condition condition)
        {
            return f.getSunByattachment(model, condition);
        }
        /// <summary>
        /// 打印时候获取详细信息，包含没个申请的明细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object getDetail(ResultListModel model, Condition condition)
        {
            if (model == null || string.IsNullOrEmpty(model.type))
            {
                ResultListModel re = new ResultListModel
                {
                    list = new List<Detail>()
                };
                Detail d = new Detail();
                re.lAmount = 0;
                d.jine = 0;
                re.list.Add(d);
                return re;
            }
            return f.getDetail(model, condition);
        }/// <summary>
         /// 获取流程签字
         /// </summary>
         /// <param name="pro"></param>
         /// <returns></returns>
        public object getProcess(WriteModel pro, userInfo user)
        {
            //后期如果不在统计01-23前的流程签字可以使用New（需要改前端逻辑）
            if (user.company == (int)CompanyEnum.yueji)
            {
                return f.getProcessListNew(pro);
            }
            return f.getProcessList(pro);
        }
        /// <summary>
        /// 获取出纳日记账信息
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public object getChunaBianhao(string pid)
        {
            return f.getChunaBianhao(pid);
        }
        /// <summary>
        /// 获取预支单科目大纲
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public object getCode(WriteModel pro)
        {
            return f.getCode("U8预支单费用科目");
        }
        /// <summary>
        /// 获取费用科目大纲
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public object getCodes(WriteModel pro, userInfo user)
        {
            if (user.company == (int)CompanyEnum.yueji)
            {
                return f.getCode("悦肌U8科目");
            }
            return f.getCode("U8046费用科目");
        }
        /// <summary>
        /// 添加手工做单
        /// </summary>
        /// <param name="pro"></param>
        /// <returns></returns>
        public object addToYYBymaual(List<ResultListModel> model, userInfo user)
        {
            return f.addToYYBymaual(model, user);
        }
        /// <summary>
        /// 获取财务列表
        /// </summary>
        /// <returns></returns>
        public object getCiawuList()
        {
            return f.getCiawuList();
        }
        /// <summary>
        /// 获取转移列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public object getFinancialList( userInfo user)
        {
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
        public object editFinancial(string type, string tabName, string liushuiId, string liushuiCol, string fromFinancial, string toFinancial, string caiwuCol, userInfo user)
        {
            return f.editFinancial(type, tabName, liushuiId, liushuiCol, fromFinancial, toFinancial, caiwuCol, user);
        }

    }
}
