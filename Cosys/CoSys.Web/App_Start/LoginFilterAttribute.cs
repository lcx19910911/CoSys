
using CoSys.Core;
using CoSys.Model;
using CoSys.Service;
using CoSys.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web
{
    /// <summary>
    /// 过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LoginFilterAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as BaseController;


            var controllerName = filterContext.RouteData.Values["Controller"].ToString();
            string requestUrl = filterContext.HttpContext.Request.Url.ToString();
            
            if (!LoginHelper.UserIsLogin()&&!LoginHelper.AdminIsLogin())
            {
                RedirectResult redirectResult = new RedirectResult("/Accout/Login?redirecturl=" + requestUrl);
                filterContext.Result = redirectResult;
            }
            else
            {

            }
        }
    }
}