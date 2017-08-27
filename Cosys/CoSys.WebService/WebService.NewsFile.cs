//using CoSys.Core;
//using CoSys.Model;
//using CoSys.Repository;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Runtime.Remoting.Messaging;
//using System.Text;
//using System.Threading.Tasks;

//namespace CoSys.Service
//{
//    public partial class WebService
//    {

//        /// <summary>
//        /// 查找实体
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public NewsFile Find_NewsFile(string id)
//        {
//            if (!id.IsNotNullOrEmpty())
//                return null;
//            using (DbRepository db = new DbRepository())
//            {
//                return db.NewsFile.Find(id);
//            }
//        }

//        /// <summary>
//        /// 增加日志
//        /// </summary>
//        /// <param name="code">日志编码</param>
//        /// <param name="newsId">新闻Id</param>
//        /// <param name="remark">备注</param>
//        /// <param name="beforeJson"></param>
//        /// <param name="afterJson"></param>
//        public void Add_NewsFile(NewsFile model)
//        {
//            using (DbRepository db = new DbRepository())
//            {
//                db.NewsFile.Add(model);
//                db.SaveChanges();
//            }

//        }

//        /// <summary>
//        /// 获取分页列表
//        /// </summary>
//        /// <param name="pageIndex">页码</param>
//        /// <param name="pageSize">分页大小</param>
//        /// <param name="name">名称 - 搜索项</param>
//        /// <param name="no">编号 - 搜索项</param>
//        /// <returns></returns>
//        public WebResult<PageList<NewsFile>> Get_NewsFilePageList(int pageIndex, int pageSize, string name, string newsTitle, DateTime? searchTimeStart, DateTime? searchTimeEnd)
//        {
//            using (DbRepository db = new DbRepository())
//            {
//                var query = db.NewsFile.AsQueryable().AsNoTracking().Where(x => !x.IsDelete);
//                if (name.IsNotNullOrEmpty())
//                {
//                    query = query.Where(x => x.Name.Contains(name));
//                }
//                if (newsTitle.IsNotNullOrEmpty())
//                {
//                    var newsIds = db.News.Where(x => !x.IsDelete && x.Title.Contains(newsTitle)).Select(x => x.ID).ToList();
//                    if (newsIds != null && newsIds.Count > 0)
//                    {
//                        query = query.Where(x => newsIds.Contains(x.NewsID));
//                    }
//                }
//                if (searchTimeStart != null)
//                {
//                    query = query.Where(x => x.CreatedTime >= searchTimeStart);
//                }
//                if (searchTimeEnd != null)
//                {
//                    searchTimeEnd = searchTimeEnd.Value.AddDays(1);
//                    query = query.Where(x => x.CreatedTime < searchTimeEnd);
//                }
//                var count = query.Count();
//                var list = query.OrderByDescending(x => x.CreatedTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
//                var newsIdList = list.Select(x => x.NewsID).ToList();
//                var newsDic = db.News.Where(x => newsIdList.Contains(x.ID)&&!x.IsDelete).ToList().ToDictionary(x=>x.ID);
//                var userIdList = list.Select(x => x.UserID).ToList();
//                var userDic = db.User.Where(x => userIdList.Contains(x.ID) && !x.IsDelete).ToList().ToDictionary(x => x.ID);
//                list.ForEach(x =>
//                {
//                    if (x.UserID.IsNotNullOrEmpty() && userDic.ContainsKey(x.UserID))
//                    {
//                        x.UserName = userDic[x.UserID].RealName;
//                    }
//                    if (x.NewsID.IsNotNullOrEmpty() && newsDic.ContainsKey(x.NewsID))
//                    {
//                        x.NewsName = newsDic[x.NewsID].Title;
//                    }
//                });
//                return ResultPageList(list, pageIndex, pageSize, count);
//            }
//        }
//    }
//}
