using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    //[LoginFilter]
    public class DepartmentController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Add(Department entity)
        {
            ModelState.Remove("ID");
            ModelState.Remove("IsDelete");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_Department(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Update(Department entity)
        {
            ModelState.Remove("IsDelete");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_Department(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string name)
        {
            return JResult(WebService.Get_DepartmentPageList(pageIndex, pageSize, name));
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_Department(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_Department(ids));
        }


        /// <summary>
        /// 获取选择项
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem(string id)
        {
            return JResult(WebService.Get_DepartmentSelectItem(id));
        }

        /// <summary>
        /// 获取下拉框 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetZTreeChildren()
        {
            return JResult(WebService.Get_DepartmentZTreeChildren(null));
        }
    }
}