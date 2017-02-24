
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
        public WebResult<PageList<News>> Get_NewsPageList(int pageIndex, int pageSize, string title,string newsTypeId,bool isAudit,NewsState? state, DateTime? createdTimeStart, DateTime? createdTimeEnd)
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
                if (!isAudit)
                {

                    if (Client.LoginUser != null)
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.UserID) && x.UserID.Equals(Client.LoginUser.ID));
                    }
                    if (Client.LoginAdmin != null)
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.AdminID) && x.AdminID.Equals(Client.LoginAdmin.ID));
                    }
                }
                else
                {
                    if (Client.LoginAdmin != null)
                    {
                        query = query.Where(x =>!string.IsNullOrEmpty(x.AuditAdminIDs)&&x.AuditAdminIDs.Contains(Client.LoginAdmin.ID));
                    }
                }
                if (state != null)
                {
                    query = query.Where(x => x.State== state);
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
               

                if (Client.LoginAdmin != null)
                {
                    var ids = list.Select(x => x.UserID).ToList();
                    userDic = db.User.Where(x => ids.Contains(x.ID)).ToDictionary(x => x.ID,x=>x.RealName);
                    var adminIds = list.Select(x => x.AdminID).ToList();
                    adminDic = db.Admin.Where(x => adminIds.Contains(x.ID)).ToDictionary(x => x.ID, x => x.Name);
                }
                var departmentDic = db.Department.ToDictionary(x => x.ID);
                list.ForEach(x =>
                {
                    x.StateStr = x.State.GetDescription();
                    if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
                        x.UserName = userDic.GetValue(x.UserID);
                    else if (x.AdminID.IsNotNullOrEmpty() && adminDic.ContainsKey(x.AdminID))
                        x.UserName = adminDic.GetValue(x.AdminID);
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
                    model.UserID = Client.LoginAdmin.ID;
                if(isAudit)
                    model.State = NewsState.None;
                else
                    model.State = NewsState.WaitAudit;

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
                    if (oldEntity.State == NewsState.Reject)
                    {
                        oldEntity.AuditAdminIDs = string.Empty;
                        if (isAudit)
                            oldEntity.State = NewsState.WaitAudit;
                        else
                            oldEntity.State = NewsState.None;
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

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
                var News = db.News.Find(id);
                if (News == null)
                    return Result(false, ErrorCode.sys_param_format_error);

                if (News.State != NewsState.WaitAudit)
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                else
                {
                    News.State = isPass == YesOrNoCode.Yes ? NewsState.Pass : NewsState.Reject;

                }
                if(isPass == YesOrNoCode.Yes)
                    Add_Log(LogCode.AuditPass, id,Client.LoginAdmin.ID);
                else
                    Add_Log(LogCode.AuditFail, id, Client.LoginAdmin.ID);
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
