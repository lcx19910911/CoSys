using CoSys.Model;
using CoSys.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CoSys.Core
{
    public static class LoginHelper
    {
        

        public static void CreateUser(User user)
        {
            HttpContext.Current.Session[Params.UserCookieName] = CryptoHelper.AES_Encrypt(new User() {
                ID = user.ID,
           } .ToJson(), Params.SecretKey);
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static User GetCurrentUser()
        {
            var userObj = HttpContext.Current.Session[Params.UserCookieName];
            if (userObj == null)
                return null;
            using (var db = new DbRepository())
            {
                User user = db.User.Find(GetCurrentUserID());          
                return user;
            }
        }


        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserID()
        {
            var userObj = HttpContext.Current.Session[Params.UserCookieName];
            if (userObj == null)
                return null;
            User user = (CryptoHelper.AES_Decrypt(userObj.ToString(), Params.SecretKey)).DeserializeJson<User>();
            return user.ID;
        }


        /// <summary>
        /// 是否登陆
        /// </summary>
        /// <returns></returns>
        public static bool UserIsLogin()
        {
            var userObj = HttpContext.Current.Session[Params.UserCookieName];
            if (userObj == null)
                return false;
            else
                return true;
        }
        

        public static void CreateAdmin(Admin user)
        {
            // 写登录Cookie
            HttpContext.Current.Session[Params.AdminCookieName] = CryptoHelper.AES_Encrypt(new Admin() {
                ID=user.ID,
                IsSuperAdmin=user.IsSuperAdmin,
                Account=user.Account,
                Name=user.Name,
                Mobile=user.Mobile,
            }.ToJson(), Params.SecretKey);
        }

        /// <summary>
        /// 获取当前管理员
        /// </summary>
        /// <returns></returns>
        public static Admin GetCurrentAdmin()
        {
            var userObj = HttpContext.Current.Session[Params.AdminCookieName];
            if (userObj == null)
                return null;
            using (var db = new DbRepository())
            {
                Admin user = db.Admin.Find(GetCurrentAdminID());
                return user;
            }
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAdminID()
        {
            var userObj = HttpContext.Current.Session[Params.AdminCookieName];
            if (userObj == null)
                return null;
            Admin user = (CryptoHelper.AES_Decrypt(userObj.ToString(), Params.SecretKey)).DeserializeJson<Admin>();
            return user.ID;
        }

        /// <summary>
        /// 是否登陆
        /// </summary>
        /// <returns></returns>
        public static bool AdminIsLogin()
        {
            var userObj = HttpContext.Current.Session[Params.AdminCookieName];
            if (userObj == null)
                return false;
            else
                return true;
        }
    }
}
