using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using CoSys.Core;

namespace CoSys.Web.Controllers
{
    public class AccoutController : BaseController
    {
        // GET: Login
        public ActionResult Login()
        {
            WebService.GetArea();
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        

        /// <summary>
        /// 登录提交
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param> 
        /// <returns></returns>
        public JsonResult Submit(string account, string password)
        {
            return JResult(WebService.Login(account, password));
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Quit()
        {
            Client.LoginUser = null;
            Client.LoginAdmin = null;
            return View("Login");
        }

        public ActionResult ChangePassword(string oldPassword, string newPassword, string cfmPassword)
        {
            return JResult(WebService.Admin_ChangePassword(oldPassword, newPassword, cfmPassword));
        }
    }
}