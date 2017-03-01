﻿using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    [LoginFilter]
    public class HomeController : BaseController
    {
        // GET: 
        public ActionResult Index()
        {
            if (Client.LoginAdmin != null)
            {
                return RedirectToAction("Admin");
            }
            if (Client.LoginUser == null)
            {
                return RedirectToAction("Login","Account");
            }
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}