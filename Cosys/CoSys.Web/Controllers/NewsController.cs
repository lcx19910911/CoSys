
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    //[LoginFilter]
    public class NewsController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Manage(string id)
        {
            var model = WebService.Find_News(id);
            if (model == null)
            {
                model = new News();
                model.ChildrenDepartmentList = new List<SelectItem>();
            }
            else
            {
                if (!model.UserID.Equals(Client.LoginUser.ID))
                    return View("Index");
                if (model.NewsDepartmentID.Split(',').Length == 2)
                {
                    model.ChildrenDepartmentList = WebService.Get_NewsDepartmentSelectItem(model.NewsDepartmentID.Split(',')[0]);
                }
            }

            model.DepartmentList = WebService.Get_NewsDepartmentSelectItem(null);
            model.TypeList = WebService.Get_DataDictorySelectItem(GroupCode.Type);
            return View(model);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult Add(News model,bool isAduit=true)
        {
            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("UserID");
            ModelState.Remove("MethodFlag");
            ModelState.Remove("State");
            
            if (ModelState.IsValid)
            {
                var result = WebService.Add_News(model, isAduit);
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
        [ValidateInput(false)]
        public JsonResult Update(News model, bool isAduit = true)
        {
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("State");
            ModelState.Remove("UserID");
            ModelState.Remove("MethodFlag");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_News(model, isAduit);
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
        public ActionResult GetPageList(int pageIndex, int pageSize, string title, string newsTypeId, YesOrNoCode? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            return JResult(WebService.Get_NewsPageList(pageIndex, pageSize, title, newsTypeId, state, createdTimeStart, createdTimeEnd));
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_News(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_News(ids));
        }
    }
}