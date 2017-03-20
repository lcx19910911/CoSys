﻿
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoSys.Core;
using System.Collections;
using System.IO;

namespace CoSys.Web.Controllers
{
    [LoginFilter]
    public class NewsController : BaseController
    {

        public ActionResult Index()
        {
            if (Client.LoginUser==null)
                return RedirectToAction("Login", "Account");
            return View();
        }
        public ActionResult Admin()
        {
            if (Client.LoginAdmin == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult My()
        {
            if (Client.LoginAdmin == null)
                return RedirectToAction("Login", "Account");
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
                var user = Client.LoginUser;
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
                if (user == null)
                    return RedirectToAction("Login", "Account");
                if (!model.UserID.Equals(user.ID))
                    return View("Index");
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
                model.Logs = WebService.Get_LogByNewsId(model.ID);
            }

            model.TypeList = WebService.Get_DataDictorySelectItem(GroupCode.Type);
            model.MethodList= WebService.Get_DataDictorySelectItem(GroupCode.Channel); 
            return View(model);
        }

        public ActionResult AdminManage(string id)
        {
            var model = WebService.Find_News(id);
            ViewBag.CanEdit = false;;
            if (model == null)
            {
                model = new News();
                model.ChildrenDepartmentList = new List<SelectItem>();
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
                if (model.DepartmentList != null && model.DepartmentList.Count > 0)
                    model.ChildrenDepartmentList = WebService.Get_DepartmentSelectItem(model.DepartmentList[0].Value);
            }
            else
            {
                var admin = Client.LoginAdmin;
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);

                if (admin == null)
                    return RedirectToAction("Login", "Account");
                var department = new Department();
                if (model.DepartmentID.Split(';').Length == 1)
                {
                    department = WebService.Find_Department(model.DepartmentID);
                }
                else if (model.DepartmentID.Split(';').Length == 2)
                {
                    model.ChildrenDepartmentList = WebService.Get_DepartmentSelectItem(model.DepartmentID.Split(';')[0]);
                    department = WebService.Find_Department(model.DepartmentID.Split(';')[1]);
                }

                if (department == null)
                    return View("Admin");
                model.Logs = WebService.Get_LogByNewsId(model.ID);
                if ((model.State==NewsState.None||model.State==NewsState.Reject)&&!model.UserID.Equals(admin.ID))
                    return View("Admin");
                if (!admin.IsSuperAdmin)
                {
                    //判断角色
                    var role = WebService.Find_Role(admin.RoleID);
                    if (role == null)
                    {
                        return View("Admin");
                    }
                    //审核中
                    if (model.State == NewsState.WaitAudit)
                    {
                        //判断投递部门权限
                        if ((department.Flag & admin.DepartmentFlag) == 0 && !admin.IsSuperAdmin)
                        {
                            return View("Admin");
                        }
                        //是否审核
                        if (role.AuditState != model.AuditState)
                        {
                            ViewBag.CanEdit = false;
                        }
                        else
                        {
                            ViewBag.CanEdit = true;
                        }
                    }
                }
                else
                {
                    ViewBag.CanEdit = true;
                }

            }

            model.TypeList = WebService.Get_DataDictorySelectItem(GroupCode.Type);
            model.MethodList = WebService.Get_DataDictorySelectItem(GroupCode.Channel);
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


        public ActionResult Statistics()
        {
            return View();
        }

        public ActionResult ExportStatistics(string title,string userId, NewsState? state, int? type, int? areaId)
        {
            var list = WebService.Get_UserNewsPageList(1, 100000, title, userId, state, type,areaId).Result.List;
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            NPOIHelper<News>.GetExcel(list, GetChanelHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }
        private Hashtable GetChanelHT()
        {
            Hashtable hs = new Hashtable();
            hs["Title"] = "标题";
            hs["SubmitTime"] = "投递时间"; 
            hs["StateStr"] = "状态";
            hs["UserName"] = "作者";
            return hs;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string title,NewsState? state,int? type,int? areaId)
        {
            return JResult(WebService.Get_NewsPageList(pageIndex, pageSize, title, state, type,areaId));
        }



        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetAdminPageList(int pageIndex, int pageSize, string title,string userId, NewsState? state, bool isAudit=true)
        {
            return JResult(WebService.Get_AdminNewsPageList(pageIndex, pageSize, title, userId, isAudit, state));
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetUserPageList(int pageIndex, int pageSize, string title, string userId, NewsState? state, int? type, int? areaId)
        {
            return JResult(WebService.Get_UserNewsPageList(pageIndex, pageSize, title, userId, state,type,areaId));
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
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Reset(string id)
        {
            return JResult(WebService.ReSet_News(id));
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
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Plush(string id, long flag, string msg)
        {
            return JResult(WebService.Plush_News(id, flag, msg));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ActionResult EditorialPass(string id, string msg)
        {
            return JResult(WebService.News_EditorialPass(id, msg));           
        }
    }
}