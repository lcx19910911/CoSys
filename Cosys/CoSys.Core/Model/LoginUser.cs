
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    /// <summary>
    /// 登录用户
    /// </summary>
    public class LoginUser
    {
        public LoginUser(User user)
        {
            this.ID = user.ID;
            this.Account = user.Account;
            this.Name = user.RealName;
        }


        public LoginUser()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string Name { get; set; }
        

        /// <summary>
        /// 权限值
        /// </summary>
        public Nullable<long> MenuFlag { get; set; }
    }
}
