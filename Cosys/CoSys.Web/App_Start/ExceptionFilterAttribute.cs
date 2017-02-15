
using CoSys.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CoSys.Web
{
    public class ExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
                return;
            var ex = filterContext.Exception ?? new Exception("no further information exists");
            filterContext.ExceptionHandled = true;
            
            LogHelper.WriteException(ex);
            RedirectResult redirectResult = new RedirectResult("/base/_500");
            filterContext.Result = redirectResult;
        }
    }
}