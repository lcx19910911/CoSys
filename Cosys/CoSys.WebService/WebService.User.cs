
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
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<int> Login(string loginName, string password)
        {
            try
            {
                if (loginName.IsNullOrEmpty() || password.IsNullOrEmpty())
                {
                    return Result(0, ErrorCode.sys_param_format_error);
                }
                using (var db = new DbRepository())
                {
                    string md5Password = CryptoHelper.MD5_Encrypt(password);

                    var user = db.User.AsQueryable().AsNoTracking().Where(x => x.Password.Equals(md5Password) && x.Account.Equals(loginName)).FirstOrDefault();
                    if (user == null)
                        return Result(0, ErrorCode.user_login_error);
                    else if (user.IsDelete)
                    {
                        return Result(0, ErrorCode.user_not_exit);
                    }
                    else
                    {
                        if (!user.IsAdmin)
                        {
                            Client.LoginUser = user;
                            return Result(1);
                        }
                        else
                        {
                            Client.LoginAdmin = user;
                            return Result(2);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(ex);
                return Result(0, ErrorCode.sys_fail);
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
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<User>> Get_UserPageList(int pageIndex, int pageSize, string name,int? type, int? areaId,bool isAdmin=false)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.User.AsQueryable().AsNoTracking().AsNoTracking().Where(x => !x.IsDelete&&!x.IsSuperAdmin);

                if (areaId == null && type == null)
                {
                    query = query.Where(x => x.IsAdmin == isAdmin);
                }

                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.RealName.Contains(name));
                }
                if (areaId != null&& type != null)
                {
                    if (type == 0)
                    {
                        query = query.Where(x =>!string.IsNullOrEmpty(x.ProvoniceCode)&& x.ProvoniceCode.Equals(areaId.ToString())&& string.IsNullOrEmpty(x.CityCode));
                    }
                    else if (type == 1)
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.CityCode) && x.CityCode.Equals(areaId.ToString()));
                    }
                    else if (type == 2)
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.CountyCode) && x.CountyCode.Equals(areaId.ToString()));
                    }
                    else if (type == 3)
                    {
                        query = query.Where(x => !string.IsNullOrEmpty(x.StreetCode) && x.StreetCode.Equals(areaId.ToString()));
                    }
                    else if (type == -1)
                    {
                        query = query.Where(x => string.IsNullOrEmpty(x.ProvoniceCode) );
                    }
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.RealName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                var userIdList = list.Select(x => x.ID).ToList();
                var newsGroupDic = db.News.Where(x=> userIdList.Contains(x.UserID)).GroupBy(x=>x.UserID).ToDictionary(x=>x.Key,x=>x.ToList());
                var roleDic = new Dictionary<string, Role>();
                var areaDic = new Dictionary<string, DataDictionary>();
                if (isAdmin)
                {
                    roleDic = db.Role.ToDictionary(x => x.ID);
                    areaDic = Cache_Get_DataDictionary()[GroupCode.Area];
                }
                list.ForEach(x =>
                {
                    if (isAdmin)
                    {
                        if (x.RoleID.IsNotNullOrEmpty() && roleDic.ContainsKey(x.RoleID))
                            x.RoleName = roleDic[x.RoleID].Name;
                        if (x.ProvoniceCode.IsNotNullOrEmpty() && areaDic.ContainsKey(x.ProvoniceCode))
                            x.ProvoniceName = areaDic[x.ProvoniceCode].Value;
                        if (x.CityCode.IsNotNullOrEmpty() && areaDic.ContainsKey(x.CityCode))
                            x.CityName = areaDic[x.CityCode].Value;
                        if (x.CountyCode.IsNotNullOrEmpty() && areaDic.ContainsKey(x.CountyCode))
                            x.CountyName = areaDic[x.CountyCode].Value;
                        if (x.StreetCode.IsNotNullOrEmpty() && areaDic.ContainsKey(x.StreetCode))
                            x.StreetName = areaDic[x.StreetCode].Value;
                    }

                    if (newsGroupDic.ContainsKey(x.ID))
                    {
                        x.AllCount = newsGroupDic[x.ID].Count();
                        x.PassCount = newsGroupDic[x.ID].Where(y => y.State == NewsState.Pass || y.State == NewsState.Plush).Count();
                    }
                });
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 用户修改密码
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns> 
        public WebResult<bool> User_ChangePassword(string oldPassword, string newPassword, string cfmPassword, string id)
        {
            if (newPassword.IsNullOrEmpty() || cfmPassword.IsNullOrEmpty() || id.IsNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            if (!cfmPassword.Equals(newPassword))
            {
                return Result(false, ErrorCode.user_password_notequal);

            }
            using (var db = new DbRepository())
            {
                
                var user = db.User.Find(id);
                if (user == null)
                    return Result(false, ErrorCode.user_not_exit);
                if (oldPassword == "")
                {
                    oldPassword = CryptoHelper.MD5_Encrypt(oldPassword);
                    if (!user.Password.Equals(oldPassword))
                        return Result(false, ErrorCode.user_password_nottrue);
                }
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

                if (!model.IsAdmin)
                {
                    var code = CacheHelper.Get<string>("code" + Client.IP);
                    if (code.IsNullOrEmpty() || !code.Equals(model.ValiteCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return Result(false, ErrorCode.verification_time_out);
                    }
                }
                if (model.ProvoniceCode == "0")
                    model.ProvoniceCode = null;
                if (model.CityCode == "0")
                    model.CityCode = null;
                if (model.CountyCode == "0")
                    model.CountyCode = null;
                if (model.StreetCode == "0")
                    model.StreetCode = null;
                model.Password = CryptoHelper.MD5_Encrypt("111111");
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                db.User.Add(model);
                if (db.SaveChanges() > 0)
                {
                    LoginHelper.CreateUser(model.ID);
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
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
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.User.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Email = model.Email;
                    oldEntity.RealName = model.RealName;
                    oldEntity.PenName = model.PenName;
                    oldEntity.IDCardAddres = model.IDCardAddres;
                    oldEntity.IDCard = model.IDCard;
                    oldEntity.PenName = model.PenName;
                    oldEntity.CompanyName = model.CompanyName;
                    oldEntity.Position = model.Position;
                    oldEntity.Zipcode = model.Zipcode;
                    oldEntity.ProvoniceCode = model.ProvoniceCode;
                    oldEntity.CityCode = model.CityCode;
                    oldEntity.CountyCode = model.CountyCode;
                    oldEntity.StreetCode = model.StreetCode;
                    oldEntity.Code = model.Code;
                    oldEntity.Addres = model.Addres;
                    oldEntity.TelePhone = model.TelePhone;
                    oldEntity.Phone = model.Phone;
                    oldEntity.QQ = model.QQ;

                    oldEntity.DepartmentFlag = model.DepartmentFlag;
                    oldEntity.OperateFlag = model.OperateFlag;
                    oldEntity.RoleID = model.RoleID;

                    if (oldEntity.ProvoniceCode == "0")
                        oldEntity.ProvoniceCode = null;
                    if (oldEntity.CityCode == "0")
                        oldEntity.CityCode = null;
                    if (oldEntity.CountyCode == "0")
                        oldEntity.CountyCode = null;
                    if (oldEntity.StreetCode == "0")
                        oldEntity.StreetCode = null;
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
        /// 删除分类
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_User(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                //找到实体
                db.User.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.IsDelete = true;
                });
                return db.SaveChanges() > 0 ? Result(true) : Result(false, ErrorCode.sys_fail);
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
