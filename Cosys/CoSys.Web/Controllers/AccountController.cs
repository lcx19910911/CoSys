using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using CoSys.Core;
using CoSys.Model;

namespace CoSys.Web.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Login
        public ActionResult Login()
        {
            //WebService.GetArea();
            return View();
        }
        public ActionResult Register()
        {
            var model = new User();
            model.ProvinceItems = WebService.Get_AreaList("");
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(User model)
        {

            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_User(model);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
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

        #region 验证码
        /// <summary>
        /// 验证码
        /// </summary>
        public void ValidateCode()
        {
            var tuple = WebService.Create_ValidateCode(Client.IP);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.ClearContent();
            HttpContext.Response.ContentType = "image/Jpeg";
            HttpContext.Response.BinaryWrite(tuple.Item1.CreateImageStream(tuple.Item2));    //  输出图片
        }
        #endregion
    }
}