﻿
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoSys.Core;

namespace CoSys.Web.Controllers
{
    [LoginFilter]
    public class NewsController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }
        public ViewResult Admin()
        {
            return View();
        }

        public ActionResult Manage(string id)
        {
            var model = WebService.Find_News(id);
            if (model == null)
            {
                model = new News();
                model.ChildrenDepartmentList = new List<SelectItem>();
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
                if(model.DepartmentList!=null&& model.DepartmentList.Count>0)
                model.ChildrenDepartmentList = WebService.Get_DepartmentSelectItem(model.DepartmentList[0].Value);
            }
            else
            {
                var admin = Client.LoginAdmin;
                var user = Client.LoginUser;
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
                if (model.UserID.IsNotNullOrEmpty())
                {
                    if (user!=null&&!model.UserID.Equals(user.ID))
                        return View("Index");
                }
                if (model.AdminID.IsNotNullOrEmpty())
                {
                    if (admin == null)
                        return RedirectToAction("Login", "Account");
                    if (!model.AdminID.Equals(admin.ID))
                        return View("Index");
                }
                var department = new Department();
                if (model.DepartmentID.Split(';').Length == 1)
                {
                    department = WebService.Find_Department(model.DepartmentID);
                }
                 else  if (model.DepartmentID.Split(';').Length == 2)
                {
                    model.ChildrenDepartmentList = WebService.Get_DepartmentSelectItem(model.DepartmentID.Split(';')[0]);
                    department = WebService.Find_Department(model.DepartmentID.Split(';')[1]);
                }

                if(department==null)
                    return View("Index");

                if (user != null)
                {
                    if (user.ID != model.UserID)
                    {
                        return View("Index");
                    }
                }
                if (admin != null)
                {
                    if (!admin.IsSuperAdmin)
                    {
                        //判断角色
                        var role = WebService.Find_Role(admin.RoleID);
                        if (role == null)
                        {
                            return View("Index");
                        }
                        //审核中
                        if (model.State == NewsState.WaitAudit)
                        {
                            //是否审核
                            if (role.AuditState != model.AuditState)
                            {
                                return View("Index");
                            }
                            //判断投递部门权限
                            if ((department.Flag & admin.DepartmentFlag) == 0 && !admin.IsSuperAdmin)
                            {
                                return View("Index");
                            }
                        }
                    }
                }

            }

            model.TypeList = WebService.Get_DataDictorySelectItem(GroupCode.Type);
            model.MethodList= WebService.Get_DataDictorySelectItem(GroupCode.Channel); 
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
        public ActionResult GetPageList(int pageIndex, int pageSize, string title,string newsTypeId,NewsState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            return JResult(WebService.Get_NewsPageList(pageIndex, pageSize, title, newsTypeId, false, state, createdTimeStart, createdTimeEnd));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetAdminPageList(int pageIndex, int pageSize, string title, string newsTypeId, NewsState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            return JResult(WebService.Get_NewsPageList(pageIndex, pageSize, title, newsTypeId, true, state, createdTimeStart, createdTimeEnd));
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
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Audit(YesOrNoCode isPass, string id, string msg)
        {
            return JResult(WebService.Audit_News(isPass, id,msg));
        }

        
    }
}