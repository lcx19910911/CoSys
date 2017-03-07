﻿
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
        public WebResult<PageList<News>> Get_NewsPageList(int pageIndex, int pageSize, string title,string newsTypeId,NewsState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.News.AsQueryable().AsNoTracking();
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Title.Contains(title));
                }
                if (newsTypeId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.NewsTypeID.Equals(newsTypeId));
                }
                if (createdTimeStart != null)
                {
                    query = query.Where(x => x.CreatedTime >= createdTimeStart);
                }
                if (createdTimeEnd != null)
                {
                    createdTimeEnd = createdTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < createdTimeEnd);
                }

                var userDic = new Dictionary<string, string>();
                var adminDic = new Dictionary<string, string>();
                query = query.Where(x => !string.IsNullOrEmpty(x.UserID) && x.UserID.Equals(Client.LoginUser.ID));
                if (state != null)
                {
                    query = query.Where(x => x.State== state);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                var ids = list.Select(x => x.UserID).ToList();
                userDic = db.User.Where(x => ids.Contains(x.ID)).ToDictionary(x => x.ID,x=>x.RealName);                 
                var adminIds = list.Select(x => x.AdminID).ToList();
                adminIds.AddRange(list.Select(x => x.UpdateAdminID).ToList());
                adminIds.AddRange(list.Select(x => x.UserID).ToList());
                adminDic = db.Admin.Where(x => adminIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
                var departmentDic = db.Department.ToDictionary(x => x.ID);
                list.ForEach(x =>
                {
                    x.StateStr = x.State.GetDescription();
                    if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                        x.UserName = userDic.GetValue(x.UserID);
                    else if (x.UserID.IsNotNullOrEmpty() && adminDic.ContainsKey(x.UserID))
                        x.UserName = adminDic.GetValue(x.UserID);
                    if (x.UpdateAdminID.IsNotNullOrEmpty() && adminDic.ContainsKey(x.UpdateAdminID))
                        x.UpdateAdminName = adminDic.GetValue(x.UpdateAdminID);
                    if (x.NewsTypeID.IsNotNullOrEmpty())
                        x.NewsTypeName = GetValue(GroupCode.Type, x.NewsTypeID);
                    if (x.DepartmentID.IsNotNullOrEmpty())
                    {
                        var departmentAry = x.DepartmentID.Split(':');
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
                                x.DepartmentName +="-"+ departmentDic[departmentAry[1]].Name;
                            }
                        }
                    }

                });
                return ResultPageList(list, pageIndex, pageSize, count);
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
        public WebResult<PageList<News>> Get_AdminNewsPageList(int pageIndex, int pageSize, string title, string newsTypeId, bool isAudit,NewsState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
        {
            using (DbRepository db = new DbRepository())
            {
                var admin = Client.LoginAdmin;
                //角色的审核权限
                var role = db.Role.Find(admin.RoleID);
                if (role == null&& !admin.IsSuperAdmin)
                    return ResultPageList(new List<News>(), pageIndex, pageSize, 0);
                var query = db.News.AsQueryable().AsNoTracking();
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Title.Contains(title));
                }
                if (newsTypeId.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.NewsTypeID.Equals(newsTypeId));
                }
                if (createdTimeStart != null)
                {
                    query = query.Where(x => x.CreatedTime >= createdTimeStart);
                }
                if (createdTimeEnd != null)
                {
                    createdTimeEnd = createdTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < createdTimeEnd);
                }

                var userDic = new Dictionary<string, string>();
                var adminDic = new Dictionary<string, string>();
                if (!isAudit)
                {
                    query = query.Where(x => !string.IsNullOrEmpty(x.AdminID) && x.AdminID.Equals(admin.ID));
                }
                else
                {
                    //判断权限加载相对应的新闻
                    if (!admin.IsSuperAdmin)
                    {
                        var departmentIds = db.Department.Where(x => (admin.DepartmentFlag & x.Flag) != 0).Select(x =>!string.IsNullOrEmpty(x.ParentID)?(x.ParentID + ";" + x.ID):x.ID).ToList();
                        query = query.Where(x => departmentIds.Contains(x.DepartmentID));
                        if (role.AuditState == NewsAuditState.EditorialAudit)
                        {
                            query = query.Where(x => (x.AuditState == NewsAuditState.EditorAudit || x.AuditState == NewsAuditState.EditorialAudit));
                        }
                        if (role.AuditState == NewsAuditState.MinisterAudit||role.AuditState == NewsAuditState.LastAudit)
                        {
                            query = query.Where(x => (x.AuditState == NewsAuditState.EditorAudit || x.AuditState == NewsAuditState.MinisterAudit || x.AuditState == NewsAuditState.LastAudit));
                        }
                    }
                }
                if (state != null)
                {
                    query = query.Where(x => x.State == state);
                }
                else
                {
                    query = query.Where(x => x.State == NewsState.WaitAudit|| x.State == NewsState.Pass || x.State == NewsState.Plush);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var ids = list.Select(x => x.UserID).ToList();
                userDic = db.User.Where(x => ids.Contains(x.ID)).ToDictionary(x => x.ID, x => x.RealName);
                var adminIds = list.Select(x => x.UpdateAdminID).ToList();
                adminIds.AddRange(list.Select(x => x.AdminID).ToList());
                adminDic = db.Admin.Where(x => adminIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
                var departmentDic = db.Department.ToDictionary(x => x.ID);
                var typeIdList = list.Select(x => x.NewsTypeID).ToList();
                var typeDic = db.DataDictionary.Where(x => x.GroupCode == GroupCode.Type && typeIdList.Contains(x.ID)).ToDictionary(x=>x.ID,x=>x.Value);
                list.ForEach(x =>
                {
                    if (!admin.IsSuperAdmin)
                    {
                        if (x.AuditState == role.AuditState)
                        {
                            x.StateStr = x.State.GetDescription();
                        }
                        else if (x.AuditState.GetInt() > role.AuditState.GetInt())
                        {
                            x.StateStr = "已审核";
                        }
                        else
                        {
                            x.StateStr = "上节点未审核";
                        }
                        }
                    else
                    {
                        x.StateStr = x.State.GetDescription();
                    }
                    if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                        x.UserName = userDic.GetValue(x.UserID);
                    else if (x.AdminID.IsNotNullOrEmpty() && adminDic.ContainsKey(x.AdminID))
                        x.UserName = adminDic.GetValue(x.AdminID);
                    if (x.UpdateAdminID.IsNotNullOrEmpty() && adminDic.ContainsKey(x.UpdateAdminID))
                        x.UpdateAdminName = adminDic.GetValue(x.UpdateAdminID);
                    if (x.NewsTypeID.IsNotNullOrEmpty()&& typeDic.ContainsKey(x.NewsTypeID))
                       x.NewsTypeName = typeDic[x.NewsTypeID];
                    if (admin != null)
                    {
                        if (x.DepartmentID.IsNotNullOrEmpty())
                        {
                            var departmentAry = x.DepartmentID.Split(':');
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
                    }

                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_News(News model,bool isAudit)
        {
            using (DbRepository db = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                if (Client.LoginUser != null)
                    model.UserID = Client.LoginUser.ID;
                else
                    model.AdminID = Client.LoginAdmin.ID;
                if (!isAudit)
                    model.State = NewsState.None;
                else
                {
                    model.State = NewsState.WaitAudit;
                    model.AuditState = NewsAuditState.EditorAudit;
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
        public WebResult<bool> Update_News(News model,bool isAudit)
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
                    if(Client.LoginUser==null)
                        oldEntity.UpdateAdminID = Client.LoginAdmin.ID;
                    if (oldEntity.State == NewsState.Reject|| oldEntity.State == NewsState.None)
                    {
                        if (isAudit)
                            oldEntity.State = NewsState.WaitAudit;
                        else
                            oldEntity.State = NewsState.None;
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                db.SaveChanges();
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
                    var adminDic = db.Admin.Where(x => adminIdList.Contains(x.ID)).ToDictionary(x => x.ID,x=>x.Name);
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
                if(Client.LoginUser!=null)
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

                var admin = Client.LoginAdmin;
                if (news.State != NewsState.WaitAudit|| admin==null)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                else
                {
                    var role = db.Role.Find(admin.RoleID);
                    if (role == null)
                        return Result(false, ErrorCode.sys_param_format_error);
                    

                    //主编审核
                   if(news.AuditState==NewsAuditState.EditorAudit)
                   { 
                        if(role.AuditState==NewsAuditState.EditorAudit)
                        {
                            news.UpdateAdminID = admin.ID;
                            if (isPass == YesOrNoCode.Yes)
                            {
                                if(news.DepartmentID.Split(';').Length==1)
                                    news.AuditState = NewsAuditState.MinisterAudit;
                                else
                                    news.AuditState = NewsAuditState.EditorialAudit;
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
                            news.UpdateAdminID = admin.ID;
                            if (isPass == YesOrNoCode.Yes)
                            {
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
                            news.UpdateAdminID = admin.ID;
                            if (isPass == YesOrNoCode.Yes)
                            {
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
                            news.UpdateAdminID = admin.ID;
                            if (isPass == YesOrNoCode.Yes)
                            {
                                news.State = NewsState.Pass;
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
                if(isPass == YesOrNoCode.Yes)
                    Add_Log(LogCode.AuditPass, id,Client.LoginAdmin.ID,msg);
                else
                    Add_Log(LogCode.AuditFail, id, Client.LoginAdmin.ID, msg);
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

                var admin = Client.LoginAdmin;
                if (news.State != NewsState.Pass|| admin == null)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }

                news.UpdateAdminID = admin.ID;
                news.State = NewsState.Plush;
                news.PlushMethodFlag = channelFlag;
                //邮箱头盖
                Cache_Get_DataDictionary()[GroupCode.Channel].Values.Where(x => (x.Key.GetLong() & channelFlag) != 0&&x.Remark.IsNotNullOrEmpty()).ToList().ForEach(x =>
                {
                    var result = WebHelper.SendMail(x.Remark,$"{CustomHelper.GetValue("Company_Email_Title")} 笔名:{news.PenName}" , news.Content, "");
                });     
                Add_Log(LogCode.Plush, id, Client.LoginAdmin.ID,msg);
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
}
