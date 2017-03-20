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
        

        public static void CreateUser(string id)
        {
            HttpCookie cookie = new HttpCookie(Params.UserCookieName);
            cookie.Value = id;
            cookie.Expires = DateTime.Now.AddMinutes(Params.CookieExpires);
            // 写登录Cookie
            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void ClearUser()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.UserCookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddHours(-1);
                HttpContext.Current.Response.Cookies.Remove(cookie.Name);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static User GetCurrentUser()
        {
            var userId = GetCurrentUserID();
           if (userId.IsNullOrEmpty())
                return null;
            using (var db = new DbRepository())
            {
                User user = db.User.Find(userId);          
                return user;
            }
        }


        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserID()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.UserCookieName];
            if (cookie == null)
                return null;
            return cookie.Value;
        }


        /// <summary>
        /// 是否登陆
        /// </summary>
        /// <returns></returns>
        public static bool UserIsLogin()
        {
            return GetCurrentUserID().IsNotNullOrEmpty();
        }
        

        public static void CreateAdmin(string id)
        {
            HttpCookie cookie = new HttpCookie(Params.AdminCookieName);
            cookie.Value = id;
            cookie.Expires = DateTime.Now.AddMinutes(Params.CookieExpires);
            // 写登录Cookie
            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        public static void ClearAdmin()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.AdminCookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddHours(-1);
                HttpContext.Current.Response.Cookies.Remove(cookie.Name);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        /// <summary>
        /// 获取当前管理员
        /// </summary>
        /// <returns></returns>
        public static User GetCurrentAdmin()
        {
            var id = GetCurrentAdminID();
            if (id.IsNullOrEmpty())
                return null;
            using (var db = new DbRepository())
            {
                User user = db.User.Find(id);
                return user;
            }
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAdminID()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.AdminCookieName];
            if (cookie == null)
                return null;
            return cookie.Value;
        }

        /// <summary>
        /// 是否登陆
        /// </summary>
        /// <returns></returns>
        public static bool AdminIsLogin()
        {
            return GetCurrentAdminID().IsNotNullOrEmpty();
        }
    }
}
