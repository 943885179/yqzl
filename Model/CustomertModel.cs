using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
   public class CustomertModel
    {
        /// <summary>
        /// 单位名称
        /// </summary>
        public string cCusName { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string cDepName { get; set; }

        public string  cDepCode { get; set; }
        public string cCusCode { get; set; }
        /// <summary>
        /// 类型（客户1还是供应商2/员工0）
        /// </summary>
        public string UnitTypeName { get; set; }
        /// <summary>
        /// 类型编码（客户还是供应商）
        /// </summary>
        public int UnitType { get; set; }
        /// <summary>
        /// CN_LevelListID 对应DeptID(部门)
        /// </summary>
        public int DeptID { get; set; }
    }
}
