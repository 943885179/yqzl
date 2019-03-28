using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFromework;

namespace Model
{
    public class UserModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool start { get; set; }
        public int type { get; set; }
        public string phnoe { get; set; }
        public string loginName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string newpassword { get; set; }
        public string code { get; set; }
        public int company { get; set; }
        public string phone { get; set; }
        public string  mac { get; set; }
        public string errmsg { get; set; }
        public userInfo userinfo { get; set; }
        public form_enumvalue caiwu { get; set; }
    }
}
