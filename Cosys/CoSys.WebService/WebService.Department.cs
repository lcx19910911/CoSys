
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
        public WebResult<PageList<Department>> Get_DepartmentPageList(int pageIndex, int pageSize, string name)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.Department.AsQueryable().AsNoTracking().Where(x => !x.IsDelete);
                if (name.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Name.Contains(name));
                }

                var count = query.Count();
                var dic = db.Department.ToDictionary(x => x.ID);
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
        public WebResult<bool> Add_Department(Department model)
        {
            using (DbRepository db = new DbRepository())
            {
                model.ID = Guid.NewGuid().ToString("N");
                var limitFlags = db.Department.Where(x => (x.Flag & (long)GlobalFlag.Removed) == 0).Select(x => x.Flag).ToList();
                var limitFlagAll = 0L;
                // 获取所有角色位值并集
                limitFlags.ForEach(x => limitFlagAll |= x);
                var limitFlag = 0L;
                // 从低位遍历是否为空
                for (var i = 0; i < 64; i++)
                {
                    if ((limitFlagAll & (1 << i)) == 0)
                    {
                        limitFlag = 1 << i;
                        break;
                    }
                }
                model.Flag = limitFlag;
                db.Department.Add(model);
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
        public WebResult<bool> Update_Department(Department model)
        {
            using (DbRepository db = new DbRepository())
            {
                var oldEntity = db.Department.Find(model.ID);
                if (oldEntity != null)
                {
                    oldEntity.Name = model.Name;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);
               
                if (model.ParentID.IsNotNullOrEmpty() && model.ParentID == "-1")
                {
                    oldEntity.ParentID = null;
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
        public Department Find_Department(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {
                var model= db.Department.AsQueryable().AsNoTracking().FirstOrDefault(x => x.ID.Equals(id));
                return model;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public WebResult<bool> Delete_Department(string ids)
        {
            if (!ids.IsNotNullOrEmpty())
            {
                return Result(false, ErrorCode.sys_param_format_error);
            }
            using (DbRepository db = new DbRepository())
            {
                //找到实体
                db.Department.Where(x => ids.Contains(x.ID)).ToList().ForEach(x =>
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
        /// <param name="DepartmentFlag">角色flag值</param>
        /// <returns></returns>
        public List<SelectItem> Get_DepartmentSelectItem(string id) 
        {
            List<SelectItem> list = new List<SelectItem>();

            using (DbRepository db = new DbRepository())
            {
                if (id.IsNotNullOrEmpty())
                {
                    db.Department.AsQueryable().AsNoTracking().Where(x => !x.IsDelete&&!string.IsNullOrEmpty(x.ParentID)&&x.ParentID.Equals(id)).OrderBy(x=>x.Flag).ToList().ForEach(x =>
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
                    db.Department.AsQueryable().AsNoTracking().Where(x => !x.IsDelete&&string.IsNullOrEmpty(x.ParentID)).OrderBy(x => x.Flag).ToList().ForEach(x =>
                    {
                        list.Add(new SelectItem()
                        {
                            Text = x.Name,
                            Value = x.ID
                        });
                    });
                }
                return list;
            }
        }


        #region 部门下拉框

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        private List<ZTreeNode> Get_DepartmentZTreeChildren(string parentId, List<IGrouping<string, Department>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key== parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.Flag.ToString(),
                        nocheck= Get_DepartmentZTreeChildren(x.ID, groups).Count>0,
                        children = Get_DepartmentZTreeChildren(x.ID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        public List<ZTreeNode> Get_DepartmentZTreeChildren(string parentId)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            using (var db = new DbRepository())
            {
                var group = db.Department.AsQueryable().Where(x =>!x.IsDelete).AsNoTracking().OrderByDescending(x => x.Flag).GroupBy(x => x.ParentID).ToList();
                return Get_DepartmentZTreeChildren(parentId, group);
            }
        }

        /// <summary>
        /// 获取ZTree子节点
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <param name="groups">分组数据</param>
        /// <returns></returns>
        private List<ZTreeNode> Get_DepartmentZTreeFlagChildren(string parentId, List<IGrouping<string, Department>> groups)
        {
            List<ZTreeNode> ztreeNodes = new List<ZTreeNode>();
            var group = groups.FirstOrDefault(x => x.Key == parentId);
            if (group != null)
            {
                ztreeNodes = group.Select(
                    x => new ZTreeNode()
                    {
                        name = x.Name,
                        value = x.Flag.ToString(),
                        children = Get_DepartmentZTreeFlagChildren(x.ID, groups)
                    }).ToList();
            }
            return ztreeNodes;
        }

        #endregion
    }
}
