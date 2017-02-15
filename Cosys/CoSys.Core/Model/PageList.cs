using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoSys.Core
{
    /// <summary>
    /// 分页列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PageList<T>
    {
        public PageList(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="RecordCount"></param>
        public PageList(List<T> list, int pageIndex, int pageSize, long recordCount)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.RecordCount = recordCount;
            this.List = list;
        }

        /// <summary>
        /// 全部数据总数
        /// </summary>
        public long RecordCount { get; set; }

        /// <summary>
        /// 页面总数
        /// </summary>
        public int PageCount
        {
            get
            {
                return this.PageSize > 0 ? (int)Math.Ceiling((double)RecordCount / this.PageSize) : 0;
            }
        }

        /// <summary>
        /// 当前页面索引
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 页面数据数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 权限集合
        /// </summary>
        public List<string> OperateList { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> List { get; set; }

        /// <summary>
        /// 是否最后一页
        /// </summary>
        public bool IsLastPage
        {
            get
            {
                return List.Count < PageSize || PageCount <= PageIndex;
            }
        }
    }
}
