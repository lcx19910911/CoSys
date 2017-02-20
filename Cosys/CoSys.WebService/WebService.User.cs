
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
using System.Web;

namespace CoSys.Service
{
    public partial class WebService
    {
        public void GetArea()
        {
            using (var db = new DbRepository())
            {
                //StringBuilder sb = new StringBuilder();
                //var list = db.JD_area.ToList().Select(x => new
                //{
                //    Value = x.name.Trim(),
                //    Key = x.ID.GetInt(),
                //    Parent_Id = x.parent_id.GetInt()
                //}).ToList();
                //var provinceList = list.Where(x => x.Parent_Id == 0).OrderBy(x => x.Key).ToList();
                //provinceList.ForEach(x =>
                //{

                //    sb.Append($"insert into DataDictionary(ID,GroupCode,[Key],[Value],Sort) values('{Guid.NewGuid().ToString("N")}',1,'{x.Key}','{x.Value}',{x.Key});\r\n");

                //    list.Where(y => y.Parent_Id == x.Key).ToList().ForEach(y =>
                //    {

                //        sb.Append($"insert into DataDictionary(ID,ParentKey,GroupCode,[Key],[Value],Sort) values('{Guid.NewGuid().ToString("N")}','{x.Key}',1,'{y.Key}','{y.Value}',{y.Key});\r\n");


                //        list.Where(z => z.Parent_Id == y.Key).ToList().ForEach(z =>
                //        {
                //            sb.Append($"insert into DataDictionary(ID,ParentKey,GroupCode,[Key],[Value],Sort) values('{Guid.NewGuid().ToString("N")}','{y.Key}',1,'{z.Key}','{z.Value}',{z.Key});\r\n");

                //            list.Where(j => j.Parent_Id == z.Key).ToList().ForEach(j =>
                //            {
                //                sb.Append($"insert into DataDictionary(ID,ParentKey,GroupCode,[Key],[Value],Sort) values('{Guid.NewGuid().ToString("N")}','{z.Key}',1,'{j.Key}','{j.Value}',{j.Key});\r\n");
                //            });
                //        });
                //    });
                
                //});

                //var aa = sb.ToString();
            }
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> Login(string loginName, string password)
        {
            try
            {
                if (loginName.IsNullOrEmpty() || password.IsNullOrEmpty())
                {
                    return Result(false, ErrorCode.sys_param_format_error);
                }
                using (var db = new DbRepository())
                {
                    string md5Password = CryptoHelper.MD5_Encrypt(password);
                    var admin = db.Admin.Where(x => x.Password.Equals(md5Password) && x.Account.Equals(loginName)).FirstOrDefault();
                    if (admin == null ||(admin!=null&& admin.IsDelete))
                    {
                        var user = db.User.AsQueryable().AsNoTracking().Where(x => x.Password.Equals(md5Password) && x.Account.Equals(loginName)).FirstOrDefault();
                        if (user == null)
                            return Result(false, ErrorCode.user_login_error);
                        else if (user.IsDelete)
                        {
                            return Result(false, ErrorCode.user_not_exit);
                        }
                        else
                        {
                            Client.LoginUser = user;
                            return Result(true);
                        }

                    }
                    else if (admin.IsDelete)
                    {
                        return Result(false, ErrorCode.user_not_exit);
                    }
                    else
                    {
                        Client.LoginAdmin = admin;
                        return Result(true);
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
        /// 用户登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public Tuple<ValidateCodeGenertor,string> Create_ValidateCode(string loginName)
        {
            ValidateCodeGenertor v = ValidateCodeGenertor.Default;
            CacheHelper.Remove("code" + loginName);
            var code = v.CreateValidateCode();
            CacheHelper.Set<string>("code" + loginName, code, CacheTimeOption.SixMinutes);
            return new Tuple<ValidateCodeGenertor, string>(v,code);
        }


        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> User_ChangePassword(string oldPassword, string newPassword, string cfmPassword)
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

                    var user = db.User.Where(x => x.ID.Equals(this.Client.LoginUser.ID)).FirstOrDefault();
                    if (user == null)
                        return Result(false, ErrorCode.user_not_exit);
                    if (!user.Password.Equals(oldPassword))
                        return Result(false, ErrorCode.user_password_nottrue);
                    newPassword = CryptoHelper.MD5_Encrypt(newPassword);
                    user.Password = newPassword;
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
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_User(User model)
        {
            using (DbRepository db = new DbRepository())
            {
                if (db.User.AsQueryable().AsNoTracking().Where(x => x.Account.Equals(model.Account) && !x.IsDelete).Any())
                    return Result(false, ErrorCode.user_name_already_exist);
                //var role = Cache_Get_RoleList().Where(x => x.ID.Equals(model.RoleID)).FirstOrDefault();
                //if(role==null)
                //    return Result(false, ErrorCode.datadatabase_primarykey_not_found);

                var code=CacheHelper.Get<string>("code" + Client.IP);
                if (code.IsNullOrEmpty() || !code.Equals(model.ValiteCode,StringComparison.InvariantCultureIgnoreCase))
                {
                    return Result(false, ErrorCode.verification_time_out);
                }
                model.Password = CryptoHelper.MD5_Encrypt(model.ConfirmPassword);
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                db.User.Add(model);
                if (db.SaveChanges() > 0)
                {
                    Client.LoginUser = model;
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
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
        public WebResult<PageList<User>> Get_UserPageList(int pageIndex, int pageSize, string name, string phone, DateTime? startTimeStart, DateTime? endTimeEnd)
        {
            using (DbRepository entities = new DbRepository())
            {
                var query = entities.User.AsQueryable().AsNoTracking().AsNoTracking().Where(x => !x.IsDelete);

                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.RealName.Contains(name));
                }
                if (phone.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Phone.Contains(phone));
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
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private WebResult<User> RegisterUser(string phone, string password)
        {
            User user = null;
            using (var db = new DbRepository())
            {
                user = new User
                {
                    Phone = phone,
                    Password = CryptoHelper.MD5_Encrypt(password),
           
                };
                db.User.Add(user);
                if (db.SaveChanges() > 0)
                {

                    LoginHelper.CreateUser(user.ID);
                    return Result(user);
                }
                else
                {
                    return Result(user, ErrorCode.sys_fail);
                }
            }
        }
        
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_User(User model)
        {
            using (DbRepository entities = new DbRepository())
            {
                if (entities.User.AsQueryable().AsNoTracking().Where(x => x.Phone.Equals(model.Phone) && !x.ID.Equals(Client.LoginUser.ID) && !x.IsDelete).Any())
                    return Result(false, ErrorCode.datadatabase_mobile__had);
                var oldEntity = entities.User.Find(Client.LoginUser.ID);
                if (oldEntity != null)
                {
                    oldEntity.Position = model.Position;
                    oldEntity.Email = model.Email;
                    oldEntity.CompanyName = model.CompanyName;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);
                entities.SaveChanges();
                return Result(true);
            }

        }



        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User Find_User(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository entities = new DbRepository())
            {
                var entity = entities.User.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                return entity;
            }
        }


        /// <summary>
        /// 获取用户注册验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        private string GetValidCode(string account)
        {
            var key = CacheHelper.RenderKey(Params.Cache_Prefix_Key, account);
            return CacheHelper.Get<string>(key);
        }   
    }
}
