
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
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<Role>> Get_RolePageList(int pageIndex, int pageSize, string name, string no)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.Role.AsQueryable().AsNoTracking().Where(x => !x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_Role(Role model)
        {
            using (DbRepository db = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                model.CreatedTime = DateTime.Now;
                db.Role.Add(model);
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
        public WebResult<bool> Update_Role(Role model)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.Role.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Remark = model.Remark;
                    oldEntity.Name = model.Name;
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
        public Role Find_Role(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {
                var model= db.Role.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                if (model != null)
                {
                    model.CodeStr = model.Code.GetDescription();
                }
                return model;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Role(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                //找到实体
                db.Role.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
                {
                    x.IsDelete = true;
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
        /// 获取选择项
        /// </summary>
        /// <param name="roleFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_RoleSelectItem(string id)
        {
            List<SelectItem> list = new List<SelectItem>();

            using (DbRepository db = new DbRepository())
            {
                db.Role.AsQueryable().AsNoTracking().Where(x => !x.IsDelete).OrderBy(x => x.CreatedTime).ToList().ForEach(x =>
                {
                    list.Add(new SelectItem()
                    {
                        Selected = x.ID.Equals(id),
                        Text = x.Name,
                        Value = x.ID
                    });
                });
                return list;
            }
        }
    }
}
