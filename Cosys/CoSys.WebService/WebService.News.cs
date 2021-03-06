﻿
using CoSys.Core;
using CoSys.Core.Model;
using CoSys.Model;
using CoSys.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CoSys.Service
{
    public partial class WebService
    {

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<News>> Get_NewsPageList(int pageIndex, int pageSize, string title, NewsState? state, int? type, int? areaId)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.News.AsQueryable().AsNoTracking();
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Title.Contains(title) || x.PenName.Contains(title));
                }

                var userDic = new Dictionary<string, string>();
                var adminDic = new Dictionary<string, string>();
                if (type == null)
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.UserID) && x.UserID.Equals(Client.LoginUser.ID));
                }
                else if (areaId != null)
                {
                    var userIdList = new List<string>();
                    if (type == 0)
                    {
                        userIdList = db.User.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode) && x.ProvoniceCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 1)
                    {
                        userIdList = db.User.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 2)
                    {
                        userIdList = db.User.Where(x => !string.IsNullOrEmpty(x.CountyCode) && x.CountyCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 3)
                    {
                        userIdList = db.User.Where(x => !string.IsNullOrEmpty(x.StreetCode) && x.StreetCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == -1)
                    {
                        userIdList = db.User.Where(x => string.IsNullOrEmpty(x.ProvoniceCode)).Select(x => x.ID).ToList();
                    }
                    query = query.Where(x => !string.IsNullOrEmpty(x.UserID) && userIdList.Contains(x.UserID));
                }
                if (state != null && (int)state != -1)
                {
                    query = query.Where(x => x.State == state);
                }
                var count = query.Count();
                if (type == null && areaId == null)
                {
                    query = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }
                var list = query.ToList();

                return ResultPageList(GetReturnList(list, db), pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<NewsHistory>> Get_NewsHistoryPageList(int pageIndex, int pageSize, string newsId)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.NewsHistory.AsQueryable().AsNoTracking();
                if (newsId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.NewsID==newsId);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (list != null && list.Count > 0)
                {
                    var userIdlist = list.Select(x => x.UpdaterID).ToList();
                    var userDic = db.User.Where(x => userIdlist.Contains(x.ID)).ToList().ToDictionary(x => x.ID);
                    list.ForEach(x =>
                    {
                        if (x.UpdaterID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UpdaterID))
                            x.UpdaterName = userDic[x.UpdaterID].RealName;
                    });
                }
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        public List<News> GetReturnList(List<News> list, DbRepository db)
        {
            var admin = Client.LoginAdmin;
            var role = new Role();
            if (admin != null)
            {
                role = db.Role.Find(admin.RoleID);
            }
            var ids = list.Select(x => x.UserID).ToList();
            ids.AddRange(list.Select(x => x.UpdateAdminID).ToList());

            var userDic = db.User.Where(x => ids.Contains(x.ID)).ToDictionary(x => x.ID);
            var roleDic = db.Role.ToDictionary(x => x.ID);
            var departmentDic = db.Department.ToDictionary(x => x.ID);
            var typeIdList = list.Select(x => x.NewsTypeID).ToList();
            var typeDic = db.DataDictionary.Where(x => x.GroupCode == GroupCode.Type && typeIdList.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Value);
            list.ForEach(x =>
            {
                if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                    x.UserName = userDic.GetValue(x.UserID).RealName;

                x.AuditStateStr = x.AuditState.GetDescription();
                if (admin != null)
                {
                    if (x.State == NewsState.Pass)
                    {

                        if (userDic.ContainsKey(x.UpdateAdminID))
                        {
                            var updateAdmin = userDic[x.UpdateAdminID];
                            if (roleDic.ContainsKey(updateAdmin.RoleID))
                            {
                                x.RoleName = roleDic[updateAdmin.RoleID].Name;
                                if (roleDic[updateAdmin.RoleID].AuditState == NewsAuditState.EditorialAudit)
                                {
                                    x.StateStr = x.State.GetDescription();
                                }
                                else if (roleDic[updateAdmin.RoleID].AuditState == NewsAuditState.LastAudit)
                                {
                                    x.StateStr = "编委会转稿";
                                }
                            }
                            else
                                x.StateStr = x.State.GetDescription();
                        }
                    }
                    else if (x.State == NewsState.Plush)
                    {
                        x.StateStr = x.State.GetDescription();
                        if (userDic.ContainsKey(x.UpdateAdminID))
                        {
                            var updateAdmin = userDic[x.UpdateAdminID];
                            if (roleDic.ContainsKey(updateAdmin.RoleID) && updateAdmin != null)
                                x.RoleName = roleDic[updateAdmin.RoleID].Name;
                        }
                    }
                    else if (x.State == NewsState.WaitAudit)
                    {
                        if (!admin.IsSuperAdmin)
                        {
                            if (x.AuditState == role.AuditState)
                            {

                                //判断是否被上级退回
                                if (x.UpdateAdminID.IsNullOrEmpty())
                                {
                                    x.StateStr = x.State.GetDescription();
                                    x.RoleName = "未审核";
                                }
                                else
                                {
                                    if (userDic.ContainsKey(x.UpdateAdminID))
                                    {
                                        var updateAdmin = userDic[x.UpdateAdminID];
                                        if (roleDic.ContainsKey(updateAdmin.RoleID))
                                        {
                                            x.RoleName = roleDic[updateAdmin.RoleID].Name;
                                            if (roleDic[updateAdmin.RoleID].AuditState.GetInt() > x.AuditState.GetInt())
                                            {
                                                x.StateStr = "退回";
                                            }
                                            else
                                                x.StateStr = x.State.GetDescription();
                                        }
                                        else
                                            x.StateStr = x.State.GetDescription();
                                    }
                                    else
                                    {
                                        x.StateStr = "未知";
                                    }
                                }
                            }
                            else if (x.AuditState.GetInt() > role.AuditState.GetInt())
                            {
                                if (x.UpdateAdminID.IsNotNullOrEmpty())
                                {
                                    if (x.UpdateAdminID.Equals(admin.ID))
                                    {
                                        var updateAdmin = userDic[x.UpdateAdminID];
                                        if (x.UpdateAdminID.Equals(admin.ID))
                                        {
                                            x.RoleName = roleDic[updateAdmin.RoleID].Name;
                                            if (roleDic[updateAdmin.RoleID].AuditState == NewsAuditState.EditorialAudit)
                                            {
                                                x.StateStr = "转稿给稿件审核员";
                                            }
                                            else
                                            {
                                                x.StateStr = "已审核";
                                            }
                                        }
                                        else
                                        {
                                            x.StateStr = "编委会转稿";
                                        }
                                    }
                                    else
                                    {
                                        x.StateStr = "已审核";
                                        x.RoleName = "已审核";
                                    }
                                }
                                else
                                {
                                    x.StateStr = x.State.GetDescription();
                                    x.RoleName = "未审核";
                                }
                            }
                            else
                            {
                                if (x.UpdateAdminID.IsNotNullOrEmpty())
                                {
                                    if (x.UpdateAdminID.Equals(admin.ID))
                                    {
                                        x.StateStr = "已退回";
                                        x.RoleName = "已审核";
                                    }
                                    else
                                    {
                                        var updateAdmin = userDic[x.UpdateAdminID];
                                        if (roleDic.ContainsKey(updateAdmin.RoleID))
                                        {
                                            x.RoleName = roleDic[updateAdmin.RoleID].Name;
                                            if (roleDic[updateAdmin.RoleID].AuditState.GetInt() > x.AuditState.GetInt())
                                            {
                                                x.StateStr = "退回";
                                            }
                                            else
                                                x.StateStr = x.State.GetDescription();
                                        }
                                        else
                                        {
                                            x.StateStr = x.State.GetDescription();
                                            x.RoleName = "未审核";
                                        }
                                    }
                                }
                                else
                                {
                                    x.StateStr = x.State.GetDescription();
                                    x.RoleName = "未审核";
                                }
                            }
                        }
                        else
                        {
                            x.StateStr = x.State.GetDescription();
                            if (x.UpdateAdminID.IsNotNullOrEmpty())
                            {
                                if (userDic.ContainsKey(x.UpdateAdminID))
                                {
                                    var updateAdmin = userDic[x.UpdateAdminID];
                                    if (roleDic.ContainsKey(updateAdmin.RoleID) && updateAdmin != null)
                                        x.RoleName = roleDic[updateAdmin.RoleID].Name;
                                }
                            }
                            else
                                x.RoleName = "未审核";
                        }
                    }
                    else
                    {
                        x.StateStr = x.State.GetDescription();
                        x.RoleName = "未审核";
                    }
                }
                else
                {
                    x.StateStr = x.State.GetDescription();
                }
                if (x.UpdateAdminID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UpdateAdminID))
                    x.UpdateAdminName = userDic.GetValue(x.UpdateAdminID).RealName;
                if (x.NewsTypeID.IsNotNullOrEmpty() && typeDic.ContainsKey(x.NewsTypeID))
                    x.NewsTypeName = typeDic[x.NewsTypeID];
                if (x.DepartmentID.IsNotNullOrEmpty())
                {
                    var departmentAry = x.DepartmentID.Split(';');
                    if (departmentAry.Length == 1)
                    {
                        if (departmentDic.ContainsKey(departmentAry[0]))
                        {
                            x.DepartmentName = departmentDic[departmentAry[0]].Name;
                        }
                    }
                    else if (departmentAry.Length == 2)
                    {
                        if (departmentDic.ContainsKey(departmentAry[0]))
                        {
                            x.DepartmentName = departmentDic[departmentAry[0]].Name;
                        }
                        if (departmentDic.ContainsKey(departmentAry[1]))
                        {
                            x.DepartmentName += "-" + departmentDic[departmentAry[1]].Name;
                        }
                    }
                }
            });
            return list;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<News>> Get_UserNewsPageList(int pageIndex, int pageSize, string title, string userId, NewsState? state, int? type, int? areaId, string name, DateTime? searchTimeStart, DateTime? searchTimeEnd, long methodFlag = 0, long departmentFlag = 0)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.News.AsQueryable().AsNoTracking();
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Title.Contains(title) || x.PenName.Contains(title));
                }
                if (userId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.UserID.Equals(userId));
                }
                var userDic = new Dictionary<string, string>();

                if (areaId != null && type != null)
                {
                    var userQUery = db.User.Where(x => !x.IsDelete);
                    if (name.IsNotNullOrEmpty())
                    {
                        userQUery = userQUery.Where(x => x.RealName.Contains(name));
                    }
                    if (searchTimeStart != null)
                    {
                        query = query.Where(x => x.CreatedTime >= searchTimeStart);
                    }
                    if (searchTimeEnd != null)
                    {
                        searchTimeEnd = searchTimeEnd.Value.AddDays(1);
                        query = query.Where(x => x.CreatedTime < searchTimeEnd);
                    }
                    if (methodFlag != -1)
                    {
                        query = query.Where(x => (x.MethodFlag&methodFlag)!=0);
                    }
                    if (departmentFlag != 0)
                    {
                        var departmentIdList = db.Department.Where(x => (x.Flag & departmentFlag) != 0).Select(x => (string.IsNullOrEmpty(x.ParentID) ? x.ID : x.ParentID + ";" + x.ID)).ToList();
                        query = query.Where(x => departmentIdList.Contains(x.DepartmentID));
                    }
                    var userIdList = new List<string>();
                    if (type == 0)
                    {
                        userIdList = userQUery.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode) && x.ProvoniceCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 1)
                    {
                        userIdList = userQUery.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 2)
                    {
                        userIdList = userQUery.Where(x => !string.IsNullOrEmpty(x.CountyCode) && x.CountyCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 3)
                    {
                        userIdList = userQUery.Where(x => !string.IsNullOrEmpty(x.StreetCode) &&x.StreetCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == -1)
                    {
                        userIdList = userQUery.Where(x => string.IsNullOrEmpty(x.ProvoniceCode)).Select(x => x.ID).ToList();
                    }
                    query = query.Where(x => !string.IsNullOrEmpty(x.UserID) && userIdList.Contains(x.UserID));
                }
                if (state != null && (int)state != -1)
                {
                    query = query.Where(x => x.State == state);
                }
                else
                {
                    if (Client.LoginAdmin != null && userId != Client.LoginAdmin.ID)
                    {
                        query = query.Where(x => x.State == NewsState.Pass || x.State == NewsState.WaitAudit || x.State == NewsState.Plush);
                    }
                }
                var count = query.Count();
                if (type == null && areaId == null)
                {
                    query = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                }
                var list = query.ToList();
                return ResultPageList(GetReturnList(list, db), pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<News>> Get_AdminNewsPageList(int pageIndex, int pageSize, string title, string userId,string ids, bool isAudit, NewsState? state, bool isExport = false)
        {
            using (DbRepository db = new DbRepository())
            {
                var admin = Client.LoginAdmin;
                //角色的审核权限
                var role = db.Role.Find(admin.RoleID);
                if (role == null && !admin.IsSuperAdmin)
                    return ResultPageList(new List<News>(), pageIndex, pageSize, 0);
                var query = db.News.AsQueryable().AsNoTracking();
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Title.Contains(title) || x.PenName.Contains(title));
                }
                if (userId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => (!string.IsNullOrEmpty(x.UserID) && x.UserID.Equals(userId)));
                }
                if (ids.IsNotNullOrEmpty())
                {
                    query = query.Where(x => ids.Contains(x.ID));
                }
                if (!isAudit)
                {
                    query = query.Where(x => x.UserID.Equals(admin.ID));
                }
                else
                {
                    //判断权限加载相对应的新闻
                    if (!admin.IsSuperAdmin)
                    {
                        var departmentIds = db.Department.Where(x => (admin.DepartmentFlag & x.Flag) != 0).Select(x => !string.IsNullOrEmpty(x.ParentID) ? (x.ParentID + ";" + x.ID) : x.ID).ToList();
                        query = query.Where(x => departmentIds.Contains(x.DepartmentID));
                        query = query.Where(x => (x.AuditState == role.AuditState));


                        //if (role.AuditState == NewsAuditState.EditorialAudit)
                        //{
                        //    query = query.Where(x => (x.AuditState == NewsAuditState.EditorAudit || x.AuditState == NewsAuditState.EditorialAudit || x.State == NewsState.WaitAudit || x.State == NewsState.Pass || x.State == NewsState.Plush));
                        //}
                        //if (role.AuditState == NewsAuditState.MinisterAudit || role.AuditState == NewsAuditState.LastAudit)
                        //{
                        //    query = query.Where(x => (x.AuditState == NewsAuditState.EditorAudit || x.AuditState == NewsAuditState.MinisterAudit || x.AuditState == NewsAuditState.LastAudit || x.State == NewsState.WaitAudit || x.State == NewsState.Pass || x.State == NewsState.Plush));
                        //}
                    }
                }
                if (state != null && (int)state != -1)
                {
                    query = query.Where(x => x.State == state);
                }
                else
                {
                    if (isExport)
                    {
                       // query = query.Where(x => x.State == NewsState.Pass || x.State == NewsState.Plush);
                    }
                    else
                    {
                        query = query.Where(x => x.State == NewsState.Pass || x.State == NewsState.Plush || x.State == NewsState.WaitAudit);
                    }
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(GetReturnList(list, db), pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_News(News model, bool isAudit, long departmentFlag)
        {
            var departmentList = new List<Department>();
            var parentIdList = new List<string>();
            using (DbRepository db = new DbRepository())
            {
                departmentList = db.Department.Where(x => !x.IsDelete && (departmentFlag & x.Flag) != 0).ToList();
                parentIdList = departmentList.Select(x => x.ParentID).ToList();
            }
            departmentList.Where(x => !parentIdList.Contains(x.ID)).ToList().ForEach(x =>
            {
                model.DepartmentID = x.ParentID.IsNotNullOrEmpty() ? $"{x.ParentID};{x.ID}" : x.ID;
                AddNews(model, isAudit);
            });

            return Result(true);

        }

        private void AddNews(News model, bool isAudit)
        {
            using (DbRepository db = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                if (Client.LoginUser != null)
                    model.UserID = Client.LoginUser.ID;
                else
                    model.UserID = Client.LoginAdmin.ID;
                if (!isAudit)
                    model.State = NewsState.None;
                else
                {
                    model.SubmitTime = DateTime.Now;
                    model.State = NewsState.WaitAudit;

                    if (Client.LoginAdmin != null && Client.LoginUser == null)
                    {
                        var role = db.Role.Find(Client.LoginAdmin.RoleID);
                        if (role != null)
                        {
                            var departmentList = db.Department.Where(x => !x.IsDelete && (Client.LoginAdmin.DepartmentFlag & x.Flag) != 0).Select(x => x.ID).ToList();
                            var newsDeparmentIdList = model.DepartmentID.Split(';');
                            if (newsDeparmentIdList.Count() == 2)
                            {
                                if (departmentList.Contains(newsDeparmentIdList[1]))
                                {
                                    if (role.AuditState != NewsAuditState.EditorAudit)
                                    {
                                        if (role.AuditState == NewsAuditState.EditorialAudit)
                                        {
                                            model.AuditState = NewsAuditState.LastAudit;
                                        }
                                        else
                                        {
                                            model.State = NewsState.Pass;
                                            model.AuditState = NewsAuditState.LastAudit;
                                            model.UpdateAdminID = Client.LoginAdmin.ID;
                                        }
                                    }
                                    else
                                    {
                                        model.AuditState = NewsAuditState.EditorialAudit;
                                    }
                                }
                                else
                                {
                                    model.AuditState = NewsAuditState.EditorAudit;
                                }
                            }
                            else
                            {
                                if (departmentList.Contains(newsDeparmentIdList[0]))
                                {
                                    if (role.AuditState != NewsAuditState.EditorAudit)
                                    {
                                        if (role.AuditState == NewsAuditState.LastAudit)
                                        {
                                            model.State = NewsState.Pass;
                                            model.UpdateAdminID = Client.LoginAdmin.ID;
                                            model.AuditState = NewsAuditState.LastAudit;
                                        }
                                        else
                                            model.AuditState = NewsAuditState.LastAudit;
                                    }
                                    else
                                    {
                                        model.AuditState = NewsAuditState.MinisterAudit;
                                    }
                                }
                                else
                                {
                                    model.AuditState = NewsAuditState.EditorAudit;
                                }
                            }
                        }
                    }
                    else
                    {
                        model.AuditState = NewsAuditState.EditorAudit;
                    }
                }
                db.News.Add(model);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> UserUpdate_News(News model, bool isAudit)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.News.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Title = model.Title;
                    oldEntity.PenName = model.PenName;
                    oldEntity.Content = model.Content;
                    oldEntity.MethodFlag = model.MethodFlag;
                    oldEntity.Paths = model.Paths;
                    oldEntity.NewsTypeID = model.NewsTypeID;
                    oldEntity.DepartmentID = model.DepartmentID;
                    oldEntity.Msg = model.Msg;
                    if (oldEntity.State == NewsState.Reject || oldEntity.State == NewsState.None)
                    {
                        if (isAudit)
                        {
                            oldEntity.State = NewsState.WaitAudit;
                            oldEntity.SubmitTime = DateTime.Now;
                            oldEntity.AuditState = NewsAuditState.EditorAudit;                           
                        }
                        else
                            oldEntity.State = NewsState.None;
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);


                if (db.SaveChanges() > 0)
                {
                }
                return Result(true);
            }

        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_News(News model, bool isAudit,string mark)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.News.Find(model.ID);
                if (oldEntity != null)
                {
                    if (mark.IsNotNullOrEmpty() && mark == "history")
                    {
                        db.NewsHistory.Add(new NewsHistory()
                        {
                            NewsID = model.ID,
                            Content = oldEntity.Content,
                            Title = oldEntity.Title,
                            UpdaterID=Client.LoginAdmin.ID
                        });
                    }
                    oldEntity.Title = model.Title;
                    oldEntity.PenName = model.PenName;
                    oldEntity.Content = model.Content;
                    oldEntity.MethodFlag = model.MethodFlag;
                    oldEntity.Paths = model.Paths;
                    oldEntity.NewsTypeID = model.NewsTypeID;
                    oldEntity.DepartmentID = model.DepartmentID;
                    oldEntity.Msg = model.Msg;
                    if (oldEntity.State == NewsState.Reject || oldEntity.State == NewsState.None)
                    {
                        if (isAudit)
                        {
                            oldEntity.State = NewsState.WaitAudit;
                            oldEntity.SubmitTime = DateTime.Now;
                            if (Client.LoginAdmin != null)
                            {
                                if (oldEntity.UserID.Equals(Client.LoginAdmin.ID))
                                {
                                    var role = db.Role.Find(Client.LoginAdmin.RoleID);
                                    if (role == null)
                                    {
                                        return Result(false, ErrorCode.sys_param_format_error);
                                    }
                                    var departmentList = db.Department.Where(x => !x.IsDelete && (Client.LoginAdmin.DepartmentFlag & x.Flag) != 0).Select(x => x.ID).ToList();
                                    var newsDeparmentIdList = model.DepartmentID.Split(';');
                                    if (newsDeparmentIdList.Count() == 2)
                                    {
                                        if (departmentList.Contains(newsDeparmentIdList[1]))
                                        {
                                            if (role.AuditState != NewsAuditState.EditorAudit)
                                            {
                                                if (role.AuditState == NewsAuditState.EditorialAudit)
                                                {
                                                    oldEntity.AuditState = NewsAuditState.LastAudit;
                                                }
                                                else
                                                {
                                                    oldEntity.State = NewsState.Pass;
                                                    oldEntity.AuditState = NewsAuditState.LastAudit;
                                                    oldEntity.UpdateAdminID = Client.LoginAdmin.ID;
                                                }

                                            }
                                            else
                                            {
                                                oldEntity.AuditState = NewsAuditState.EditorialAudit;
                                            }
                                        }
                                        else
                                        {
                                            oldEntity.AuditState = NewsAuditState.EditorAudit;
                                        }
                                    }
                                    else
                                    {

                                        if (departmentList.Contains(newsDeparmentIdList[0]))
                                        {

                                            if (role.AuditState != NewsAuditState.EditorAudit)
                                            {
                                                if (role.AuditState == NewsAuditState.MinisterAudit)
                                                {
                                                    oldEntity.AuditState = NewsAuditState.LastAudit;
                                                }
                                                else
                                                {
                                                    oldEntity.State = NewsState.Pass;
                                                    oldEntity.AuditState = NewsAuditState.LastAudit;
                                                    oldEntity.UpdateAdminID = Client.LoginAdmin.ID;
                                                }
                                            }
                                            else
                                            {
                                                oldEntity.AuditState = NewsAuditState.MinisterAudit;
                                            }
                                        }
                                        else
                                        {
                                            oldEntity.AuditState = NewsAuditState.EditorAudit;
                                        }
                                    }
                                }
                                else
                                {
                                    if (Client.LoginUser == null)
                                        oldEntity.UpdateAdminID = Client.LoginAdmin.ID;
                                }
                            }
                            else
                            {
                                oldEntity.AuditState = NewsAuditState.EditorAudit;
                            }
                        }
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);


                if (db.SaveChanges() > 0)
                {
                    var adminLogin = LoginHelper.GetCurrentAdmin();
                    if (adminLogin != null)
                    {
                        var admin = db.User.Find(adminLogin.ID);
                        admin.EditCount++;
                        db.SaveChanges();
                    }
                }
                return Result(true);
            }

        }


        public WebResult<bool> ReSet_News(string id)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.News.Find(id);
                if (oldEntity != null)
                {
                    if (Client.LoginUser == null)
                        return Result(false, ErrorCode.sys_param_format_error);
                    if (oldEntity.State == NewsState.WaitAudit || oldEntity.State == NewsState.Pass)
                    {
                        oldEntity.AuditState = NewsAuditState.EditorAudit;
                        oldEntity.State = NewsState.WaitAudit;
                        oldEntity.SubmitTime = DateTime.Now;
                        oldEntity.UpdateAdminID = string.Empty;
                    }
                    else
                        return Result(false, ErrorCode.sys_param_format_error);
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);


                if (db.SaveChanges() > 0)
                {
                    var adminLogin = LoginHelper.GetCurrentAdmin();
                    if (adminLogin != null)
                    {
                        var admin = db.User.Find(adminLogin.ID);
                        admin.EditCount++;
                        db.SaveChanges();
                    }
                }
                return Result(true);
            }
        }
        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public News Find_News(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {

                var model = db.News.Find(id);
                if (model != null)
                {
                    model.StateStr = model.State.GetDescription();
                    model.UserName = db.User.Find(model.UserID)?.RealName;
                    model.Logs = db.Log.Where(x => x.NewsID.Equals(id)).ToList();
                    var adminIdList = model.Logs.Select(x => x.AdminID).ToList();
                    var adminDic = db.User.Where(x => adminIdList.Contains(x.ID)).ToDictionary(x => x.ID, x => x.RealName);
                    model.Logs.ForEach(x =>
                    {
                        if (adminDic.ContainsKey(x.AdminID))
                            x.AdminName = adminDic.GetValue(x.AdminID);
                    });
                }
                return model;
            }
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<News> Get_UserNews()
        {
            using (DbRepository db = new DbRepository())
            {
                if (Client.LoginUser != null)
                    return db.News.Where(x => !string.IsNullOrEmpty(x.UserID) && x.UserID.Equals(Client.LoginUser.ID)).ToList();
                if (Client.LoginAdmin != null)
                    return db.News.Where(x => !string.IsNullOrEmpty(x.UserID) && x.UserID.Equals(Client.LoginAdmin.ID)).ToList();
                return new List<News>();
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_News(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                //找到实体
                db.News.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    db.News.Remove(x);
                });
                if (db.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> News_EditorialPass(string id, string msg)
        {
            if (!id.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                var news = db.News.Find(id);
                if (news == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                var admin = db.User.Find(LoginHelper.GetCurrentAdminID());
                if (news.State != NewsState.WaitAudit || admin == null || news.AuditState != NewsAuditState.EditorialAudit)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                else
                {
                    admin.AuditPassCount++;
                    admin.AuditCount++;
                    news.UpdateAdminID = admin.ID;
                    news.Msg = msg;
                    news.State = NewsState.Pass;
                    Add_Log(LogCode.AuditPass, id, Client.LoginAdmin.ID, msg);
                    if (db.SaveChanges() > 0)
                    {
                        return Result(true);
                    }
                    else
                    {
                        return Result(false, ErrorCode.sys_fail);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns> 
        public WebResult<bool> Audit_News(YesOrNoCode isPass, string id, string msg)
        {
            using (var db = new DbRepository())
            {
                var news = db.News.Find(id);
                if (news == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                var admin = db.User.Find(LoginHelper.GetCurrentAdminID());
                if ((news.State != NewsState.WaitAudit && news.State != NewsState.Pass) || admin == null)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                else
                {
                    var role = db.Role.Find(admin.RoleID);
                    if (role == null)
                        return Result(false, ErrorCode.sys_param_format_error);

                    if (news.State == NewsState.WaitAudit)
                    {

                        if (isPass == YesOrNoCode.Yes)
                            Add_Log(LogCode.AuditPass, id, Client.LoginAdmin.ID, msg);
                        else
                            Add_Log(LogCode.AuditFail, id, Client.LoginAdmin.ID, msg);
                        //主编审核
                        if (news.AuditState == NewsAuditState.EditorAudit)
                        {
                            if (role.AuditState == NewsAuditState.EditorAudit)
                            {
                                admin.AuditCount++;
                                news.UpdateAdminID = admin.ID;
                                if (isPass == YesOrNoCode.Yes)
                                {
                                    admin.AuditPassCount++;
                                    if (news.DepartmentID.Split(';').Length == 1)
                                        news.AuditState = NewsAuditState.MinisterAudit;
                                    else
                                    {
                                        news.AuditState = NewsAuditState.EditorialAudit;
                                    }
                                }
                                else
                                {
                                    news.Msg = msg;
                                    news.State = NewsState.Reject;
                                }
                            }
                            else
                                return Result(false, ErrorCode.sys_user_role_error);
                        }
                        //部长/编委会审核
                        else if (news.AuditState == NewsAuditState.MinisterAudit)
                        {
                            if (role.AuditState == NewsAuditState.MinisterAudit)
                            {
                                admin.AuditCount++;
                                news.UpdateAdminID = admin.ID;
                                if (isPass == YesOrNoCode.Yes)
                                {
                                    admin.AuditPassCount++;
                                    news.AuditState = NewsAuditState.LastAudit;
                                }
                                else
                                {
                                    news.Msg = msg;
                                    news.AuditState = NewsAuditState.EditorAudit;
                                }
                            }
                            else
                                return Result(false, ErrorCode.sys_user_role_error);
                        }
                        //编委会审核
                        else if (news.AuditState == NewsAuditState.EditorialAudit)
                        {
                            if (role.AuditState == NewsAuditState.EditorialAudit)
                            {
                                admin.AuditCount++;
                                news.UpdateAdminID = admin.ID;
                                if (isPass == YesOrNoCode.Yes)
                                {
                                    admin.AuditPassCount++;
                                    news.AuditState = NewsAuditState.LastAudit;
                                }
                                else
                                {
                                    news.Msg = msg;
                                    news.AuditState = NewsAuditState.EditorAudit;
                                }
                            }
                            else
                                return Result(false, ErrorCode.sys_user_role_error);
                        }//稿件审核员/领导审核
                        else if (news.AuditState == NewsAuditState.LastAudit)
                        {
                            if (role.AuditState == NewsAuditState.LastAudit)
                            {
                                admin.AuditCount++;
                                news.UpdateAdminID = admin.ID;
                                if (isPass == YesOrNoCode.Yes)
                                {
                                    admin.AuditPassCount++;
                                    news.State = NewsState.Pass;
                                    admin.AuditPassCount++;
                                }
                                else
                                {
                                    news.Msg = msg;
                                    if (news.DepartmentID.Split(';').Length == 2)
                                    {
                                        news.AuditState = NewsAuditState.EditorialAudit;
                                    }
                                    else
                                    {
                                        news.AuditState = NewsAuditState.MinisterAudit;
                                    }
                                }
                            }
                            else
                                return Result(false, ErrorCode.sys_user_role_error);
                        }
                    }
                    else if (news.State == NewsState.Pass)
                    {
                        news.State = NewsState.WaitAudit;
                        news.UpdateAdminID = admin.ID;
                        news.AuditState = NewsAuditState.LastAudit;
                    }
                }
                if (db.SaveChanges() > 0)
                {
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns> 
        public WebResult<bool> Plush_News(string id, string methodIDStr, string msg)
        {
            using (var db = new DbRepository())
            {
                var news = db.News.Find(id);
                if (news == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                var admin = db.User.Find(LoginHelper.GetCurrentAdminID());
                if ((news.State != NewsState.Pass && news.State != NewsState.Plush && news.State != NewsState.WaitAudit) || admin == null)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                admin.PlushCount++;

                news.UpdateAdminID = admin.ID;
                news.State = NewsState.Plush;
                news.PlushMethodIDStr = methodIDStr;
                //邮箱头盖
                var plushMethod = "";
                var typeNameDic = Cache_Get_DataDictionary()[GroupCode.Channel];
                var list = Cache_Get_DataDictionary()[GroupCode.Channel].Values.ToList();
                list.Where(x => methodIDStr.Contains(x.ID)).ToList().ForEach(x =>
                 {
                     plushMethod += " " + x.Value;
                     if (x.Remark.IsNotNullOrEmpty())
                     {
                         if (x.Remark.EndsWith("@id.com"))
                         {
                             var date = new {
                                 act = "add",
                                 catid = x.Remark.Replace("@id.com", "").GetInt(),
                                 dtime = DateTime.Now.Ticks,
                                 uptime = DateTime.Now.Ticks,
                                 title = news.Title,
                                 body = news.Content,
                                 author = news.PenName,
                                 catpath = "0086",
                                 xuhao = 0,
                                 cl = 0,
                                 tj = 0,
                                 iffb = 1,
                                 ifbold = 0,
                                 ifred = 0,
                                 type = "gif",
                                 source = "",
                                 memberid = 0,
                                 secure = 0,
                                 memo = "test12",
                                 prop1 = "",
                                 prop2 = "",
                                 prop3 = "",
                                 prop4 = "",
                                 prop5 = "",
                                 prop6 = "",
                                 prop7 = "",
                                 prop8 = "",
                                 prop9 = "",
                                 prop10 = "",
                                 prop11 = "",
                                 prop12 = "",
                                 prop13 = "",
                                 prop14 = "",
                                 prop15 = "",
                                 prop16 = "",
                                 prop17 = "",
                                 prop18 = "",
                                 prop19 = "",
                                 prop20 = "",
                                 downcentid = "1",
                                 downcent = "0",
                                 tourl = "",
                                 fileurl = "http://"
                             };
                             var getResult = WebHelper.GetPage("http://www.fjfpa.org.cn/api/publish.php?username=admin&password=windows", date.ToJson(), "post");
                         }
                         else
                         {
                             var result = WebHelper.SendMail(x.Remark, $"{CustomHelper.GetValue("Company_Email_Title")} 笔名:{news.PenName}", news.Content, news.Paths);
                         }
                     }
                 });
                Add_Log(LogCode.Plush, id, Client.LoginAdmin.ID, $"发布于{plushMethod}");
                if (db.SaveChanges() > 0)
                {

                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }
        }


        #region 统计

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public StatisticsTotal Get_NewsStatisticsArea(string name, int? province, DateTime? searchTimeStart, DateTime? searchTimeEnd, long methodFlag, bool isArea = true, long departmentFlag = 0)
        {

            StatisticsTotal returnModel = new StatisticsTotal();
            returnModel.List = new List<StatisticsModel>();
            using (DbRepository db = new DbRepository())
            {
                if (province != null && province != -1 && province != -2)
                {
                    var provinceName = "";
                    var cityList = new List<SelectItem>(); var countyList = new List<SelectItem>();
                    var userQuery = db.User.Where(x => !x.IsDelete);
                    var newsQuery = db.News.Where(x => !x.IsDelete && x.State != NewsState.None);
                    if (methodFlag != 0 && methodFlag != -1)
                        newsQuery = newsQuery.Where(x => (methodFlag & x.MethodFlag) != 0);
                    if (name.IsNotNullOrEmpty())
                    {
                        userQuery = userQuery.Where(x => x.RealName.Contains(name));
                    }
                    if (isArea)
                    {
                        if (searchTimeStart != null)
                        {
                            newsQuery = newsQuery.Where(x => x.CreatedTime >= searchTimeStart);
                        }
                        if (searchTimeEnd != null)
                        {
                            searchTimeEnd = searchTimeEnd.Value.AddDays(1);
                            newsQuery = newsQuery.Where(x => x.CreatedTime < searchTimeEnd);
                        }
                    }
                    else
                    {
                        if (searchTimeStart != null)
                        {
                            userQuery = userQuery.Where(x => x.CreatedTime >= searchTimeStart);
                        }
                        if (searchTimeEnd != null)
                        {
                            searchTimeEnd = searchTimeEnd.Value.AddDays(1);
                            userQuery = userQuery.Where(x => x.CreatedTime < searchTimeEnd);
                        }
                    }
                    if (departmentFlag != 0)
                    {
                        var departmentIdList = db.Department.Where(x => (x.Flag & departmentFlag) != 0).Select(x => (string.IsNullOrEmpty(x.ParentID) ? x.ID : x.ParentID + ";" + x.ID)).ToList();
                        newsQuery = newsQuery.Where(x => departmentIdList.Contains(x.DepartmentID));
                    }
                    var newsList = newsQuery.ToList();
                    var userList = userQuery.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode) && x.ProvoniceCode == province.ToString()).ToList();
                    returnModel.AllCount = newsList.Where(x => userList.Select(y => y.ID).Contains(x.UserID)).Count();
                    returnModel.PassCount = newsList.Where(x => userList.Select(y => y.ID).Contains(x.UserID) && (x.State == NewsState.Pass || x.State == NewsState.Plush)).Count();

                    provinceName = GetValue(GroupCode.Area, province.ToString());
                    cityList = Get_AreaList(province.ToString());
                    var cityIdList = cityList.Select(x => x.Value).ToList();
                    countyList = Cache_Get_DataDictionary()[GroupCode.Area].Values.Where(x => x.ParentKey.IsNotNullOrEmpty() && cityIdList.Contains(x.ParentKey.Trim())).ToList().Select(x => new SelectItem()
                    {
                        Value = x.Key,
                        Text = x.Value,
                        ParentKey = x.ParentKey
                    }).ToList();
                    var countyIdList = countyList.Select(x => x.Value).ToList();
                    var streetList = Cache_Get_DataDictionary()[GroupCode.Area].Values.Where(x => x.ParentKey.IsNotNullOrEmpty() && countyIdList.Contains(x.ParentKey.Trim())).ToList().Select(x => new SelectItem()
                    {
                        Value = x.Key,
                        Text = x.Value,
                        ParentKey = x.ParentKey
                    }).ToList();

                    returnModel.Name = provinceName;

                    var noUserIdList = userQuery.Where(y => string.IsNullOrEmpty(y.CityCode) && !string.IsNullOrEmpty(y.ProvoniceCode) && y.ProvoniceCode == province.ToString()).ToList();
                    returnModel.List.Add(new StatisticsModel()
                    {
                        CityName = "未选择地区",
                        Name = "未选择地区",
                        ProvinceName = provinceName,
                        AllCount = newsList.Where(y => noUserIdList.Select(z => z.ID).ToList().Contains(y.UserID)).Count(),
                        PassCount = newsList.Where(y => noUserIdList.Select(z => z.ID).ToList().Contains(y.UserID) && (y.State == NewsState.Pass || y.State == NewsState.Plush)).Count(),
                        PeopleCount = noUserIdList.Count,
                        UserLink = $"/user/index?type=0&areaId={province}",
                        NewsLink = $"/news/Statistics?type=0&areaId={province}",
                        Childrens = new List<StatisticsModel>()
                    });
                    cityList.ForEach(x =>
                    {
                        if (x.Value != "0")
                        {
                            var userIdList = userList.Where(y => y.CityCode.IsNotNullOrEmpty() && y.CityCode == x.Value).ToList();
                            var staticModel = new Core.StatisticsModel()
                            {
                                Name = x.Text,
                                ProvinceName = provinceName,
                                CityName = x.Text,
                                AllCount = newsList.Where(y => userIdList.Select(z => z.ID).ToList().Contains(y.UserID)).Count(),
                                PassCount = newsList.Where(y => userIdList.Select(z => z.ID).ToList().Contains(y.UserID) && (y.State == NewsState.Pass || y.State == NewsState.Plush)).Count(),
                                PeopleCount = userIdList.Count,
                                AreaId = x.Value,
                                UserLink = $"/user/index?type=1&areaId={x.Value}",
                                NewsLink = $"/news/Statistics?type=1&areaId={x.Value}",
                                Childrens = countyList.Where(z => z.ParentKey.Trim().Equals(x.Value) && z.Value != "0").ToList().Select(z =>
                                    {
                                        return new Core.StatisticsModel()
                                        {
                                            ProvinceName = provinceName,
                                            CityName = x.Text,
                                            CountyName = z.Text,
                                            Name = z.Text,
                                            AllCount = newsList.Where(u => userIdList.Where(t => t.CountyCode.IsNotNullOrEmpty() && t.CountyCode == z.Value).Select(i => i.ID).ToList().Contains(u.UserID)).Count(),
                                            PassCount = newsList.Where(u => userIdList.Where(t => t.CountyCode.IsNotNullOrEmpty() && t.CountyCode == z.Value).Select(i => i.ID).ToList().Contains(u.UserID) && (u.State == NewsState.Pass || u.State == NewsState.Plush)).Count(),
                                            PeopleCount = userIdList.Where(t => t.CountyCode.IsNotNullOrEmpty() && t.CountyCode == z.Value).Select(i => i.ID).Count(),
                                            UserLink = $"/user/index?type=2&areaId={z.Value}",
                                            NewsLink = $"/news/Statistics?type=2&areaId={z.Value}",
                                            AreaId = z.Value
                                        };
                                    }).ToList()
                            };
                            if (isArea)
                                staticModel.Childrens = staticModel.Childrens.Where(y => y.AllCount != 0).ToList();
                            else
                                staticModel.Childrens = staticModel.Childrens.Where(y => y.PeopleCount != 0).ToList();
                            if (staticModel.Childrens.Count > 0)
                            {
                                staticModel.Childrens.ForEach(y =>
                                {
                                    y.Childrens = streetList.Where(z => z.ParentKey.Trim().Equals(y.AreaId) && z.Value != "0").ToList().Select(z =>
                                     {
                                         return new Core.StatisticsModel()
                                         {
                                             ProvinceName = provinceName,
                                             CityName = x.Text,
                                             CountyName = y.Name,
                                             StreetName = z.Text,
                                             Name = z.Text,
                                             AllCount = newsList.Where(u => userIdList.Where(t => t.StreetCode.IsNotNullOrEmpty() && t.StreetCode == z.Value).Select(i => i.ID).ToList().Contains(u.UserID)).Count(),
                                             PassCount = newsList.Where(u => userIdList.Where(t => t.StreetCode.IsNotNullOrEmpty() && t.StreetCode == z.Value).Select(i => i.ID).ToList().Contains(u.UserID) && (u.State == NewsState.Pass || u.State == NewsState.Plush)).Count(),
                                             PeopleCount = userIdList.Where(t => t.StreetCode.IsNotNullOrEmpty() && t.StreetCode == z.Value).Select(i => i.ID).Count(),
                                             UserLink = $"/user/index?type=3&areaId={z.Value}",
                                             NewsLink = $"/news/Statistics?type=3&areaId={z.Value}",
                                             AreaId = z.Value
                                         };
                                     }).ToList();

                                    if (isArea)
                                        y.Childrens = y.Childrens.Where(z => z.AllCount != 0).ToList();
                                    else
                                        y.Childrens = y.Childrens.Where(z => z.PeopleCount != 0).ToList();
                                });
                            }
                            returnModel.List.Add(staticModel);

                        }
                    });
                }
                else if (province != null && province == -2)
                {
                    var provinceName = "未选择省份";
                    var userQuery = db.User.Where(x => !x.IsDelete);
                    var newsQuery = db.News.Where(x => !x.IsDelete && x.State != NewsState.None);
                    if (methodFlag != 0 && methodFlag != -1)
                        newsQuery = newsQuery.Where(x => (methodFlag & x.MethodFlag) != 0);
                    if (name.IsNotNullOrEmpty())
                    {
                        userQuery = userQuery.Where(x => x.RealName.Contains(name));
                    }
                    if (searchTimeStart != null)
                    {
                        newsQuery = newsQuery.Where(x => x.CreatedTime >= searchTimeStart);
                    }
                    if (searchTimeEnd != null)
                    {
                        searchTimeEnd = searchTimeEnd.Value.AddDays(1);
                        newsQuery = newsQuery.Where(x => x.CreatedTime < searchTimeEnd);
                    }
                    if (departmentFlag != 0)
                    {
                        var departmentIdList = db.Department.Where(x => (x.Flag & departmentFlag) != 0).Select(x => (string.IsNullOrEmpty(x.ParentID) ? x.ID : x.ParentID + ";" + x.ID)).ToList();
                        newsQuery = newsQuery.Where(x => departmentIdList.Contains(x.DepartmentID));
                    }
                    var newsList = newsQuery.ToList();
                    var userList = userQuery.Where(x => string.IsNullOrEmpty(x.ProvoniceCode)).ToList();
                    returnModel.AllCount = newsList.Where(x => userList.Select(y => y.ID).Contains(x.UserID)).Count();
                    returnModel.PassCount = newsList.Where(x => userList.Select(y => y.ID).Contains(x.UserID) && (x.State == NewsState.Pass || x.State == NewsState.Plush)).Count();
                    returnModel.Name = provinceName;
                    returnModel.List.Add(new StatisticsModel()
                    {
                        CityName = "未选择地区",
                        Name = "未选择地区",
                        ProvinceName = provinceName,
                        AllCount = returnModel.AllCount,
                        PassCount = returnModel.PassCount,
                        PeopleCount = userList.Count,
                        UserLink = $"/user/index?type=-1&areaId={province}",
                        NewsLink = $"/news/Statistics?type=-1&areaId={province}",
                        Childrens = new List<StatisticsModel>()
                    });
                }

                return returnModel;
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public List<StatisticsModel> Get_NewsStatisticsArea(int? province, int? city, int? county, long methodFlag)
        {
            List<StatisticsModel> model = new List<StatisticsModel>();
            using (DbRepository db = new DbRepository())
            {
                var query = db.User.AsQueryable().AsNoTracking();
                if (province != null && province != -1)
                {
                    var provinceName = GetValue(GroupCode.Area, province.ToString());
                    if (city != null && city != 0 && city != -1)
                    {
                        var cityName = GetValue(GroupCode.Area, city.ToString());
                        if (county != null && county != 0 && county != -1)
                        {
                            var countyName = GetValue(GroupCode.Area, county.ToString());
                            var userList = query.Where(x => !string.IsNullOrEmpty(x.CountyCode) && x.CountyCode == county.ToString()).Select(x => new SelectItem() { Value = x.StreetCode, Text = x.ID }).ToList();

                            var townUserDic = userList.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Text).ToList());
                            var townList = Get_AreaList(county.ToString());
                            townList.ForEach(x =>
                            {
                                if (townUserDic.ContainsKey(x.Value))
                                {
                                    var idList = townUserDic[x.Value];
                                    var newsList = db.News.Where(y => (!string.IsNullOrEmpty(y.UserID) && idList.Contains(y.UserID)) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                                    model.Add(new StatisticsModel()
                                    {
                                        Key = x.Text,
                                        AllCount = newsList.Count,
                                        Name = x.Text,
                                        PassCount = newsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                        AreaType = 3,
                                        AreaId = x.Value,
                                        ProvinceName = provinceName,
                                        CityName = cityName,
                                        CountyName = countyName
                                    });
                                }
                            });

                            var uidList = query.Where(x => string.IsNullOrEmpty(x.StreetCode) && !string.IsNullOrEmpty(x.CountyCode)).Select(x => x.ID).ToList();
                            var unewsList = db.News.Where(y => (!string.IsNullOrEmpty(y.UserID) && uidList.Contains(y.UserID)) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                            if (unewsList != null && unewsList.Count > 0)
                            {
                                model.Add(new StatisticsModel()
                                {
                                    Key = "未填写",
                                    AllCount = unewsList.Count,
                                    Name = "未填写",
                                    PassCount = unewsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                    AreaType = 4,
                                    AreaId = "",
                                    ProvinceName = provinceName,
                                    CityName = cityName,
                                    CountyName = countyName
                                });
                            }
                            return model;
                        }
                        else
                        {
                            var userList = query.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode == city.ToString()).Select(x => new SelectItem() { Value = x.CountyCode, Text = x.ID }).ToList();

                            var countyUserDic = userList.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Text).ToList());
                            var countyList = Get_AreaList(city.ToString());
                            countyList.ForEach(x =>
                            {
                                if (countyUserDic.ContainsKey(x.Value))
                                {
                                    var idList = countyUserDic[x.Value];
                                    var newsList = db.News.Where(y => !string.IsNullOrEmpty(y.UserID) && idList.Contains(y.UserID) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                                    model.Add(new StatisticsModel()
                                    {
                                        Key = x.Text,
                                        AllCount = newsList.Count,
                                        Name = x.Text,
                                        PassCount = newsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                        AreaType = 2,
                                        AreaId = x.Value,
                                        ProvinceName = provinceName,
                                        CityName = cityName,
                                    });
                                }
                            });

                            var uidList = query.Where(x => string.IsNullOrEmpty(x.CountyCode) && !string.IsNullOrEmpty(x.CityCode)).Select(x => x.ID).ToList();
                            var unewsList = db.News.Where(y => (!string.IsNullOrEmpty(y.UserID) && uidList.Contains(y.UserID)) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                            if (unewsList != null && unewsList.Count > 0)
                            {
                                model.Add(new StatisticsModel()
                                {
                                    Key = "未填写",
                                    AllCount = unewsList.Count,
                                    Name = "未填写",
                                    PassCount = unewsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                    AreaType = 4,
                                    AreaId = "",
                                    ProvinceName = provinceName,
                                    CityName = cityName,
                                });
                            }
                        }
                    }
                    else
                    {
                        var userList = query.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode) && x.ProvoniceCode == province.ToString()).Select(x => new SelectItem() { Value = x.CityCode, Text = x.ID }).ToList();

                        var cityUserDic = userList.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Text).ToList());
                        var cityList = Get_AreaList(province.ToString());
                        cityList.ForEach(x =>
                        {
                            if (cityUserDic.ContainsKey(x.Value))
                            {
                                var list = cityUserDic[x.Value];
                                var newsList = db.News.Where(y => !string.IsNullOrEmpty(y.UserID) && list.Contains(y.UserID) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                                model.Add(new StatisticsModel()
                                {
                                    Key = x.Text,
                                    AllCount = newsList.Count,
                                    Name = x.Text,
                                    PassCount = newsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                    AreaType = 1,
                                    AreaId = x.Value,
                                    ProvinceName = provinceName,
                                });
                            }
                        });

                        var uidList = query.Where(x => string.IsNullOrEmpty(x.CityCode) && !string.IsNullOrEmpty(x.ProvoniceCode)).Select(x => x.ID).ToList();
                        var unewsList = db.News.Where(y => (!string.IsNullOrEmpty(y.UserID) && uidList.Contains(y.UserID)) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                        if (unewsList != null && unewsList.Count > 0)
                        {
                            model.Add(new StatisticsModel()
                            {
                                Key = "未填写",
                                AllCount = unewsList.Count,
                                Name = "未填写",
                                PassCount = unewsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                AreaType = 4,
                                AreaId = "",
                                ProvinceName = provinceName,
                            });
                        }
                    }
                }
                else
                {
                    var userList = query.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode)).Select(x => new SelectItem() { Value = x.ProvoniceCode, Text = x.ID }).ToList();

                    var provinceUserDic = userList.GroupBy(x => x.Value).ToDictionary(x => x.Key, x => x.Select(y => y.Text).ToList());
                    var provinceList = Get_AreaList("");
                    provinceList.ForEach(x =>
                    {
                        if (provinceUserDic.ContainsKey(x.Value))
                        {
                            var list = provinceUserDic[x.Value];
                            var newsList = db.News.Where(y => !string.IsNullOrEmpty(y.UserID) && list.Contains(y.UserID) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                            model.Add(new StatisticsModel()
                            {
                                Key = x.Text,
                                AllCount = newsList.Count,
                                Name = x.Text,
                                PassCount = newsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                                AreaType = 0,
                                AreaId = x.Value
                            });
                        }
                    });

                    var uidList = query.Where(x => string.IsNullOrEmpty(x.ProvoniceCode)).Select(x => x.ID).ToList();
                    var unewsList = db.News.Where(y => (!string.IsNullOrEmpty(y.UserID) && uidList.Contains(y.UserID)) && (methodFlag == -1 ? 1 == 1 : (y.MethodFlag & methodFlag) != 0)).ToList();
                    if (unewsList != null && unewsList.Count > 0)
                    {
                        model.Add(new StatisticsModel()
                        {
                            Key = "未填写",
                            AllCount = unewsList.Count,
                            Name = "未填写",
                            PassCount = unewsList.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count(),
                            AreaType = 4,
                            AreaId = ""
                        });
                    }
                }
                return model;
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public List<StatisticsModel> Get_NewsStatisticsChanel()
        {
            List<StatisticsModel> model = new List<StatisticsModel>();
            using (DbRepository db = new DbRepository())
            {
                var dic = Cache_Get_DataDictionary()[GroupCode.Channel];
                dic.Values.ToList().ForEach(x =>
                {
                    var key = x.Key.GetLong();
                    model.Add(new StatisticsModel()
                    {
                        Key = x.Key,
                        AllCount = db.News.Where(y => y.PlushMethodIDStr.Contains(x.ID)).Select(y => y.ID).Count(),
                        Name = dic[x.Key].Value
                    });
                });
                return model;
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="title">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public List<StatisticsModel> Get_NewsStatisticsRegister(int? province, int? city, int? county)
        {
            List<StatisticsModel> model = new List<StatisticsModel>();
            using (DbRepository db = new DbRepository())
            {
                var query = db.User.AsQueryable().AsNoTracking();

                if (province != null && province != -1)
                {
                    if (city != null && city != 0)
                    {
                        if (county != null && county != 0)
                        {
                            query = query.Where(x => !string.IsNullOrEmpty(x.CountyCode) && x.CountyCode == county.ToString());
                            var townUserDic = query.GroupBy(x => x.StreetCode).ToDictionary(x => x.Key, x => x.Select(y => y.ID).ToList());
                            var townList = Get_AreaList(county.ToString());
                            townList.ForEach(x =>
                            {
                                if (townUserDic.ContainsKey(x.Value))
                                {
                                    model.Add(new StatisticsModel()
                                    {
                                        Key = x.Text,
                                        AllCount = townUserDic[x.Value].Count(),
                                        AreaType = 3,
                                        Name = x.Text,
                                        AreaId = x.Value
                                    });
                                }
                            });

                            return model;
                        }
                        else
                        {
                            query = query.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode == city.ToString());
                            var countyUserDic = query.GroupBy(x => x.CountyCode).ToDictionary(x => x.Key, x => x.Select(y => y.ID).ToList());
                            var countyList = Get_AreaList(city.ToString());
                            countyList.ForEach(x =>
                            {
                                if (countyUserDic.ContainsKey(x.Value))
                                {
                                    model.Add(new StatisticsModel()
                                    {
                                        Key = x.Text,
                                        AllCount = countyUserDic[x.Value].Count(),
                                        AreaType = 2,
                                        Name = x.Text,
                                        AreaId = x.Value
                                    });
                                }
                            });
                        }
                    }
                    else
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode) && x.ProvoniceCode == province.ToString());
                        var cityUserDic = query.GroupBy(x => x.CityCode).ToDictionary(x => x.Key, x => x.Select(y => y.ID).ToList());
                        var cityList = Get_AreaList(province.ToString());
                        cityList.ForEach(x =>
                        {
                            if (cityUserDic.ContainsKey(x.Value))
                            {
                                model.Add(new StatisticsModel()
                                {
                                    Key = x.Text,
                                    AllCount = cityUserDic[x.Value].Count(),
                                    AreaType = 1,
                                    Name = x.Text,
                                    AreaId = x.Value
                                });
                            }
                        });
                    }
                }
                else
                {
                    var provniceserDic = query.Where(x => !string.IsNullOrEmpty(x.ProvoniceCode)).GroupBy(x => x.ProvoniceCode).ToDictionary(x => x.Key, x => x.Select(y => y.ID).ToList());
                    var provniceList = Get_AreaList("");
                    provniceList.ForEach(x =>
                    {
                        if (provniceserDic.ContainsKey(x.Value))
                        {
                            model.Add(new StatisticsModel()
                            {
                                Key = x.Text,
                                AllCount = provniceserDic[x.Value].Count(),
                                AreaType = 0,
                                Name = x.Text,
                                AreaId = x.Value
                            });
                        }
                    });
                }
                return model;
            }
        }
        #endregion
    }
}
