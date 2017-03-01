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
        public ActionResult Register(string id)
        {
            var model = WebService.Find_User(id);
            if(model==null)
                new User();
            model.ProvinceItems = WebService.Get_AreaList("");
            if (model.ID.IsNotNullOrEmpty())
            {
                model.CityItems = WebService.Get_AreaList(model.ProvoniceCode);
                model.CountyItems = WebService.Get_AreaList(model.CityCode);
                model.StreetItems = WebService.Get_AreaList(model.CountyCode);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Register(User model)
        {

            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("Password");
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
        [HttpPost]
        public ActionResult Save(User model)
        {

            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("Password");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_User(model);
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
            LoginHelper.ClearAdmin();
            LoginHelper.ClearUser();
            return View("Login");
        }

        public ActionResult ChangeAdminPassword(string oldPassword, string newPassword, string cfmPassword,string id)
        {
            return JResult(WebService.Admin_ChangePassword(oldPassword, newPassword, cfmPassword,id));
        }
        public ActionResult ChangeUserPassword(string oldPassword, string newPassword, string cfmPassword)
        {
            return JResult(WebService.User_ChangePassword(oldPassword, newPassword, cfmPassword));
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