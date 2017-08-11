
using CoSys.Core;
using CoSys.Model;
using CoSys.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoSys.Service
{
    public partial class WebService
    {
        string dictionaryKey = CacheHelper.RenderKey(Params.Cache_Prefix_Key, "DataDictionary");


        public Dictionary<GroupCode, Dictionary<string, DataDictionary>> Cache_Get_DataDictionary()
        {

            return CacheHelper.Get<Dictionary<GroupCode, Dictionary<string, DataDictionary>>>(dictionaryKey,CacheTimeOption.OneYear, () =>
            {
                var listss = new List<string>();
                using (var db = new DbRepository())
                {
                    var dic = db.DataDictionary.GroupBy(x => x.GroupCode).ToDictionary(x => x.Key, x => x.OrderBy(y=>y.Sort).ToList());
                    return dic.ToDictionary(x => x.Key,
                        x => {
                            var returnDic = new Dictionary<string, DataDictionary>();
                            x.Value.ForEach(y =>
                            {
                                if (!returnDic.ContainsKey(y.Key))
                                {
                                    returnDic.Add(y.Key, y);
                                }
                            });
                            return returnDic;
                        });
                }
            });

        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<DataDictionary>> Get_GroupPageList(int pageIndex, int pageSize, string parentKey, GroupCode group, string key, string value)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.DataDictionary.AsNoTracking().Where(x=>x.GroupCode==group);
                if (parentKey.IsNullOrEmpty())
                {
                    query = query.Where(x => string.IsNullOrEmpty(x.ParentKey));
                }
                else
                {
                    query = query.Where(x => x.ParentKey.Equals(parentKey.Trim()));
                }
                if (key.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Key.Contains(key));
                }
                if (value.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Value.Contains(value));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                list.ForEach(x=>{
                    if (!string.IsNullOrEmpty(x.ParentKey))
                    {
                        if (x.GroupCode == GroupCode.Area)
                        {
                            x.ParentName = Cache_Get_DataDictionary()[group][x.ParentKey.Trim()]?.Value;
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
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public WebResult<PageList<DataDictionary>> Get_DataDictionaryPageList(int pageIndex, int pageSize, GroupCode group, string key, string value)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.DataDictionary.AsNoTracking().Where(x => x.GroupCode == group);

                if (key.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Key.Contains(key));
                }
                if (value.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Value.Contains(value));
                }
                var count = query.Count();
                var list = query.OrderByDescending(x => x.Sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Add_DataDictionary(CoSys.Model.DataDictionary model)
        {
            using (DbRepository db = new DbRepository())
            {
                if(db.DataDictionary.Where(x=>x.GroupCode==GroupCode.Area&&x.Key.Equals(model.Key)).Any())
                    return Result(false, ErrorCode.sys_param_format_error);
                if (model.GroupCode == GroupCode.Channel)
                {
                    var limitFlags = Cache_Get_DataDictionary()[GroupCode.Channel].Select(x=>x.Value.Key.GetLong()).ToList();
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
                    model.Key = limitFlag.ToString();
                }
                model.ID = Guid.NewGuid().ToString("N");
                if(string.IsNullOrEmpty(model.Key))
                    model.Key = model.ID;
                db.DataDictionary.Add(model);
                if (db.SaveChanges() > 0)
                {
                    CacheHelper.Clear();
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
        public WebResult<bool> Delete_DataDictionary(string ID)
        {
            using (DbRepository db = new DbRepository())
            {
                var dataDic = db.DataDictionary.Find(ID);
                if (dataDic != null)
                {
                    db.DataDictionary.Remove(dataDic);
                    if (db.SaveChanges() > 0)
                    {
                        CacheHelper.Clear();
                        return Result(true);
                    }
                    else
                    {
                        return Result(false, ErrorCode.sys_fail);
                    }
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);
            }

        }


        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WebResult<bool> Update_DataDictionary(DataDictionary model)
        {
            using (DbRepository db = new DbRepository())
            {
                if (db.DataDictionary.Where(x => x.GroupCode == GroupCode.Area && x.Key.Equals(model.Key) && !x.ID.Equals(model.ID)).Any())
                    return Result(false, ErrorCode.sys_param_format_error);
                var oldEntity = db.DataDictionary.Find(model.ID);
                if (oldEntity != null)
                {
                    if (oldEntity.GroupCode != GroupCode.Channel && string.IsNullOrEmpty(model.Key))
                        oldEntity.Key = model.ID;
                    oldEntity.ParentKey = model.ParentKey;
                    oldEntity.Sort = model.Sort;
                    oldEntity.Remark = model.Remark;
                    oldEntity.Value = model.Value;
                }
                else
                    return Result(false, ErrorCode.sys_param_format_error);

                if (db.SaveChanges() > 0)
                {
                    CacheHelper.Clear();
                    return Result(true);
                }
                else
                {
                    return Result(false, ErrorCode.sys_fail);
                }
            }

        }


        /// <summary>
        /// 根据分组类型和key获取数值
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(GroupCode code, string key)
        {
     
            var data_Dictionary = Cache_Get_DataDictionary()[code];
            
            if (data_Dictionary != null&& data_Dictionary.ContainsKey(key))
            {
                return data_Dictionary[key].Value;
            }
            return null;
        }

        /// <summary>
        /// 获取数据字典对象
        /// </summary>
        /// <returns></returns>
        public DataDictionary Find_DataDictionary(string ID)
        {
            using (DbRepository db = new DbRepository())
            {
                return db.DataDictionary.Find(ID);
            }
        }

        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public List<SelectItem> Get_AreaList(string value)
        {
            var areas = Cache_Get_DataDictionary()[GroupCode.Area].Values.OrderByDescending(x => x.Sort).ToList().AsQueryable();
            if (!string.IsNullOrEmpty(value)&&!value.Equals("0"))
                areas = areas.Where(x=>!string.IsNullOrEmpty(x.ParentKey)&&x.ParentKey.Trim().Equals(value));
            else
                areas = areas.Where(_ => string.IsNullOrEmpty(_.ParentKey));
            var alist = areas.ToList();
            var list=alist.Select(x => new SelectItem() { Value = x.Key, Text = x.Value }).ToList();           
            list.Insert(0, new SelectItem() { Value = "0", Text = "点击选择...",Selected=true });
            return list;
        }


        /// <summary>
        /// 获取地区数据
        /// </summary>
        /// <param name="value">地区编码</param>
        /// <returns></returns>
        public Tuple<string,string> Get_ByCityCode(string cityCode)
        {
            
            var provniceName = GetValue(GroupCode.Area, cityCode.Substring(0, 2) + "0000");
            var cityName = GetValue(GroupCode.Area, cityCode);
            return new Tuple<string, string>(provniceName, cityName);
        }


        /// <summary>
        /// 获取下拉框
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<SelectItem> Get_DataDictorySelectItem(GroupCode group, Func<DataDictionary, bool> predicate = null)
        {
            var list = new List<SelectItem>();
            var dataDic = Cache_Get_DataDictionary();
            if (dataDic.Keys.Contains(group))
            {
                var dic = dataDic[group];
                var itemList = dic.Values;
                if(predicate != null)
                {
                   itemList.Where(x=>predicate(x)).OrderByDescending(x=>x.Sort).ToList().ForEach(x =>
                   {
                       list.Add(new SelectItem()
                       {
                           Text = x.Value,
                           Value = x.Key,
                           ParentKey=x.ParentKey
                       });
                   }); 
                }
                else
                {
                    itemList.OrderByDescending(x => x.Sort).ToList().ForEach(x =>
                      {
                          list.Add(new SelectItem()
                          {
                              Text = x.Value,
                              Value = x.Key,
                              ParentKey = x.ParentKey
                          });
                      });
                }
                return list;
            }
            else
                return null;
        }
    }
}
