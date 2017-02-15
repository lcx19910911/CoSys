using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Security;

namespace CoSys.Core
{
    public class Params
    {
        /// <summary>
        /// 缓存前缀
        /// </summary>
        public static readonly string Cache_Prefix_Key = "Cosys";

        public static readonly string Mobile = ConfigurationManager.AppSettings["Mobile"];

        /// <summary>
        /// 公司名称
        /// </summary>
        public static readonly string CompanyName = ConfigurationManager.AppSettings["CompanyName"];               

        /// <summary>
        /// 密钥
        /// </summary>
        public static readonly string SecretKey = ConfigurationManager.AppSettings["SecretKey"];
        

        /// <summary>
        /// 登陆cookie
        /// </summary>
        public static readonly string UserCookieName = "CoSys_user";

        /// <summary>
        /// 登陆cookie
        /// </summary>
        public static readonly string AdminCookieName = "CoSys_admin";

        /// <summary>
        /// cookie 过期时间
        /// </summary>
        public static readonly int CookieExpires = 12000;
    }
}
