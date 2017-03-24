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
            return View(new Tuple<List<SelectItem>, List<SelectItem>, List<SelectItem>, List<SelectItem>>(WebService.Get_AreaList(""), WebService.Get_AreaList("16"), WebService.Get_AreaList("1303"), WebService.Get_DataDictorySelectItem(GroupCode.Channel)));
        }

        public ActionResult GetAreaList(int? province, int? city, int? county, long methodFlag)
        {
            return JResult(WebService.Get_NewsStatisticsArea(province, city, county, methodFlag));
        }


        /// <summary>
        /// 导出获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult ExportAreaPageList(int? province, int? city, int? county, long methodFlag)
        {
            int type = 1;
            if (province != null)
            {
                if (city != null)
                {
                    if (county != null)
                    {
                        type = 3;
                    }
                    else
                        type = 2;
                }
                else
                {
                    type = 1;
                }
            }
            var list = WebService.Get_NewsStatisticsArea(province, city, county, methodFlag);
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            NPOIHelper<StatisticsModel>.GetExcel(list, GetHT(province,city,county), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }

        private Hashtable GetHT(int? province, int? city, int? county)
        {
            Hashtable hs = new Hashtable();
            if (province == null || province == -1)
            {
                hs["Name"] = "省份";
            } 
            else if (city == null || city == 0 || city == -1)
            {
                hs["ProvinceName"] = "省份";
                hs["Name"] = "市";
            }
            else if (county == null || county == 0 || county == -1)
            {
                hs["ProvinceName"] = "省份";
                hs["CityName"] = "市";
                hs["Name"] = "县/区";
            }
            else
            {
                hs["ProvinceName"] = "省份";
                hs["CityName"] = "市";
                hs["CountyName"] = "县/区";
                hs["Name"] = "街道";
            }
            hs["AllCount"] = "总投稿数";
            hs["PassCount"] = "总采纳数";
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
            hs["Name"] ="投稿渠道";
            hs["AllCount"] = "总投稿数";
            hs["PassCount"] = "总采纳数";
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
        public ActionResult ExportRegisterPageList(int? province, int? city, int? county)
        {
            var list = WebService.Get_NewsStatisticsRegister(province, city, county);
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            int type = 1;
            if(province!=null)
            {
                if (city != null)
                {
                    if (county != null)
                    {
                        type = 3;
                    }
                    else
                        type = 2;
                }
                else
                    type = 1;
            }
            NPOIHelper<StatisticsModel>.GetExcel(list, GetHT(province, city, county), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }
    }
}