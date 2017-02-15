
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    [LoginFilter]
    public class DataDictionaryController : BaseController
    {

        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Add(DataDictionary entity)
        {
            ModelState.Remove("ID");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_DataDictionary(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public JsonResult Update(DataDictionary entity)
        {
            if (ModelState.IsValid)
            {
                var result = WebService.Update_DataDictionary(entity);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="key"> 搜索项</param>
        /// <param name="value">搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string key, string value, GroupCode group)
        {
            return JResult(WebService.Get_DataDictionaryPageList(pageIndex, pageSize, group, key, value));
        }


        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string ID)
        {
            return JResult(WebService.Find_DataDictionary(ID));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ID)
        {
            return JResult(WebService.Delete_DataDictionary(ID));
        }



        /// <summary>
        /// 获取支付方式选择项
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSelectItem(GroupCode group)
        {
            return JResult(WebService.Get_DataDictorySelectItem(group));
        }
    }
}