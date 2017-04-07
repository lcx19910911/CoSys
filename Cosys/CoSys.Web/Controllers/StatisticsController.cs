using CoSys.Core;
using CoSys.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoSys.Web.Controllers
{
    [LoginFilter]
    public class StatisticsController : BaseController
    {

        public ViewResult Area()
        {
            return View(new Tuple<List<SelectItem>, List<SelectItem>>(WebService.Get_AreaList(""), WebService.Get_DataDictorySelectItem(GroupCode.Channel)));
        }

        public ActionResult GetAreaList(string name,int? province,DateTime ? searchTimeStart, DateTime? searchTimeEnd, long methodFlag=0,bool isArea=true,long departmentFlag=0)
        {
            return JResult(WebService.Get_NewsStatisticsArea(name,province, searchTimeStart, searchTimeEnd, methodFlag, isArea, departmentFlag));
        }


        /// <summary>
        /// 导出获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult ExportAreaPageList(string name, int? province, DateTime? searchTimeStart, DateTime? searchTimeEnd, long methodFlag = 0)
        {
            var list = new List<StatisticsModel>();
            var result = WebService.Get_NewsStatisticsArea(name,province, searchTimeStart, searchTimeEnd, methodFlag, true).List;
            if (result != null&&result.Count!=0)
            {
                result.ForEach(x =>
                {
                    list.Add(x);
                    if (x.Childrens != null && x.Childrens.Count != 0)
                    {
                        x.Childrens.ForEach(y =>
                        {
                            list.Add(y);
                            if (y.Childrens != null && y.Childrens.Count != 0)
                            {
                                y.Childrens.ForEach(z =>
                                {
                                    list.Add(z);
                                });
                            }
                        });
                    }
                });
            }
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            NPOIHelper<StatisticsModel>.GetExcel(list, GetAreaHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }

        private Hashtable GetAreaHT()
        {
            Hashtable hs = new Hashtable();
            hs["ProvinceName"] = "省份";
            hs["CityName"] = "市";
            hs["CountyName"] = "县/区";
            hs["StreetName"] = "街道";
            hs["AllCount"] = "总投稿数";
            hs["PassCount"] = "总采纳数";
            return hs;
        }

        private Hashtable GetUserHT()
        {
            Hashtable hs = new Hashtable();
            hs["ProvinceName"] = "省份";
            hs["CityName"] = "市";
            hs["CountyName"] = "县/区";
            hs["StreetName"] = "街道";
            hs["PeopleCount"] = "注册人数";
            return hs;
        }

        public ViewResult Chanel()
        {
            return View(WebService.Get_NewsStatisticsChanel());
        }
        /// <summary>
        /// 导出获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult ExportChanelPageList()
        {
            var list = WebService.Get_NewsStatisticsChanel();
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            NPOIHelper<StatisticsModel>.GetExcel(list, GetChanelHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }
        private Hashtable GetChanelHT()
        {
            Hashtable hs = new Hashtable();
            hs["Name"] = "发布渠道";
            hs["AllCount"] = "总发布数";
            return hs;
        }


        public ViewResult Register()
        {
            return View(WebService.Get_AreaList(""));
        }

        public ActionResult GetRegisterList(int? province, int? city, int? county)
        {
            return JResult(WebService.Get_NewsStatisticsRegister(province, city, county));
        }
        /// <summary>
        /// 导出获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult ExportRegisterPageList(string name, int? province, DateTime? searchTimeStart, DateTime? searchTimeEnd)
           {
            var list = new List<StatisticsModel>();
            var result = WebService.Get_NewsStatisticsArea(name, province, searchTimeStart, searchTimeEnd, 0,false).List;
            if (result != null && result.Count != 0)
            {
                result.ForEach(x =>
                {
                    list.Add(x);
                    if (x.Childrens != null && x.Childrens.Count != 0)
                    {
                        x.Childrens.ForEach(y =>
                        {
                            list.Add(y);
                            if (y.Childrens != null && y.Childrens.Count != 0)
                            {
                                y.Childrens.ForEach(z =>
                                {
                                    list.Add(z);
                                });
                            }
                        });
                    }
                });
            }
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);

            NPOIHelper<StatisticsModel>.GetExcel(list, GetUserHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }
    }
}