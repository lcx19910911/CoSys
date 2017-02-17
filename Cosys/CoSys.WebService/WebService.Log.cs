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
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Log Find_Log(string id)
        {
            if (!id.IsNotNullOrEmpty())
                return null;
            using (DbRepository db = new DbRepository())
            {
                return db.Log.Find(id);
            }
        }

        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="code">日志编码</param>
        /// <param name="newsId">新闻Id</param>
        /// <param name="remark">备注</param>
        /// <param name="beforeJson"></param>
        /// <param name="afterJson"></param>
        public void Add_Log(LogCode code,string newsId,string userId,string remark = null, string beforeJson=null,string afterJson = null, string info = null)
        {
            using (DbRepository db = new DbRepository())
            {
                var model = new Log();
                model.ID = Guid.NewGuid().ToString("N");
                model.NewsID = newsId;
                model.AdminID = userId;
                model.CreatedTime = DateTime.Now;
                model.Remark = remark;
                model.Code = code;
                model.BeforeJson = beforeJson;
                model.AfterJson = afterJson;
                model.UpdateInfo = info;
                db.Log.Add(model);

                db.SaveChanges();
            }

        }
     
        /// 获取学员缴费记录
        /// </summary>
        /// <param name="">门店id</param>
        /// <returns></returns>
        public WebResult<PageList<Log>> Get_LogByStudentId(int pageIndex,
            int pageSize, string newId)
        {
            using (DbRepository db = new DbRepository())
            {
                var query = db.Log.AsQueryable().AsNoTracking().Where(x => x.NewsID.Equals(newId)).OrderByDescending(x => x.CreatedTime);
                var count = query.Count();
                var list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return ResultPageList(list, pageIndex, pageSize, count);
            }
        }
    }
}
