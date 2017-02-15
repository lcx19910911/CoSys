
using CoSys.Core;
using CoSys.Model;
using CoSys.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Service
{
    public partial class WebService
    {


        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> Admin_ChangePassword(string oldPassword, string newPassword, string cfmPassword)
        {
            try
            {
                if (oldPassword.IsNullOrEmpty() || newPassword.IsNullOrEmpty() || cfmPassword.IsNullOrEmpty())
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                if (!cfmPassword.Equals(newPassword))
                {
                    return Result(false, ErrorCode.user_password_notequal);

                }
                using (var db = new DbRepository())
                {
                    oldPassword = CryptoHelper.MD5_Encrypt(oldPassword);

                    var user = db.Admin.Where(x => x.ID.Equals(this.Client.LoginAdmin.ID)).FirstOrDefault();
                    if (user == null)
                        return Result(false, ErrorCode.user_not_exit);
                    if (!user.Password.Equals(oldPassword))
                        return Result(false, ErrorCode.user_password_nottrue);
                    newPassword = CryptoHelper.MD5_Encrypt(newPassword);
                    user.Password = newPassword;
                    Client.LoginAdmin = user;
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
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                return Result(false, ErrorCode.sys_fail);
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
        public WebResult<PageList<Admin>> Get_AdminPageList(int pageIndex, int pageSize, string name,string mobile, DateTime? startTimeStart, DateTime? endTimeEnd)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.Admin.AsQueryable().AsNoTracking().AsNoTracking().Where(x => !x.IsDelete && !x.ID.Equals(this.Client.LoginAdmin.ID));

                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }
                if (mobile.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Mobile.Contains(mobile));
                }
                if (startTimeStart != null)
                {
                    query = query.Where(x => x.CreatedTime >= startTimeStart);
                }
                if (endTimeEnd != null)
                {
                    endTimeEnd = endTimeEnd.Value.AddDays(1);
                    query = query.Where(x => x.CreatedTime < endTimeEnd);
                }
                
                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    //推荐人
                    //if (!string.IsNullOrEmpty(x.RoleID) && roleDic.ContainsKey(x.RoleID))
                    //    x.RoleName = roleDic[x.RoleID]?.Name;
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }


        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Admin(Admin model)
        {
            using (DbRepository db = new DbRepository())
            {
                if (db.Admin.AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile)&&!x.IsDelete).Any())
                    return Result(false, ErrorCode.datadatabase_mobile__had);
                if (db.Admin.AsQueryable().AsNoTracking().Where(x => x.Account.Equals(model.Account) &&!x.IsDelete).Any())
                    return Result(false, ErrorCode.user_name_already_exist);
                //var role = Cache_Get_RoleList().Where(x => x.ID.Equals(model.RoleID)).FirstOrDefault();
                //if(role==null)
                //    return Result(false, ErrorCode.datadatabase_primarykey_not_found);
                model.Password = CryptoHelper.MD5_Encrypt(model.ConfirmPassword);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                string menuIdStr = string.Empty;
                db.Admin.Add(model);
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
        public WebResult<bool> Update_Admin(Admin model)
        {
            using (DbRepository db = new DbRepository())
            {
                if (db.Admin.AsQueryable().AsNoTracking().Where(x => x.Mobile.Equals(model.Mobile) && !x.ID.Equals(model.ID) && !x.IsDelete).Any())
                    return Result(false, ErrorCode.datadatabase_mobile__had);
                var oldEntity = db.Admin.Find(model.ID);
                if (oldEntity != null)
                {
                    //var role = Cache_Get_RoleList().Where(x => x.ID.Equals(model.RoleID)).FirstOrDefault();
                    //if (role == null)
                    //    return Result(false, ErrorCode.datadatabase_primarykey_not_found);
                    oldEntity.Mobile = model.Mobile;
                    oldEntity.Name = model.Name;
                    oldEntity.Remark = model.Remark;
                    string menuIdStr = string.Empty;
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
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_AdminOperate(string ID, long OperateFlag)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.Admin.Find(ID);
                if (oldEntity != null)
                {
                    oldEntity.OperateFlag = OperateFlag;
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
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Admin(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                //找到实体
                db.Admin.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.IsDelete = true;
                });
                return db.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
            }
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Admin Find_Admin(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {
                var entity = db.Admin.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                return entity;
            }
        }
     

        /// <summary>
        /// 下拉框集合
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public List<SelectItem> Get_AdminSelectItem()
        {
            using (DbRepository db = new DbRepository())
            {
                List<SelectItem> list = new List<SelectItem>();

                var query = db.Admin.AsQueryable().Where(x => !x.IsDelete).AsNoTracking();
                query.OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                {
                    list.Add(new SelectItem()
                    {
                        Text = x.Name,
                        Value = x.ID
                    });
                });

                return list;

            }
        }

    }
}
