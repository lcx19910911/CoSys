
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
        public WebResult<PageList<NewsDepartment>> Get_NewsDepartmentPageList(int pageIndex, int pageSize, string name)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.NewsDepartment.AsQueryable().AsNoTracking().Where(x => !x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var dic = db.NewsDepartment.ToDictionary(x => x.ID);
                var list = query.OrderByDescending(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x =>
                {
                    if (x.ParentID.IsNotNullOrEmpty() && dic.ContainsKey(x.ParentID))
                    {
                        x.ParentName = dic.GetValue(x.ParentID).Name;
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
        public WebResult<bool> Add_NewsDepartment(NewsDepartment model)
        {
            using (DbRepository db = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                if (model.ParentID.IsNotNullOrEmpty() && model.ParentID == "-1")
                {
                    model.ParentID = string.Empty;
                }
                db.NewsDepartment.Add(model);
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
        public WebResult<bool> Update_NewsDepartment(NewsDepartment model)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.NewsDepartment.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Name = model.Name;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);
                if (model.ParentID.IsNotNullOrEmpty() && model.ParentID == "-1")
                {
                    oldEntity.ParentID = string.Empty;
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
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsDepartment Find_NewsDepartment(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {
                var model= db.NewsDepartment.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                return model;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_NewsDepartment(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                //找到实体
                db.NewsDepartment.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// <param name="NewsDepartmentFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_NewsDepartmentSelectItem(string id) 
        {
            List<SelectItem> list = new List<SelectItem>();

            using (DbRepository db = new DbRepository())
            {
                if (id.IsNotNullOrEmpty())
                {
                    db.NewsDepartment.AsQueryable().AsNoTracking().Where(x => !x.IsDelete&&!string.IsNullOrEmpty(x.ParentID)&&x.ParentID.Equals(id)).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Selected = x.ID.Equals(id),
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
                else
                {
                    db.NewsDepartment.AsQueryable().AsNoTracking().Where(x => !x.IsDelete&&string.IsNullOrEmpty(id)).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Selected = x.ID.Equals(id),
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
                return list;
            }
        }
    }
}
