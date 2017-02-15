using CoSys.Core;
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CoSys.Core
{
    //public static class CookieHelper
    //{

    //    //更新用户的cookie
    //    public static void CreateCookie(User user)
    //    {
    //        try
    //        {
    //            HttpCookie cookie = new HttpCookie(Params.UserCookieName);
    //            cookie.Value = CryptoHelper.AES_Encrypt(user.ToJson(), Params.SecretKey);
    //            cookie.Expires = DateTime.Now.AddMinutes(Params.CookieExpires);
    //            // 写登录Cookie
    //            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
    //            HttpContext.Current.Response.Cookies.Add(cookie);
    //        }
    //        catch (Exception ex)
    //        {
    //            LogHelper.WriteException(ex);
    //        }
    //    }

    //    /// <summary>
    //    /// 获取当前用户
    //    /// </summary>
    //    /// <returns></returns>
    //    public static User GetCurrentUser()
    //    {
    //        HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.UserCookieName];
    //        if (cookie == null)
    //            return null;
    //        User user = (CryptoHelper.AES_Decrypt(cookie.Value, Params.SecretKey)).DeserializeJson<User>();
    //        return user;
    //    }

    //    /// <summary>
    //    /// 获取当前用户
    //    /// </summary>
    //    /// <returns></returns>
    //    public static void ClearCookie()
    //    {
    //        HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.UserCookieName];
    //        if (cookie != null)
    //        {
    //            cookie.Expires = DateTime.Now.AddHours(-1);
    //            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
    //            HttpContext.Current.Response.Cookies.Add(cookie);
    //        }
    //    }

    //    /// <summary>
    //    /// 是否登陆
    //    /// </summary>
    //    /// <returns></returns>
    //    public static bool IsLogin()
    //    {
    //        HttpCookie cookie = HttpContext.Current.Request.Cookies[Params.UserCookieName];
    //        if (cookie == null)
    //            return false;
    //        else
    //            return true;
    //    }
    //}
}
