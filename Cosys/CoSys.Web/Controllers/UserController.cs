using CoSys.Core;
using CoSys.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    /// <summary>
    /// 用户
    /// </summary>
    [LoginFilter]
    public class UserController : BaseController
    {
        // GET: 
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Admin()
        {
            return View();
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string name,int? type,int ? areaId, DateTime? searchTimeStart, DateTime? searchTimeEnd, bool isAdmin = false)
        {
            return JResult(WebService.Get_UserPageList(pageIndex, pageSize, name, type,areaId, searchTimeStart, searchTimeEnd, isAdmin));
        }


        public ActionResult ExportList(string name, string userId, int? type, int? areaId, DateTime? searchTimeStart, DateTime? searchTimeEnd)
        {
            var list = WebService.Get_UserPageList(1, 100000, name, type, areaId, searchTimeStart, searchTimeEnd, true).Result.List;
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            NPOIHelper<User>.GetExcel(list, GetChanelHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }
        private Hashtable GetChanelHT()
        {
            Hashtable hs = new Hashtable();
            hs["RealName"] = "真实名称";
            hs["IDCard"] = "身份证";
            hs["Phone"] = "手机号";
            hs["ProvoniceName"] = "省份";
            hs["CityName"] = "市";
            hs["CountyName"] = "县/区";
            hs["StreetName"] = "街道";
            hs["AllCount"] = "投稿数";
            hs["PassCount"] = "总采纳数";
            hs["CreatedTime"] = "注册日期";
            return hs;
        }
        


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_User(ids));
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_User(id));
        }

        [HttpPost]
        public ActionResult Add(User model)
        {

            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("Password");
            ModelState.Remove("DepartmentFlag");
            ModelState.Remove("OperateFlag");
            ModelState.Remove("IsSuperAdmin");
            ModelState.Remove("IsAdmin");
            if (ModelState.IsValid)
            {
                model.IsAdmin = true;
                var result = WebService.Add_User(model);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }
        [HttpPost]
        public ActionResult Update(User model)
        {

            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("Password");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("DepartmentFlag");
            ModelState.Remove("OperateFlag");
            ModelState.Remove("IsSuperAdmin");
            ModelState.Remove("IsAdmin");
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
    }
}