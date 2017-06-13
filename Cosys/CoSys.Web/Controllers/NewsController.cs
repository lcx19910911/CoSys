
using CoSys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CoSys.Core;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;

namespace CoSys.Web.Controllers
{
    [LoginFilter]
    public class NewsController : BaseController
    {

        public ActionResult Index()
        {
            if (Client.LoginUser == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult Statistics()
        {
            return View();
        }
        public ActionResult Admin()
        {
            if (Client.LoginAdmin == null)
                return RedirectToAction("Login", "Account");
            ViewBag.CanExport = false;
            if (Client.LoginAdmin.IsSuperAdmin)
            {
                ViewBag.CanExport = true;
            }
            else
            {
                //判断角色
                var role = WebService.Find_Role(Client.LoginAdmin.RoleID);
                if (role != null)
                {
                    if (role.AuditState == NewsAuditState.EditorialAudit || role.AuditState == NewsAuditState.LastAudit)
                        ViewBag.CanExport = true;
                }
            }
            return View();
        }

        public ActionResult My()
        {
            if (Client.LoginAdmin == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        public ActionResult Manage(string id)
        {
            var model = WebService.Find_News(id);
            if (model == null)
            {
                model = new News();
                model.ChildrenDepartmentList = new List<SelectItem>();
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
            }
            else
            {
                var user = Client.LoginUser;
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
                if (user == null)
                    return RedirectToAction("Login", "Account");
                if (!model.UserID.Equals(user.ID))
                    return View("Index");
                var department = new Department();
                if (model.DepartmentID.Split(';').Length == 1)
                {
                    department = WebService.Find_Department(model.DepartmentID);
                }
                else if (model.DepartmentID.Split(';').Length == 2)
                {
                    model.ChildrenDepartmentList = WebService.Get_DepartmentSelectItem(model.DepartmentID.Split(';')[0]);
                    department = WebService.Find_Department(model.DepartmentID.Split(';')[1]);
                }

                if (department == null)
                    return View("Index");
                model.Logs = WebService.Get_LogByNewsId(model.ID);
            }

            model.TypeList = WebService.Get_DataDictorySelectItem(GroupCode.Type);
            model.MethodList = WebService.Get_DataDictorySelectItem(GroupCode.Channel);
            return View(model);
        }

        public ActionResult AdminManage(string id)
        {
            var model = WebService.Find_News(id);
            ViewBag.CanEdit = false; ;
            if (model == null)
            {
                model = new News();
                model.ChildrenDepartmentList = new List<SelectItem>();
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);
            }
            else
            {
                var admin = Client.LoginAdmin;
                model.DepartmentList = WebService.Get_DepartmentSelectItem(null);

                if (admin == null)
                    return RedirectToAction("Login", "Account");
                var department = new Department();
                if (model.DepartmentID.Split(';').Length == 1)
                {
                    department = WebService.Find_Department(model.DepartmentID);
                }
                else if (model.DepartmentID.Split(';').Length == 2)
                {
                    model.ChildrenDepartmentList = WebService.Get_DepartmentSelectItem(model.DepartmentID.Split(';')[0]);
                    department = WebService.Find_Department(model.DepartmentID.Split(';')[1]);
                }

                if (department == null)
                    return View("Admin");
                model.Logs = WebService.Get_LogByNewsId(model.ID);
                if ((model.State == NewsState.None || model.State == NewsState.Reject) && !model.UserID.Equals(admin.ID))
                    return View("Admin");
                if (!admin.IsSuperAdmin)
                {
                    //判断角色
                    var role = WebService.Find_Role(admin.RoleID);
                    if (role == null)
                    {
                        return View("Admin");
                    }
                    ViewBag.Role = role;
                    //审核中
                    if (model.State == NewsState.WaitAudit)
                    {
                        //判断投递部门权限
                        if ((department.Flag & admin.DepartmentFlag) == 0 && !admin.IsSuperAdmin && !model.UserID.Equals(admin.ID))
                        {
                            return View("Admin");
                        }
                        //是否审核
                        if (role.AuditState != model.AuditState)
                        {
                            ViewBag.CanEdit = false;
                        }
                        else
                        {
                            ViewBag.CanEdit = true;
                        }
                    }
                }
                else
                {
                    ViewBag.CanEdit = true;
                }

            }

            model.TypeList = WebService.Get_DataDictorySelectItem(GroupCode.Type);
            model.MethodList = WebService.Get_DataDictorySelectItem(GroupCode.Channel);
            return View(model);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult Add(News model, bool isAduit = true, long departmentFlag = 0)
        {
            ModelState.Remove("ID");
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("UserID");
            ModelState.Remove("MethodFlag");
            ModelState.Remove("State");
            ModelState.Remove("DepartmentID");
            if (ModelState.IsValid)
            {
                var result = WebService.Add_News(model, isAduit, departmentFlag);
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
        [ValidateInput(false)]
        public JsonResult Update(News model, bool isAduit = true,string mark="")
        {
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("State");
            ModelState.Remove("UserID");
            ModelState.Remove("MethodFlag");
            if (ModelState.IsValid)
            {
                var result = WebService.Update_News(model, isAduit,mark);
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
        [ValidateInput(false)]
        public JsonResult UserUpdate(News model, bool isAduit = true)
        {
            ModelState.Remove("CreatedTime");
            ModelState.Remove("IsDelete");
            ModelState.Remove("State");
            ModelState.Remove("UserID");
            ModelState.Remove("MethodFlag");
            if (ModelState.IsValid)
            {
                var result = WebService.UserUpdate_News(model, isAduit);
                return JResult(result);
            }
            else
            {
                return ParamsErrorJResult(ModelState);
            }
        }
        


        public ActionResult ExportStatistics(string title, string userId, NewsState? state, int? type, int? areaId, string name, DateTime? searchTimeStart, DateTime? searchTimeEnd, long methodFlag = 0, long departmentFlag = 0)
        {
            var list = WebService.Get_UserNewsPageList(1, 100000, title, userId, state, type, areaId, name, searchTimeStart, searchTimeEnd, methodFlag, departmentFlag).Result.List;
            string fileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            string filePath = Path.Combine(Server.MapPath("~/") + @"Export\" + fileName);
            NPOIHelper<News>.GetExcel(list, GetChanelHT(), filePath);
            //Directory.Delete(filePath);
            return File(filePath, "application/vnd.ms-excel", fileName);
        }
        private Hashtable GetChanelHT()
        {
            Hashtable hs = new Hashtable();
            hs["Title"] = "标题";
            hs["SubmitTime"] = "投递时间";
            hs["StateStr"] = "状态";
            hs["UserName"] = "作者";
            return hs;
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetPageList(int pageIndex, int pageSize, string title, NewsState? state, int? type, int? areaId)
        {
            return JResult(WebService.Get_NewsPageList(pageIndex, pageSize, title, state, type, areaId));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetHistoryPageList(int pageIndex, int pageSize, string newsId)
        {
            return JResult(WebService.Get_NewsHistoryPageList(pageIndex, pageSize, newsId));
        }
        


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetAdminPageList(int pageIndex, int pageSize, string title, string userId, NewsState? state, bool isAudit = true)
        {
            return JResult(WebService.Get_AdminNewsPageList(pageIndex, pageSize, title, userId,"", isAudit, state));
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetUserPageList(int pageIndex, int pageSize, string title, string userId, NewsState? state, int? type, int? areaId, string name, DateTime? searchTimeStart, DateTime? searchTimeEnd, long methodFlag = 0, long departmentFlag = 0)
        {
            return JResult(WebService.Get_UserNewsPageList(pageIndex, pageSize, title, userId, state, type, areaId, name, searchTimeStart, searchTimeEnd, methodFlag, departmentFlag));
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetMyPageList(int pageIndex, int pageSize, string title, NewsState? state, int? type, int? areaId)
        {
            return JResult(WebService.Get_UserNewsPageList(pageIndex, pageSize, title, Client.LoginAdmin.ID, state, type, areaId, "", null, null));
        }

        /// <summary>
        /// 查找实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Find(string id)
        {
            return JResult(WebService.Find_News(id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Delete(string ids)
        {
            return JResult(WebService.Delete_News(ids));
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Reset(string id)
        {
            return JResult(WebService.ReSet_News(id));
        }

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Audit(YesOrNoCode isPass, string id, string msg)
        {
            return JResult(WebService.Audit_News(isPass, id, msg));
        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult Plush(string id, long flag, string msg)
        {
            return JResult(WebService.Plush_News(id, flag, msg));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ActionResult EditorialPass(string id, string msg)
        {
            return JResult(WebService.News_EditorialPass(id, msg));
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetTxtExportPageList(int pageIndex, int pageSize, string title, NewsState? state,string ids)
        {
            var result = WebService.Get_AdminNewsPageList(pageIndex, pageSize, title, "", ids, true, state,true);
            if (result.Result.List != null && result.Result.List.Count > 0)
            {
                var methodDic = WebService.Cache_Get_DataDictionary()[GroupCode.Channel];
                var path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "/Export/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "/";
                result.Result.List.ForEach(x =>
                        {
                            var list = methodDic.Values.Where(y => (y.Key.GetLong() & x.PlushMethodFlag) != 0).Select(y => y.Value).ToList();
                            if (list == null)
                                list = new List<string>();
                            var str = string.Join(",", list);
                            Export(x, str, path,true);
                        });
                var zipUrl = "/Export/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                ZipHelper.Zip(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + zipUrl, System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "/Export/", 1, "", new string[] { path });

                AsyncHelper.Run(() => {
                    Directory.Delete(path,true);
                });
                return JResult(zipUrl);
            }
            return JResult("");
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="name">名称 - 搜索项</param>
        /// <param name="no">编号 - 搜索项</param>
        /// <returns></returns>
        public ActionResult GetWordExportPageList(int pageIndex, int pageSize, string title, NewsState? state, string ids)
        {
            var result = WebService.Get_AdminNewsPageList(pageIndex, pageSize, title, "", ids, true, state, true);
            if (result.Result.List != null && result.Result.List.Count > 0)
            {
                var methodDic = WebService.Cache_Get_DataDictionary()[GroupCode.Channel];
                var path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "/Export/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "/";
                result.Result.List.ForEach(x =>
                {
                    var list = methodDic.Values.Where(y => (y.Key.GetLong() & x.PlushMethodFlag) != 0).Select(y => y.Value).ToList();
                    if (list == null)
                        list = new List<string>();
                    var str = string.Join(",", list);
                    Export(x, str, path);
                });
                var zipUrl = "/Export/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                ZipHelper.Zip(System.Web.HttpContext.Current.Request.PhysicalApplicationPath + zipUrl, System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "/Export/", 1, "", new string[] { path });

                AsyncHelper.Run(() => {
                    Directory.Delete(path,true);
                });
                return JResult(zipUrl);
            }
            return JResult("");
        }

        public void Export(News model, string str, string path, bool isTxt = false)
        {

            if (!(Directory.Exists(path)))
            {
                Directory.CreateDirectory(path);
            }
            if (isTxt)
            {
                FileStream fs = new FileStream(path + model.Title + ".txt", FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                try
                {
                    sw.Write(xxHTML(model.Content));
                    sw.Flush();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sw.Close();
                    fs.Close();
                }

            }
            else
            {
                using (MemoryStream ms = WordHelper.Export(model, str))
                {
                    byte[] bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始   
                    ms.Seek(0, SeekOrigin.Begin);

                    FileStream fs = new FileStream(path + model.Title + ".doc", FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    try
                    {
                        bw.Write(bytes);
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        bw.Close();
                        fs.Close();
                    }
                }
            }
        }




        public string xxHTML(string html)
        {

            html = html.Replace("(<style)+[^<>]*>[^\0]*(</style>)+", "");
            html = html.Replace(@"\<img[^\>] \>", "");
            html = html.Replace(@"<p>", "");
            html = html.Replace(@"</p>", "");


            System.Text.RegularExpressions.Regex regex0 =
            new System.Text.RegularExpressions.Regex("(<style)+[^<>]*>[^\0]*(</style>)+", System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"<script[\s\S] </script *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@" href *= *[\s\S]*script *:", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex3 = new System.Text.RegularExpressions.Regex(@" on[\s\S]*=", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex4 = new System.Text.RegularExpressions.Regex(@"<iframe[\s\S] </iframe *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex5 = new System.Text.RegularExpressions.Regex(@"<frameset[\s\S] </frameset *>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex6 = new System.Text.RegularExpressions.Regex(@"\<img[^\>] \>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex7 = new System.Text.RegularExpressions.Regex(@"</p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex8 = new System.Text.RegularExpressions.Regex(@"<p>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regex9 = new System.Text.RegularExpressions.Regex(@"<[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            html = regex1.Replace(html, ""); //过滤<script></script>标记  
            html = regex2.Replace(html, ""); //过滤href=javascript: (<A>) 属性   
            html = regex0.Replace(html, ""); //过滤href=javascript: (<A>) 属性   


            //html = regex10.Replace(html, "");  
            html = regex3.Replace(html, "");// _disibledevent="); //过滤其它控件的on...事件  
            html = regex4.Replace(html, ""); //过滤iframe  
            html = regex5.Replace(html, ""); //过滤frameset  
            html = regex6.Replace(html, ""); //过滤frameset  
            html = regex7.Replace(html, ""); //过滤frameset  
            html = regex8.Replace(html, ""); //过滤frameset  
            html = regex9.Replace(html, "");
            //html = html.Replace(" ", "");  
            html = html.Replace("</strong>", "");
            html = html.Replace("<strong>", "");
            html = html.Replace(" ", "");
            return html;
        }
    }
}