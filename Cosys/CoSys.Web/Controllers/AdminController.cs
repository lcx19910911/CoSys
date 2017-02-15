using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    /// <summary>
    /// 管理员管理
    /// </summary>
    [LoginFilter]
    public class AdminController : BaseController
    {
        // GET: 
        public ActionResult Index()
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
        public ActionResult GetPageList(int pageIndex, int pageSize, string name, string mobile, DateTime? startTimeStart, DateTime? endTimeEnd)
        {
            return JResult(WebService.Get_AdminPageList(pageIndex, pageSize, name, mobile, startTimeStart, endTimeEnd));
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(CoSys.Model.Admin model)
        {
            ModelState.Remove("ID");
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("UserId");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_Admin(model);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <returns></returns>
        public ActionResult Update(CoSys.Model.Admin model)
        {
            ModelState.Remove("UpdaterID");
            ModelState.Remove("UpdatedTime");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("NewPassword");
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_Admin(model);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

  

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_Admin(ids));
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_Admin(id));
        }
        

        /// <summary>
        /// 获取用户选择项
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem()
        {
            return JResult(WebService.Get_AdminSelectItem());
        }
    }
}