
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
        /// 全局缓存
        /// </summary>
        /// <returns></returns>
        public static List<TEntity> GetList<TEntity>() where TEntity : BaseEntity
        {
            string key = CacheHelper.RenderKey(Params.Cache_Prefix_Key, typeof(TEntity).Name);

            return CacheHelper.Get<List<TEntity>>(key, () =>
            {
                using (var db = new DbRepository())
                {
                    DbSet<TEntity> dbSet = db.Set<TEntity>();

                    return dbSet.ToList();
                }
            });
        }

        private WebClient Client = null;

        public WebService(WebClient client)
        {
            this.Client = client;
        }

        /// <summary>
        /// list转换pageList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="allList">需要分页的数据</param>
        /// <returns></returns>
        private PageList<T> ConvertPageList<T>(List<T> allList, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            int skip = (pageIndex - 1) * pageSize;
            var list = allList?.Skip(skip).Take(pageSize).ToList();
            return new PageList<T>(list, pageIndex, pageSize, allList == null ? 0 : allList.LongCount());
        }

        /// <summary>
        /// list转换pageList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List">需要分页的数据</param>
        /// <returns></returns>
        private PageList<T> ConvertPageList<T>(List<T> list, int pageIndex, int pageSize,int recoredCount)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 10 : pageSize;
            return new PageList<T>(list, pageIndex, pageSize, recoredCount);
        }


        public WebResult<T> Result<T>(T model)
        {
            return Result(model, ErrorCode.sys_success);
        }

        public WebResult<T> Result<T>(T model, ErrorCode code)
        {
            return new WebResult<T> { Code = code, Result = model };
        }

        public WebResult<T> Result<T>(T model, ErrorCode code, string append)
        {
            return new WebResult<T> { Code = code, Result = model, Append = append };
        }
        public WebResult<PageList<T>> ResultPageList<T>(List<T> model, int pageIndex, int pageSize)
        {
            return Result(ConvertPageList<T>(model, pageIndex, pageSize));
        }

        public WebResult<PageList<T>> ResultPageList<T>(List<T> model, int pageIndex, int pageSize,int recoredCount)
        {
            List<string> operateList = new List<string>();
            //获取页面的权限
            return Result(ConvertPageList<T>(model, pageIndex, pageSize, recoredCount));
        }

        public WebResult<PageList<T>> ResultPageList<T>(List<T> model, int pageIndex, int pageSize, ErrorCode code)
        {
            return Result(ConvertPageList<T>(model, pageIndex, pageSize), code);
        }
    }
}
