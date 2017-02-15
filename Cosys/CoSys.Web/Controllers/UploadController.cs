using CoSys.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    [AllowAnonymous]
    public class UploadController : Controller
    {
        // GET: Upload
        public ActionResult UploadImage(string mark)
        {
            HttpPostedFileBase file = Request.Files[0];
            if (file != null)
            {
                string path = UploadHelper.Save(file, mark);
                return Content(path);
            }
            else
                return Content("");
        }
    }
}