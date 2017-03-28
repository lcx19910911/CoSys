
using CoSys.Core;
using CoSys.Model;
using CoSys.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        userIdList = db.User.Where(x => x.ProvoniceCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 1)
                    {
                        userIdList = db.User.Where(x => x.CountyCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 2)
                    {
                        userIdList = db.User.Where(x => x.CountyCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 3)
                    {
                        userIdList = db.User.Where(x => x.StreetCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
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
                                        x.StateStr = "已审核";
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
                                        x.StateStr = "上一节点未审核";
                                        x.RoleName = "未审核";
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
        public WebResult<PageList<News>> Get_UserNewsPageList(int pageIndex, int pageSize, string title, string userId, NewsState? state, int? type, int? areaId)
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
                    var userIdList = new List<string>();
                    if (type == 0)
                    {
                        userIdList = db.User.Where(x => x.ProvoniceCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 1)
                    {
                        userIdList = db.User.Where(x => x.CountyCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 2)
                    {
                        userIdList = db.User.Where(x => x.CountyCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
                    }
                    else if (type == 3)
                    {
                        userIdList = db.User.Where(x => x.StreetCode.Equals(areaId.ToString())).Select(x => x.ID).ToList();
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
        public WebResult<PageList<News>> Get_AdminNewsPageList(int pageIndex, int pageSize, string title, string userId, bool isAudit, NewsState? state)
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
                        if (role.AuditState == NewsAuditState.EditorialAudit)
                        {
                            query = query.Where(x => (x.AuditState == NewsAuditState.EditorAudit || x.AuditState == NewsAuditState.EditorialAudit || x.State == NewsState.WaitAudit || x.State == NewsState.Pass || x.State == NewsState.Plush));
                        }
                        if (role.AuditState == NewsAuditState.MinisterAudit || role.AuditState == NewsAuditState.LastAudit)
                        {
                            query = query.Where(x => (x.AuditState == NewsAuditState.EditorAudit || x.AuditState == NewsAuditState.MinisterAudit || x.AuditState == NewsAuditState.LastAudit || x.State == NewsState.WaitAudit || x.State == NewsState.Pass || x.State == NewsState.Plush));
                        }
                    }
                }
                if (state != null && (int)state != -1)
                {
                    query = query.Where(x => x.State == state);
                }
                else
                {
                    query = query.Where(x => x.State == NewsState.WaitAudit || x.State == NewsState.Pass || x.State == NewsState.Plush);
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
        public WebResult<bool> Add_News(News model, bool isAudit)
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

                    if (Client.LoginAdmin != null&&Client.LoginUser==null)
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
                    else
                    {
                        model.AuditState = NewsAuditState.EditorAudit;
                    }
                }
                db.News.Add(model);
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
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_News(News model, bool isAudit)
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
                                    if(Client.LoginUser==null)
                                        oldEntity.UpdateAdminID = Client.LoginAdmin.ID;
                                }
                            }
                            else
                            {
                                oldEntity.AuditState = NewsAuditState.EditorAudit;
                            }
                        }
                        else
                            oldEntity.State = NewsState.None;
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
                                    news.AuditState = NewsAuditState.MinisterAudit;
                                }
                            }
                            else
                                return Result(false, ErrorCode.sys_user_role_error);
                        }
                    }
                    else if (news.State == NewsState.Pass)
                    {
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
        public WebResult<bool> Plush_News(string id, long channelFlag, string msg)
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
                news.PlushMethodFlag = channelFlag;
                //邮箱头盖
                Cache_Get_DataDictionary()[GroupCode.Channel].Values.Where(x => (x.Key.GetLong() & channelFlag) != 0 && x.Remark.IsNotNullOrEmpty()).ToList().ForEach(x =>
                  {
                      var result = WebHelper.SendMail(x.Remark, $"{CustomHelper.GetValue("Company_Email_Title")} 笔名:{news.PenName}", news.Content, "");
                  });
                Add_Log(LogCode.Plush, id, Client.LoginAdmin.ID, msg);
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
                var dic = Cache_Get_DataDictionary()[GroupCode.Type];
                db.News.AsQueryable().AsNoTracking().GroupBy(x => x.NewsTypeID).ToList().ForEach(x =>
                  {
                      model.Add(new StatisticsModel()
                      {
                          Key = x.Key,
                          AllCount = x.Count(),
                          Name = dic[x.Key].Value,
                          PassCount = x.Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count()
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
